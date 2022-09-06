using Loader.Configuration;
using Loader.Data.RtStatus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Loader.Models;
using Loader.Persistence;
using System.Linq;
using System.Collections.Generic;

namespace Loader.Jobs
{
    public class RtServerStatusJob : RtServerGenericJob
    {
        private readonly List<string> _errorTable;

        public RtServerStatusJob(
            ILogger<RtServerStatusJob> logger,
            IOptions<JobsConfig> config,
            IHttpClientFactory clientFactory,IOptions<Error> errorTable) : base(logger, config, clientFactory)
        {
            _errorTable = errorTable.Value.TransactionErrorTable.Select(x=>x.Value).ToList();
        }

        protected override async Task<bool> JobPayload(RtServer rtServer)
        {
            Logger.LogWarning("BEG - RT Server {rtServer} Status - BEG", rtServer.SzRtServerId);
            bool success = false;

            RT_ChecksContext dbContext = NewDbContext();
            UpdateRtServerStatusLastTime(rtServer);

            try
            {
                HttpResponseMessage response = await GenericHttpGet(rtServer, Config.Value.RtServerStatusEndpoint, "");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug("XML = {content}", content);

                    UpdateRtServerStatus(rtServer, content);
                    success = true;
                }
                else
                {
                    Logger.LogWarning("Error in getting Status from {server}. StatusCode {status}",
                        rtServer.SzRtServerId, response.StatusCode);
                }

                response = await GenericHttpPost(rtServer, Config.Value.RtServerStatusEndpoint + "/html",
                    GetXmlFromGenericRequest(new ServerStatusRequest()));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Logger.LogDebug("Content = {content}", content);
                    var token = content.Split(";", 16);

                    if (token.Length > 15)
                    {
                        rtServer.RtServerStatus.SzLastCloseResult = token[15];
                    }

                    success = true;
                }
                else
                {
                    Logger.LogWarning("Error in getting additional info from {server}. StatusCode {status}",
                        rtServer.SzRtServerId, response.StatusCode);
                }
            }
            catch (HttpRequestException hre)
            {
                Logger.LogError("Error in getting Status from {server}: {e}", rtServer.SzRtServerId, hre);
            }

            try
            {
                CheckServerOnError(rtServer);
                CheckServerNonCompliant(rtServer);
                dbContext.Update(rtServer);
                await dbContext.SaveChangesAsync();
                Logger.LogDebug("DB info successfully updated for = {rtServer}", rtServer.SzRtServerId);
            }
            catch (Exception e)
            {
                Logger.LogError("Error updating DB info: {e}", e);
            }

            Logger.LogWarning("END - RT Server {rtServer} Status. Result: {result} - END", rtServer.SzRtServerId,
                success);
            return success;
        }

        private void CheckServerOnError(RtServer rtServer)
        {
            if (rtServer.RtServerStatus.DLastDateTimeRead != null &&
                rtServer.RtServerStatus.DLastDateTimeCollected != null)
            {
                TimeSpan difference =
                    rtServer.RtServerStatus.DLastDateTimeCollected.Value.Subtract(rtServer.RtServerStatus
                        .DLastDateTimeRead.Value);
                if (difference.TotalHours > Config.Value.PollingWithoutResponseMaxTime)
                {
                    rtServer.RtServerStatus.BOnErrorFlag = true;
                    rtServer.RtServerStatus.SzErrorDescription = Config.Value.PollingTimeoutMessage;
                }
                else
                {
                    rtServer.RtServerStatus.BOnErrorFlag = false;
                    rtServer.RtServerStatus.SzErrorDescription = "";
                }
            }
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
                    || (transactionsError.Any(er => er.SzRtDeviceId == srv.SzRtDeviceId
                                             && er.LRtClosureNmbr == srv.LRtClosureNmbr
                                             && er.LRtDocumentNmbr == srv.LRtDocumentNmbr)
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

        private void UpdateRtServerStatus(RtServer rtServer, string content)
        {
            var serializer = new XmlSerializer(typeof(RtStatus));

            using TextReader reader = new StringReader(content);
            var result = (RtStatus)serializer.Deserialize(reader);
            Logger.LogDebug("RtStatus Object = {result}", result);

            rtServer.RtServerStatus.DGrandTotalAmount = result.GrandTotal;
            rtServer.RtServerStatus.LPendingTransmissionNmbr = result.PendingTransmissions;
            rtServer.RtServerStatus.BRunningTransmissionFlag = result.ActiveTransmission;
            rtServer.RtServerStatus.LTransmissionScheduleMinutesLeft = result.ClosureSchedule.InMinutes;
            rtServer.RtServerStatus.LTransmissionScheduleHoursRepeat = result.ClosureSchedule.RepeatEveryHours;
            rtServer.RtServerStatus.LPendingTransmissionDays = result.PendingDays;
            rtServer.RtServerStatus.LPendingTransmissionNmbr = result.PendingTransmissions;
            rtServer.RtServerStatus.LMemoryAvailable = result.DetailMemory;
            rtServer.RtServerStatus.LLastClosureNmbr = result.LastClosureNumber;
            rtServer.RtServerStatus.DLastDateTimeCollected = DateTime.Now;

            DateTime kickOffDate = Config.Value.KickOffDate;
            if (rtServer.RtServerStatus.DLastDateTimeTransactionsCollected == null ||
                DateTime.Compare(rtServer.RtServerStatus.DLastDateTimeTransactionsCollected.Value, kickOffDate) < 0)
            {
                rtServer.RtServerStatus.DLastDateTimeTransactionsCollected = kickOffDate;
            }

            if (rtServer.RtServerStatus.DLastDateTimeTransmissionsCollected == null ||
                DateTime.Compare(rtServer.RtServerStatus.DLastDateTimeTransmissionsCollected.Value, kickOffDate) <
                0)
            {
                rtServer.RtServerStatus.DLastDateTimeTransmissionsCollected = kickOffDate;
            }
        }

        private void UpdateRtServerStatusLastTime(RtServer rtServer)
        {
            Logger.LogDebug("Updating RtServerStatus.DtLastDateTimeRead = {date}", DateTime.Now);

            rtServer.RtServerStatus ??= new RtServerStatus();

            rtServer.RtServerStatus.DLastDateTimeRead = DateTime.Now;
        }
    }
}