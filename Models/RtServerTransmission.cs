using System;
using System.Collections.Generic;

namespace Loader.Models
{
    public sealed class RtServerTransmission
    {
        public RtServerTransmission()
        {
            RtServerTransmissionDetail = new HashSet<RtServerTransmissionDetail>();
        }

        public string SzRtServerId { get; set; }
        public int LRtServerOperationId { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public ICollection<RtServerTransmissionDetail> RtServerTransmissionDetail { get; set; }
    }
}
