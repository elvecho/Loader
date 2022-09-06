using System;
using System.Collections.Generic;

namespace Loader.Models
{
    public sealed class DocumentType
    {
        public DocumentType()
        {
            TransactionDocument = new HashSet<TransactionDocument>();
        }

        public int LDocumentTypeId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public ICollection<TransactionDocument> TransactionDocument { get; set; }
    }
}
