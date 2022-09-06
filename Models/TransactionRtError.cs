using System;

namespace Loader.Models
{
    public class TransactionRtError
    {
        public long LRtErrorId { get; set; }
        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public string SzRtDeviceId { get; set; }
        public DateTime? DRtDateTime { get; set; }
        public int? LRtClosureNmbr { get; set; }
        public int? LRtDocumentNmbr { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual RtServer RtServer { get; set; }
    }
}
