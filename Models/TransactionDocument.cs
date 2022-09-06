using System;

namespace Loader.Models
{
    public class TransactionDocument
    {
        public long LTransactionDocumentId { get; set; }
        public string SzRtDocumentId { get; set; }
        public int? LDocumentTypeId { get; set; }
        public string SzDocumentNote { get; set; }
        public string SzDocumentName { get; set; }
        public string SzDocumentAttachment { get; set; }
        public string SzDocumentAttachmentTxt { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public virtual DocumentType LDocumentType { get; set; }
        public virtual TransactionAffiliation LTransactionAffiliation { get; set; }
    }
}
