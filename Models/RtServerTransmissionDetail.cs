using System;
using System.Collections.Generic;

namespace Loader.Models
{
    public sealed class RtServerTransmissionDetail
    {
        public RtServerTransmissionDetail()
        {
            RtServerTransmissionDetailRtData = new HashSet<RtServerTransmissionDetailRtData>();
            RtServerTransmissionDetailRtReport = new HashSet<RtServerTransmissionDetailRtReport>();
        }

        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public int LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        public string SzRtDeviceType { get; set; }
        public string SzRtTransmissionFormat { get; set; }
        public DateTime? DRtInactivityDateTimeFrom { get; set; }
        public DateTime? DRtInactivityDateTimeTo { get; set; }
        public DateTime? DRtDeviceClosureDateTime { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public RtServerTransmission RtServerTransmission { get; set; }
        public ICollection<RtServerTransmissionDetailRtData> RtServerTransmissionDetailRtData { get; set; }
        public ICollection<RtServerTransmissionDetailRtReport> RtServerTransmissionDetailRtReport { get; set; }
    }
}
