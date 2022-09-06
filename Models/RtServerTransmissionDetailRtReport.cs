using System;

namespace Loader.Models
{
    public class RtServerTransmissionDetailRtReport
    {
        public long LRtReportId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public int LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        public DateTime? DEventDateTime { get; set; }
        public string SzEventType { get; set; }
        public string SzEventNote { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual RtServerTransmissionDetail RtServerTransmissionDetail { get; set; }
    }
}
