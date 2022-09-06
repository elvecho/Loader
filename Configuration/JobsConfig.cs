using System;
using System.Collections.Generic;

namespace Loader.Configuration
{
    public class JobsConfig
    {
        public int HttpRequestTimeOut { get; set; }
        public bool RtServerStatusEnable { get; set; }
        public string RtServerStatusEndpoint { get; set; }
        public string RtServerStatusCron { get; set; }
        public bool RtServerTransactionsEnable { get; set; }
        public string RtServerTransactionsEndpoint { get; set; }
        public string RtServerTransactionsCron { get; set; }
        public bool RtServerTransmissionsEnable { get; set; }
        public string RtServerTransmissionsEndpoint { get; set; }
        public string RtServerTransmissionsCron { get; set; }
        public string RtServerDevicesStatusEndpoint { get; set; }
        public string RtServerDevicesErrorEndpoint { get; set; }
        public bool ParallelServersProcessing { get; set; }
        public DateTime KickOffDate { get; set; }
        public int PollingWithoutResponseMaxTime { get; set; }
        public string PollingTimeoutMessage { get; set; }
        public bool AfterEodProcessing { get; set; }
    }
}
