using System;
using System.Collections.Generic;

namespace Loader.Models
{
    public sealed class RtServer
    {
        public RtServer()
        {
            TransactionAffiliation = new HashSet<TransactionAffiliation>();
            TransactionRtError = new HashSet<TransactionRtError>();
        }

        public string SzRtServerId { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public string SzLocation { get; set; }
        public string SzIpAddress { get; set; }
        public string SzUsername { get; set; }
        public string SzPassword { get; set; }
        public bool? BOnDutyFlag { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public Store L { get; set; }
        public RtServerStatus RtServerStatus { get; set; }
        public ICollection<TransactionAffiliation> TransactionAffiliation { get; set; }
        public ICollection<TransactionRtError> TransactionRtError { get; set; }
    }
}
