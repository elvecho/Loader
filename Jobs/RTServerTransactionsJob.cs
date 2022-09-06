using Loader.Configuration;
using Loader.Data.RtTransactions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using Loader.Models;
using Loader.Persistence;
using System.Collections;
using Error = Loader.Configuration.Error;

namespace Loader.Jobs
{
    public class RtServerTransactionsJob : RtServerGenericJob
    {
        private readonly List<string> _errorTable;
        private readonly int TYPE_SERVERRT = 2;
        public RtServerTransactionsJob(
            ILogger<RtServerStatusJob> logger,
            IOptions<JobsConfig> config,
            IHttpClientFactory clientFactory,
            IOptions<Error> errorTable
            ) : base(logger, config, clientFactory)
        {
            _errorTable = errorTable.Value.TransactionErrorTable.Select(x => x.Value).ToList();
        }

        protected override async Task<bool> JobPayload(RtServer rtServer)
        {
            Logger.LogWarning("BEG - RT Server {rtServer} Transactions Details - BEG", rtServer.SzRtServerId);
            var devicesList = await GetDevicesList(rtServer);
            
            var responses = await UpdateTransactions(devicesList, rtServer);

            Logger.LogWarning("END - RT Server {rtServer} Transactions Details. Result: {result} - END",
                rtServer.SzRtServerId, !responses.Contains(false));
            return !responses.Contains(false);
        }

        private async Task<ICollection<RtDevice>> GetDevicesList(RtServer rtServer)
        {
            Logger.LogDebug("Getting RT Devices");

            try
            {
                HttpResponseMessage response =
                    await GenericHttpGet(rtServer, Config.Value.RtServerDevicesStatusEndpoint, "");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug("XML = {content}", content);
                    return GetDevicesListFromXml(content);
                }
                else
                {
                    Logger.LogWarning("Error in getting Device list from {server}. StatusCode {status}",
                        rtServer.SzRtServerId, response.StatusCode);
                    return new List<RtDevice>();
                }
            }
            catch (HttpRequestException hre)
            {
                Logger.LogWarning("Error in getting Device list from {server}: {e}", rtServer.SzRtServerId, hre);
                return new List<RtDevice>();
            }
        }

        private ICollection<RtDevice> GetDevicesListFromXml(string content)
        {
            var serializer = new XmlSerializer(typeof(RtDeviceStatus));

            try
            {
                using TextReader reader = new StringReader(content);
                var result = (RtDeviceStatus) serializer.Deserialize(reader);
                Logger.LogDebug("RtDeviceStatus Object = {result}", result);

                return result.Devices;
            }
            catch (InvalidOperationException ioe)
            {
                Logger.LogError("Cannot Deserialize XML. Error: {ioe}", ioe);
                return new List<RtDevice>();
            }
        }

        private async Task<List<bool>> UpdateTransactions(ICollection<RtDevice> devicesList, RtServer rtServer)
        {
            DateTime kickOffDate =
                Config.Value.KickOffDate;
            DateTime dateFrom = rtServer.RtServerStatus.DLastDateTimeTransactionsCollected ?? kickOffDate;
            var responses = new List<bool>();
            DateTime dateTo = Config.Value.AfterEodProcessing
                ? DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0))
                : DateTime.Now.Date;

            for (var date = dateFrom; date.Date <= dateTo; date = date.AddDays(1))
            {
                if (date.Date == DateTime.Now.Date) date = DateTime.Now;
                responses = await UpdateTransactionsPerDate(devicesList, rtServer, date);
                if (responses.Contains(false)) break;
            }

            return responses;
        }

        private async Task<List<bool>> UpdateTransactionsPerDate(ICollection<RtDevice> devicesList, RtServer rtServer,
            DateTime date)
        {
            List<bool> responses = new List<bool>();

            Logger.LogInformation("Working in serial on: {devices} devices", devicesList.Count);
            List<Task<MemoryDetail>> tasks = new List<Task<MemoryDetail>>();

            foreach (RtDevice device in devicesList)
            {
                tasks.Add(RetrieveDeviceTransactionsPerDate(rtServer, device, date));
            }

            var memoryDetails = (await Task.WhenAll(tasks)).ToList();
            if (memoryDetails.Contains(null))
            {
                Logger.LogWarning("Cannot retrieve full info from {server}. Aborting.", rtServer);
                responses.Add(false);
                return responses;
            }

            responses.Add(await UpdateTransactionsFullListPerDate(rtServer, memoryDetails, date));
            responses.Add(await UpdateTransactionErrorsPerDate(rtServer, date));

            if (!responses.Contains(false))
            {
                rtServer.RtServerStatus.DLastDateTimeTransactionsCollected = date;
                try
                {
                    RT_ChecksContext dbContext = NewDbContext();
                    /**Soukaina 24/09 ho modfificato questa riga
                     * 
                     * dbContext.Update(rtServer); 
                     * sostituita con   dbContext.RtServer.Update(rtServer);
                     */
                    CheckServerNonCompliant(rtServer);
                    dbContext.RtServer.Update(rtServer);
                    await dbContext.SaveChangesAsync();
                    Logger.LogInformation(
                        "DLastDateTimeTransactionsCollected DB info successfully updated for = {rtServer}",
                        rtServer.SzRtServerId);
                }
                catch (Exception e)
                {
                    Logger.LogError("Error updating DB info: {e}", e);
                }
            }

            return responses;
        }
        /*
         * Editor:Soukaina
         * Description:add bWarningFlag for servers has error 
         * 
         * 
         */

        private void CheckServerNonCompliant(RtServer rtServer)
        {
            var x = _errorTable;
            RT_ChecksContext dbContext = NewDbContext();
            var transactionsError = dbContext.TransactionRtError.AsEnumerable()
                                            .Where(x => x.SzRtServerId == rtServer.SzRtServerId
                                                    && _errorTable.Any(er => x.SzDescription.StartsWith(er)))
                                            .ToList();

            var result = dbContext.TransactionAffiliation
                .Where(x => x.SzRtServerId == rtServer.SzRtServerId
                       && x.LRetailStoreId == rtServer.LRetailStoreId
                       && x.LStoreGroupId == rtServer.LStoreGroupId
                      )
                      .AsEnumerable()
                      .Any(srv => (srv.LTransactionMismatchId == 1
                    ||
                    srv.LTransactionMismatchId == 2
                    //||
                    //(srv.LTransactionMismatchId == 3 && srv.DRtDateTime.Value.Date < DateTime.Today.Date)
                    //||
                    //(srv.LTransactionMismatchId == 4 && srv.DPosDateTime.Value.Date < DateTime.Today.Date)
                    ||
                    (
                       (srv.DPosTransactionTurnover.HasValue && srv.DRtTransactionTurnover.HasValue)
                                 &&
                       (srv.DRtTransactionTurnover ?? 0) * (srv.LRtReceivedTransactionCounter ?? 1)
                                 !=
                       (srv.DPosTransactionTurnover ?? 0) * (srv.LPosReceivedTransactionCounter ?? 1)
                    )
                    || ( transactionsError.Any(er => er.SzRtDeviceId == srv.SzRtDeviceId
                                              && er.LRtClosureNmbr == srv.LRtClosureNmbr
                                              && er.LRtDocumentNmbr == srv.LRtDocumentNmbr)
                    /* 15/12/2020 ho aggiunto questa riga per farmi vedere solo le trn 
                     * che non sono compliant ma anche che hanno errori**/
                    &&
                         srv.BRtNonCompliantFlag == true
                    )

                    )
                    && srv.BTransactionCheckedFlag != true)
                    ;
            if (result != rtServer.RtServerStatus.BWarningFlag)
            {
                rtServer.RtServerStatus.BWarningFlag = result;
            }
        }
        private async Task<bool> UpdateTransactionErrorsPerDate(RtServer rtServer, DateTime date)
        {
            var request = new DeviceErrorsRequest
            {
                Date = date.ToString("yyyy-MM-dd")
            };

            try
            {
                var response = await GenericHttpPost(rtServer,
                                                    Config.Value.RtServerDevicesErrorEndpoint, 
                                                    GetXmlFromGenericRequest(request));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug("XML = {content}", content);

                    try
                    {
                        RT_ChecksContext dbContext = NewDbContext();
                        DeviceErrorsResponse deviceErrorsResponse = GetDeviceErrorsResponseFromXml(content);
                        dbContext.TransactionRtError.RemoveRange(dbContext.TransactionRtError.Where(te =>
                            date.Day.Equals(te.DRtDateTime.Value.Day) && rtServer.SzRtServerId == te.SzRtServerId));
                        await dbContext.SaveChangesAsync();

                        foreach (var transactionRtError in deviceErrorsResponse.Errors.Select(error => new TransactionRtError
                        {
                            SzRtServerId = rtServer.SzRtServerId,
                            LRetailStoreId = rtServer.LRetailStoreId,
                            LStoreGroupId = rtServer.LStoreGroupId,
                            SzRtDeviceId = error.DeviceId,
                            DRtDateTime = new DateTime(
                                Int32.Parse(error.Document.Date.Substring(4, 4)),
                                Int32.Parse(error.Document.Date.Substring(2, 2)),
                                Int32.Parse(error.Document.Date.Substring(0, 2)),
                                Int32.Parse(error.Document.Time.Substring(0, 2)),
                                Int32.Parse(error.Document.Time.Substring(3, 2)),
                                0),
                            LRtClosureNmbr = error.Document.ZNumber,
                            LRtDocumentNmbr = error.Document.DocumentNumber,
                            SzDescription = error.Description
                        }))
                        {
                            await dbContext.TransactionRtError.AddAsync(transactionRtError);
                        }

                        await dbContext.SaveChangesAsync();
                    }
                    catch (InvalidOperationException ioe)
                    {
                        Logger.LogError("Cannot Deserialize XML. Error: {ioe}", ioe);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Error: {e}", e);
                    }

                    return true;
                }
                else
                {
                    Logger.LogWarning("Error in getting Transaction Errors from {server}. StatusCode {status}",
                        rtServer.SzRtServerId, response.StatusCode);
                    return false;
                }
            }
            catch (Exception hre)
            {
                Logger.LogError("Error in getting Transaction Errors from {server}: {e}", rtServer.SzRtServerId, hre);
                return false;
            }
        }

        private async Task<MemoryDetail> RetrieveDeviceTransactionsPerDate(RtServer rtServer, RtDevice device,
            DateTime date)
        {
            MemoryDetailRequest request = new MemoryDetailRequest
            {
                DateFrom = date.ToString("yyyyMMdd"),
                DateTo = date.ToString("yyyyMMdd"),
                DeviceId = device.DeviceId
            };

            try
            {
                Logger.LogInformation("Retrieving info for {device}", device.DeviceId);
                var response = await GenericHttpPost(rtServer,
                    Config.Value.RtServerTransactionsEndpoint, GetXmlFromGenericRequest(request));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug("XML = {content}", content);
                    try
                    {
                        return GetMemoryDetailFromXml(content);
                    }
                    catch (InvalidOperationException ioe)
                    {
                        Logger.LogError("Cannot Deserialize XML. Error: {ioe}", ioe);
                        return null;
                    }
                }
                else
                {
                    Logger.LogWarning("Error in getting Device Transaction from {server}. StatusCode {status}",
                        rtServer.SzRtServerId, response.StatusCode);
                    return null;
                }
            }
            catch (Exception hre)
            {
                Logger.LogError("Error in getting Device Transaction from {server}: {e}", rtServer.SzRtServerId, hre);
                return null;
            }
        }

        private async Task<bool> UpdateTransactionsFullListPerDate(RtServer rtServer, List<MemoryDetail> fullList, DateTime date)
        {
            try
            {
                using var dbContext = NewDbContext();
                var dateFrom = date.Date;
                var dateTo = date.Date.AddDays(1);
             
                var transactionAffiliations = dbContext.TransactionAffiliation
                    .Include(tr => tr.TransactionVat).Include(tr => tr.TransactionDocument)
                    .Where(tr => tr.SzRtServerId == rtServer.SzRtServerId &&
                                 ((tr.DPosDateTime >= dateFrom && tr.DPosDateTime < dateTo) ||
                                  (tr.DRtDateTime >= dateFrom && tr.DRtDateTime < dateTo))).ToList();
                    
                Logger.LogInformation("Working on: {memoryDetails} details for date {date}", fullList.Count,
                    date.ToString("yyyyMMdd"));
                foreach (var memoryDetail in fullList)
                {
                    var noDuplicates = memoryDetail.SalesDocuments.GroupBy(sd => sd.CCDC)
                        .Select(grp => grp.FirstOrDefault()).ToList();
                    Logger.LogInformation(
                        "Working on: {documents} sales documents from device {deviceId} for date {date}",
                        noDuplicates.Count, memoryDetail.DeviceId, date.ToString("yyyyMMdd"));

                    foreach (var salesDocument in noDuplicates)
                    {
                        int sign = 1;
                        int count = memoryDetail.SalesDocuments.Count(sd => sd.CCDC == salesDocument.CCDC);
                        if (salesDocument.Receipt == null && salesDocument.CanceledReceipt != null)
                        {
                            Logger.LogInformation("It's a cancellation receipt {receipt}",
                                salesDocument.CanceledReceipt.DocumentNumber);
                            salesDocument.Receipt = salesDocument.CanceledReceipt;
                            sign = -1;
                        }
                        
                        if (salesDocument.Receipt != null)
                        {
                            UpdateTransaction(dbContext, salesDocument, memoryDetail, rtServer,
                                transactionAffiliations, sign, Convert.ToByte(count));//,transactionsError);
                            
                        }
                    }
                }

               await BulkInsertOrUpdate(dbContext, transactionAffiliations);

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError("Error in updating DB info for {server}: {e}", rtServer.SzRtServerId, e);
                return false;
            }
        }

        private async Task BulkInsertOrUpdate(RT_ChecksContext dbContext,
            IList<TransactionAffiliation> transactionAffiliations)
        {
            using var transaction = dbContext.Database.BeginTransaction();
             await dbContext.BulkInsertOrUpdateAsync(transactionAffiliations,
                new BulkConfig {PreserveInsertOrder = true, SetOutputIdentity = true});
            var transactionVats = new List<TransactionVat>();
            var transactionDocuments = new List<TransactionDocument>();
            long index = -100000;
            foreach (var transactionAffiliation in transactionAffiliations)
            {
                foreach (var transactionVat in transactionAffiliation.TransactionVat)
                {
                    transactionVat.SzRtDocumentId = transactionAffiliation.SzRtDocumentId;
                }

                transactionVats.AddRange(transactionAffiliation.TransactionVat);

                foreach (var transactionDocument in transactionAffiliation.TransactionDocument.Where(td => td.LTransactionDocumentId == 0))
                {
                    transactionDocument.SzRtDocumentId = transactionAffiliation.SzRtDocumentId;
                    transactionDocument.LTransactionDocumentId = index++;
                    transactionDocuments.Add(transactionDocument);
                }
            }

            dbContext.BulkInsertOrUpdate(transactionVats);
            dbContext.BulkInsert(transactionDocuments);
            transaction.Commit();
        }

        private void UpdateTransaction(RT_ChecksContext dbContext, SalesDocument salesDocument,
            MemoryDetail memoryDetail, RtServer rtServer, List<TransactionAffiliation> transactionAffiliations,
            int sign, byte count)//,List<TransactionRtError> transactionsError)
        {
           
            
            var transaction = transactionAffiliations.FirstOrDefault(ta => ta.SzRtDocumentId == salesDocument.CCDC);

            if (transaction != null)
            {
                transaction.SzRtDocumentId = salesDocument.CCDC;
                transaction.DRtTransactionTurnover = salesDocument.Receipt.Turnover * sign;
                transaction.SzRtDeviceId = memoryDetail.DeviceId;
                transaction.LRtClosureNmbr = salesDocument.Receipt.ZNumber;
                transaction.LRtDocumentNmbr = salesDocument.Receipt.DocumentNumber;
                transaction.BRtNonCompliantFlag = salesDocument.NonCompliant != 0;
                transaction.LRtReceivedTransactionCounter = count;
                transaction.DRtDateTime = new DateTime(
                    Int32.Parse(salesDocument.Receipt.Date.Substring(4, 4)),
                    Int32.Parse(salesDocument.Receipt.Date.Substring(2, 2)),
                    Int32.Parse(salesDocument.Receipt.Date.Substring(0, 2)),
                    Int32.Parse(salesDocument.Receipt.Time.Substring(0, 2)),
                    Int32.Parse(salesDocument.Receipt.Time.Substring(3, 2)),
                    0);

                UpdateTransactionVat(transaction, salesDocument, sign);
                //UpdateTransactionDocument(transaction, salesDocument, dbContext);
                UpdateTransactionDocument(transaction, salesDocument,memoryDetail, dbContext);


                if (transaction.DPosTransactionTurnover != null && transaction.LPosReceivedTransactionCounter != null)
                {
                    transaction.LTransactionMismatchId =
                        transaction.DRtTransactionTurnover * transaction.LPosReceivedTransactionCounter
                        != transaction.DPosTransactionTurnover * transaction.LPosReceivedTransactionCounter
                            ? 1
                            : 0;
                //    /*
                //* Editor:Soukaina
                //* Description: Valorize BWarningFlag if necessary **/

                //    if (
                //       (transaction.LTransactionMismatchId == 1
                //        ||
                //        transaction.LTransactionMismatchId == 2
                //        ||
                //        (transaction.LTransactionMismatchId == 3 && transaction.DRtDateTime.Value.Date < DateTime.Today.Date)
                //        ||
                //        (transaction.LTransactionMismatchId == 4 && transaction.DPosDateTime.Value.Date < DateTime.Today.Date)
                //        ||
                //        (
                //           (transaction.DPosTransactionTurnover.HasValue && transaction.DRtTransactionTurnover.HasValue)
                //                     &&
                //           (transaction.DRtTransactionTurnover ?? 0) * (transaction.LRtReceivedTransactionCounter ?? 1)
                //                     !=
                //           (transaction.DPosTransactionTurnover ?? 0) * (transaction.LPosReceivedTransactionCounter ?? 1)
                //        )
                //        || transactionsError.Any(er => er.SzRtDeviceId == transaction.SzRtDeviceId
                //                                  && er.LRtClosureNmbr == transaction.LRtClosureNmbr
                //                                  && er.LRtDocumentNmbr == transaction.LRtDocumentNmbr)
                //        )
                //        && transaction.BTransactionCheckedFlag != true
                //        )
                //    {
                //        rtServer.RtServerStatus.BWarningFlag = true;
                //    }

                }
                else
                {
                    transaction.LTransactionMismatchId = 3;
                }
               
            }
            else
            {
                transaction = new TransactionAffiliation
                {
                    DRtDateTime = new DateTime(
                        Int32.Parse(salesDocument.Receipt.Date.Substring(4, 4)),
                        Int32.Parse(salesDocument.Receipt.Date.Substring(2, 2)),
                        Int32.Parse(salesDocument.Receipt.Date.Substring(0, 2)),
                        Int32.Parse(salesDocument.Receipt.Time.Substring(0, 2)),
                        Int32.Parse(salesDocument.Receipt.Time.Substring(3, 2)),
                        0),
                    DRtTransactionTurnover = salesDocument.Receipt.Turnover * sign,
                    SzRtDocumentId = salesDocument.CCDC,
                    SzRtDeviceId = memoryDetail.DeviceId,
                    LRtClosureNmbr = salesDocument.Receipt.ZNumber,
                    LRtDocumentNmbr = salesDocument.Receipt.DocumentNumber,
                    SzRtServerId = rtServer.SzRtServerId,
                    LStoreGroupId = rtServer.LStoreGroupId,
                    LRetailStoreId = rtServer.LRetailStoreId,
                    BRtNonCompliantFlag = salesDocument.NonCompliant == 1,
                    LRtReceivedTransactionCounter = count,
                    LTransactionMismatchId = 3,
                    TransactionVat = new List<TransactionVat>()
                };

               
                BuildTransactionVat(transaction, salesDocument, dbContext, sign);
                //BuildTransactionDocument(transaction, salesDocumentToSave, dbContext);
                BuildTransactionDocument(transaction, salesDocument, memoryDetail, dbContext);
                transactionAffiliations.Add(transaction);
            }
        }
        private TransactionDocument SerializeTransactionDocument(TransactionAffiliation transaction,
           SalesDocument salesDocument, MemoryDetail memoryDetail)
        {
            TransactionDocument transactionDocument = null;
            var salesDocumentSerialize = new SalesDocumentSerialize()
            {
                CanceledReceipt = salesDocument.CanceledReceipt,
                CCDC = salesDocument.CCDC,
                NonCompliant = salesDocument.NonCompliant,
                PreviousCCDC = salesDocument.PreviousCCDC,
                Receipt = salesDocument.Receipt,
                RegisterId = salesDocument.RegisterId,
                Cassa = memoryDetail.DeviceId ?? "",
                IdentificativoDGFE = memoryDetail.DGFEId ?? "",
                Marchio = memoryDetail.RecordMatriculation.Measurer.Mark ?? "",
                Matricola = memoryDetail.RecordMatriculation.Measurer.Mark ?? "",
                Modello = memoryDetail.RecordMatriculation.Measurer.Model ?? ""
            };

            var serializer = new XmlSerializer(typeof(SalesDocumentSerialize));
           

            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            try
            {

                using var stringWriter = new StringWriter();
                using var writer = XmlWriter.Create(stringWriter, settings);

                serializer.Serialize(writer, salesDocumentSerialize);

                var xml = stringWriter.ToString();
                Logger.LogDebug("XML = {content}", xml);
                transactionDocument = new TransactionDocument
                {
                    SzRtDocumentId = transaction.SzRtDocumentId,
                    LDocumentTypeId = TYPE_SERVERRT,
                    SzDocumentAttachment = xml
                };

            }
            catch (InvalidOperationException ioe)
            {
                Logger.LogError("Cannot Serialize XML. Error: {e}", ioe);
            }
            catch (Exception e)
            {
                Logger.LogError("Error: {e}", e);
            }
            return transactionDocument;

            ///*End changes*/	



        }
        /*
         * Editor:Soukaina
         * Description:Manage Serialize**/
        public XmlSerializer CreateOverriderSerializer()
        {
            Hashtable serializers = new Hashtable();
            // Create the XmlAttributeOverrides and XmlAttributes objects.
            XmlAttributes attributes = new XmlAttributes { XmlIgnore = false };
            XmlAttributeOverrides xmlOverrider = new XmlAttributeOverrides();

            /* Setting XmlIgnore to false overrides the XmlIgnoreAttribute
               applied to the Comment field. Thus it will be serialized.*/
            xmlOverrider.Add(typeof(SalesDocument), "IdentificativoDGFE", attributes);
            xmlOverrider.Add(typeof(SalesDocument), "Cassa", attributes);
            xmlOverrider.Add(typeof(SalesDocument), "Marchio", attributes);
            xmlOverrider.Add(typeof(SalesDocument), "Modello", attributes);
            xmlOverrider.Add(typeof(SalesDocument), "Matricola", attributes);


            /* Use the XmlIgnore to instruct the XmlSerializer to ignore
               the GroupName instead. */

            //XmlSerializer serializer = new XmlSerializer(typeof(SalesDocument), xmlOverrider);
            object key = GenerateKey(typeof(SalesDocument), xmlOverrider);

            // Check the local cache for a matching serializer.  
            XmlSerializer serializer = (XmlSerializer)serializers[key];
            if (serializer == null)
            {
                serializer = new XmlSerializer(typeof(SalesDocument), xmlOverrider);
                // Cache the serializer.  
                serializers[key] = serializer;
            }
            return serializer;
        }

        private object GenerateKey(Type type, XmlAttributeOverrides xmlOverrider)
        {
            var key = String.Format("{0}:{1}", type,  Guid.NewGuid().ToString());
           
            return key;
        }

        //end
       
        private void BuildTransactionDocument(TransactionAffiliation transaction, SalesDocument salesDocument,MemoryDetail memoryDetail, RT_ChecksContext dbContext)
        {
            transaction.TransactionDocument.Add(SerializeTransactionDocument(transaction, salesDocument,  memoryDetail));
        }

        private void UpdateTransactionDocument(TransactionAffiliation transaction, SalesDocument salesDocument, MemoryDetail memoryDetail, RT_ChecksContext dbContext)
        {
            var transactionDocument =
                transaction.TransactionDocument.FirstOrDefault(td => td.LDocumentTypeId == TYPE_SERVERRT);

            if (transactionDocument != null)
            {
                
                Logger.LogDebug("Document already present");
            }
            else
            {
                transaction.TransactionDocument.Add(SerializeTransactionDocument(transaction, salesDocument,memoryDetail));
            }
        }

        private void BuildTransactionVat(TransactionAffiliation transaction, SalesDocument salesDocument,
            RT_ChecksContext dbContext, int sign)
        {
            foreach (var vatTotal in salesDocument.Receipt.VatTotals)
            {
                string vatCode = string.IsNullOrEmpty(vatTotal.VatCode.VatExemption)
                    ? vatTotal.VatCode.RateFormatted
                    : $"{vatTotal.VatCode.VatExemption,-3}";
                vatCode = dbContext.RtServerVat.Any(rv => rv.SzVatCodeId == vatCode) ? vatCode : "XX";

                var transactionVat = new TransactionVat
                {
                    SzRtDocumentId = transaction.SzRtDocumentId,
                    SzVatCodeId = vatCode,
                    DRtGrossAmount = vatTotal.GrossAmount * sign,
                    DRtNetAmount = vatTotal.NetAmount * sign,
                    DRtVatAmount = vatTotal.TaxAmount * sign,
                    DRtVatRate = vatTotal.VatCode.Rate,
                };
                transaction.TransactionVat.Add(transactionVat);
            }
        }

        private void UpdateTransactionVat(TransactionAffiliation transaction, SalesDocument salesDocument, int sign)
        {
            foreach (var transactionVat in transaction.TransactionVat)
            {
                var trnVatDoc = salesDocument.Receipt.VatTotals.FirstOrDefault(vt =>
                    vt.VatCode.RateFormatted != null && vt.VatCode.RateFormatted.Contains(transactionVat.SzVatCodeId)
                    || (vt.VatCode.VatExemption != null &&
                        vt.VatCode.VatExemption.Contains(transactionVat.SzVatCodeId)));

                if (trnVatDoc != null)
                {
                    transactionVat.DRtGrossAmount = trnVatDoc.GrossAmount * sign;
                    transactionVat.DRtNetAmount = trnVatDoc.NetAmount * sign;
                    transactionVat.DRtVatAmount = trnVatDoc.TaxAmount * sign;
                    transactionVat.DRtVatRate = trnVatDoc.VatCode.Rate;

                    if (transactionVat.DPosVatRate != transactionVat.DRtVatRate
                        || transactionVat.DPosGrossAmount != transactionVat.DRtGrossAmount
                        || transactionVat.DPosNetAmount != transactionVat.DRtNetAmount
                        || transactionVat.DPosVatAmount != transactionVat.DRtVatAmount)
                    {
                        transaction.LTransactionMismatchId ??= 2;
                        transactionVat.BVatMismatchFlag = true;
                    }
                }
                else
                {
                    //TODO: Insert missing vat from POS
                }
            }
        }

        private MemoryDetail GetMemoryDetailFromXml(string content)
        {
            var serializer = new XmlSerializer(typeof(MemoryDetail));

            using TextReader reader = new StringReader(content);
            var result = (MemoryDetail) serializer.Deserialize(reader);
            Logger.LogTrace("MemoryDetail Object = {result}", result);

            return result;
        }
        
        private DeviceErrorsResponse GetDeviceErrorsResponseFromXml(string content)
        {
            var serializer = new XmlSerializer(typeof(DeviceErrorsResponse));

            using TextReader reader = new StringReader(content);
            var result = (DeviceErrorsResponse) serializer.Deserialize(reader);
            Logger.LogTrace("DeviceErrorsResponse Object = {result}", result);

            return result;
        }
    }
}