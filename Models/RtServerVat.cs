using System;
using System.Collections.Generic;

namespace Loader.Models
{
    public sealed class RtServerVat
    {
        public RtServerVat()
        {
            TransactionVat = new HashSet<TransactionVat>();
        }

        public string SzVatCodeId { get; set; }
        public string SzVatNature { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public ICollection<TransactionVat> TransactionVat { get; set; }
    }
}
