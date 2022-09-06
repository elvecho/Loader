using Loader.Configuration;
using Loader.Data.RtTransmissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Loader.Models;
using Loader.Persistence;

namespace Loader.Jobs
{
    class RtServerTransmissionsJob : RtServerGenericJob
    {
        public RtServerTransmissionsJob(
            ILogger<RtServerStatusJob> logger,
            IOptions<JobsConfig> config,
            IOptions<Error> errorTable,
            IHttpClientFactory clientFactory) : base(logger, config, clientFactory)
        {
        }

        protected override async Task<bool> JobPayload(RtServer rtServer)
        {
            Logger.LogWarning("BEG - RT Server {rtServer} Transmissions Details - BEG", rtServer.SzRtServerId);
            bool success = false;

            try
            {
                HttpResponseMessage response = await GenericHttpGet(rtServer,
                    Config.Value.RtServerTransmissionsEndpoint, SetQueryString(rtServer));

                if (response.IsSuccessStatusCode)
                {
                    RT_ChecksContext dbContext = NewDbContext();
                    var content = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug("XML = {content}", content);

                    var list = GetSendingsListFromXml(content);
                    if (list != null)
                    {
                        if (await UpdateRtServerTransmissions(dbContext, list, rtServer))
                        {
                            //TODO: Update DLastDateTimeTransmissionsCollected
                            rtServer.RtServerStatus.DLastDateTimeTransmissionsCollected = DateTime.Now;
                            try
                            {
                                dbContext.Update(rtServer);
                                await dbContext.SaveChangesAsync();
                                success = true;
                                Logger.LogDebug(
                                    "DLastDateTimeTransmissionsCollected DB info successfully updated for = {rtServer}",
                                    rtServer.SzRtServerId);
                            }
                            catch (Exception e)
                            {
                                Logger.LogError("Error updating DB info: {e}", e);
                            }
                        }
                    }
                    else
                    {
                        Logger.LogInformation("No new transmissions from {server}", rtServer.SzRtServerId);
                    }
                }
                else
                {
                    Logger.LogWarning("Error in getting Transmissions from {server}. StatusCode {status}",
                        rtServer.SzRtServerId, response.StatusCode);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Error getting the transmissions from {rtServer}: {e}", rtServer.SzRtServerId, e);
            }

            Logger.LogWarning("END - RT Server {rtServer} Transmissions Details. Result: {result} - END",
                rtServer.SzRtServerId, success);
            return success;
        }

        private async Task<bool> UpdateRtServerTransmissions(RT_ChecksContext dbContext,
            IEnumerable<Sending> sendingsList, RtServer rtServer)
        {
            var updated = false;
            foreach (var sending in sendingsList)
            {
                //Errore  : mancava OperationId quindi bisogna ignorare
                //i OperationOutCome senza OperationId
                //if (!int.TryParse(sending.OperationOutcome.OperationId, out int result)) continue;
                if (sending.OperationOutcome != null &&
                    sending.OperationOutcome.OperationId != null && 
                    int.TryParse(sending.OperationOutcome.OperationId, out int result))
                {
                    var transmission = await GetTransmission(dbContext, result, rtServer.SzRtServerId);
                    if (transmission != null)
                    {
                        Logger.LogDebug("Transmission with ID = {id} already loaded", transmission.LRtServerOperationId);
                    }
                    else
                    {
                        transmission = new RtServerTransmission
                        {
                            SzRtServerId = rtServer.SzRtServerId,
                            LRtServerOperationId = result,
                            RtServerTransmissionDetail = new List<RtServerTransmissionDetail>()
                        };
                        try
                        {
                            await dbContext.RtServerTransmission.AddAsync(transmission);
                            await dbContext.SaveChangesAsync();
                            updated = true;
                            Logger.LogDebug("Transmission {transmission} DB info successfully updated for = {rtServer}",
                                rtServer.SzRtServerId);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("Error updating DB info: {e}", e);
                        }
                    }

                    try
                    {
                        UpdateRtServerTransmissionsDetails(dbContext, transmission, sending, rtServer);
                        await dbContext.SaveChangesAsync();
                        Logger.LogDebug("DB info successfully updated for = {rtServer}", rtServer.SzRtServerId);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Error updating DB info: {e}", e);
                    }
                }
            }

                

            return updated; //TODO: Check this. Differentiate result depending on operations performed			
        }

        private void UpdateRtServerTransmissionsDetails(RT_ChecksContext dbContext, RtServerTransmission transmission,
            Sending sending, RtServer rtServer)
        {
            var transmissionDetail = transmission.RtServerTransmissionDetail.FirstOrDefault(td =>
                td.LRtDeviceTransmissionId == sending.AmountsData.Transmission.Sequence
                && td.LRtServerOperationId == int.Parse(sending.OperationOutcome.OperationId)
                && td.SzRtDeviceId == sending.AmountsData.Transmission.Device.Id
                && td.SzRtServerId == rtServer.SzRtServerId);

            if (transmissionDetail != null)
            {
                Logger.LogDebug("TransmissionDetail with ID = {id} already loaded",
                    transmissionDetail.LRtServerOperationId);
            }
            else
            {
                transmissionDetail = new RtServerTransmissionDetail
                {
                    LRtDeviceTransmissionId = sending.AmountsData.Transmission.Sequence,
                    LRtServerOperationId = sending.OperationOutcome.OperationId.Length,
                    SzRtDeviceId = sending.AmountsData.Transmission.Device.Id,
                    SzRtServerId = rtServer.SzRtServerId,
                    SzRtDeviceType = sending.AmountsData.Transmission.Device.Type,
                    SzRtTransmissionFormat = sending.AmountsData.Transmission.Format,
                    DRtDeviceClosureDateTime = sending.AmountsData.CollectionDate,
                    RtServerTransmissionDetailRtData = BuildTransmissionDetailRtData(sending, rtServer),
                    RtServerTransmissionDetailRtReport = BuildTransmissionDetailRtReport(sending, rtServer)
                };
                transmission.RtServerTransmissionDetail.Add(transmissionDetail);
                dbContext.Update(transmission);
            }
        }

        private ICollection<RtServerTransmissionDetailRtReport> BuildTransmissionDetailRtReport(Sending sending,
            RtServer rtServer)
        {
            return sending.AmountsData.Warnings.Select(warning => new RtServerTransmissionDetailRtReport
                {
                    LRtDeviceTransmissionId = sending.AmountsData.Transmission.Sequence,
                    LRtServerOperationId = sending.OperationOutcome.OperationId.Length,
                    SzRtDeviceId = warning.SerialCode,
                    SzRtServerId = rtServer.SzRtServerId,
                    DEventDateTime = warning.Date, //TODO: Check this, probably DB column issue
                    SzEventType = warning.Code,
                    SzEventNote = warning.Notes
                })
                .ToList();
        }

        private ICollection<RtServerTransmissionDetailRtData> BuildTransmissionDetailRtData(Sending sending,
            RtServer rtServer)
        {
            return sending.AmountsData.RtData.Summaries.Select(summary => new RtServerTransmissionDetailRtData
                {
                    LRtDeviceTransmissionId = sending.AmountsData.Transmission.Sequence,
                    LRtServerOperationId = sending.OperationOutcome.OperationId.Length,
                    SzRtDeviceId = sending.AmountsData.Transmission.Device.Id,
                    SzRtServerId = rtServer.SzRtServerId,
                    DVatRate = summary.Vat?.Rate ?? 0,
                    DVatAmount = summary.Vat?.Tax ?? 0,
                    SzVatNature = summary.Nature,
                    DSaleAmount = summary.Amount,
                    DReturnAmount = summary.ReturnsAmount,
                    DVoidAmount = summary.CancellationsAmount
                })
                .ToList();
        }

        private ICollection<Sending> GetSendingsListFromXml(string content)
        {
            var serializer = new XmlSerializer(typeof(TransmissionsInfo));

            try
            {
                using TextReader reader = new StringReader(content);
                var result = (TransmissionsInfo) serializer.Deserialize(reader);
                Logger.LogDebug("RtTransmissionInfo Object = {result}", result);

                return result.Sendings;
            }
            catch (InvalidOperationException ioe)
            {
                Logger.LogError("Error parsing XML {}", ioe);
                return null;
            }
        }

        private string SetQueryString(RtServer rtServer)
        {
            var kickOffDate = Config.Value.KickOffDate;
            var dateFrom = rtServer.RtServerStatus.DLastDateTimeTransmissionsCollected == null
                ? kickOffDate
                : (DateTime) rtServer.RtServerStatus.DLastDateTimeTransmissionsCollected;

            var queryString = "?da=" + dateFrom.ToString("dd-MM-yyyy hh:mm");
            queryString += "&a=" + DateTime.Now.ToString("dd-MM-yyyy hh:mm");
            queryString += "&Type=C";
            return queryString;
        }

        public async Task<RtServerTransmission> GetTransmission(RT_ChecksContext dbContext, int rtServerOperationId,
            string rtServerId)
        {
            var res = await dbContext.RtServerTransmission
                .Include(x => x.RtServerTransmissionDetail)
                .ThenInclude(x => x.RtServerTransmissionDetailRtData)
                .Include(x => x.RtServerTransmissionDetail)
                .ThenInclude(x => x.RtServerTransmissionDetailRtReport)
                .FirstOrDefaultAsync(x =>
                    x.LRtServerOperationId == rtServerOperationId && x.SzRtServerId == rtServerId);
            return res;
        }
    }
}