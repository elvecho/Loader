using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class VatTotal
    {
        [XmlIgnore]
        public CultureInfo CultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlIgnore]
        public decimal GrossAmount { get; set; }

        [XmlElement("Importo")]
        public string GrossAmountFormatted
        {
            get => GrossAmount.ToString(CultureInfo);
            set
            {
                decimal.TryParse(value, NumberStyles.Any,CultureInfo, out var grossAmount);
                GrossAmount = grossAmount;
            }
        }

        [XmlIgnore]
        public decimal NetAmount { get; set; }

        [XmlElement("BaseImponibile")]
        public string NetAmountFormatted
        {
            get => NetAmount.ToString(CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var netAmount);
                NetAmount = netAmount;
            }
        }

        [XmlIgnore]
        public decimal TaxAmount { get; set; }

        [XmlElement("Imposta")]
        public string TaxAmountFormatted
        {
            get => TaxAmount.ToString(CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var taxAmount);
                TaxAmount = taxAmount;
            }
        }

        [XmlElement("CodiceIVA")]
        public VatDetail VatCode { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
