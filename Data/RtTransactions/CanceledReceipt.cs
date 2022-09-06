using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class CanceledReceipt : Receipt
    {
        [XmlElement("RiferimentoDocumento")]
        public ReceiptReference ReceiptReference { get; set; }

        public CanceledReceipt()
        {
            ReceiptDetails = new List<ReceiptDetail>();
            VatTotals = new List<VatTotal>();
            CultureInfo = CultureInfo.CreateSpecificCulture("it-IT");
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
