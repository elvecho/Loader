using System;

namespace Loader.Models
{
    public class RtServerStatus
    {
        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public bool? BOnErrorFlag { get; set; }
        public string SzErrorDescription { get; set; }
        public bool? BVatVentilationFlag { get; set; }
        public short? LLastClosureNmbr { get; set; }
        public string SzLastCloseResult { get; set; }
        public short? LMemoryAvailable { get; set; }
        public decimal? DGrandTotalAmount { get; set; }
        public short? LPendingTransmissionNmbr { get; set; }
        public short? LPendingTransmissionDays { get; set; }
        public bool? BRunningTransmissionFlag { get; set; }
        public short? LTransmissionScheduleMinutesLeft { get; set; }
        public short? LTransmissionScheduleHoursRepeat { get; set; }
        public DateTime? DLastDateTimeRead { get; set; }
        public DateTime? DLastDateTimeCollected { get; set; }
        public DateTime? DLastDateTimeTransmissionsCollected { get; set; }
        public DateTime? DLastDateTimeTransactionsCollected { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
        public bool? BWarningFlag { get; set; }

        public virtual RtServer RtServer { get; set; }
    }
}
