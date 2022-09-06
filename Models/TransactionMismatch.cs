using System;
using System.Collections.Generic;

namespace Loader.Models
{
    public sealed class TransactionMismatch
    {
        public TransactionMismatch()
        {
            TransactionAffiliation = new HashSet<TransactionAffiliation>();
        }

        public int LTransactionMismatchId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public ICollection<TransactionAffiliation> TransactionAffiliation { get; set; }
    }
}
