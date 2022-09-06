using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class Receipt
    {
        [XmlIgnore] protected CultureInfo CultureInfo { get; set; } 

        [XmlElement("Data")]
        public string Date { get; set; }

        [XmlElement("Ora")]
        public string Time { get; set; }

        [XmlElement("NumeroAzzeramento")]
        public int ZNumber { get; set; }

        [XmlElement("NumeroDocumento")]
        public int DocumentNumber { get; set; }

        [XmlIgnore]
        public decimal Turnover { get; set; }

        [XmlElement("Totale")]
        public string TurnoverFormatted
        {
            get => Turnover.ToString(CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var turnover);
                Turnover = turnover;
            }
        }

        [XmlElement("Dettagli")]
        public List<ReceiptDetail> ReceiptDetails { get; set; }

        [XmlElement("CorrispettiviIVA")]
        public List<VatTotal> VatTotals { get; set; }

        public Receipt()
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
