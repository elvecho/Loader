using System;

namespace Loader.Models
{
    public class RtServerTransmissionDetailRtData
    {
        public long LRtDataId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public int LRtDeviceTransmissionId { get; set; }
        public string SzRtDeviceId { get; set; }
        public decimal DVatRate { get; set; }
        public decimal? DVatAmount { get; set; }
        public string SzVatNature { get; set; }
        public string SzVatLegalReference { get; set; }
        public bool? BVatVentilation { get; set; }
        public decimal? DSaleAmount { get; set; }
        public decimal? DReturnAmount { get; set; }
        public decimal? DVoidAmount { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual RtServerTransmissionDetail RtServerTransmissionDetail { get; set; }
    }
}
