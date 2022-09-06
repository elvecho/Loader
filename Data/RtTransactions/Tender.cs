using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class Tender
    {
        [XmlIgnore] private CultureInfo CultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlElement("Descrizione")]
        public string Description { get; set; }


        [XmlIgnore]
        public decimal Amount { get; set; }

        [XmlElement("Importo")]
        public string AmountFormatted
        {
            get => Amount.ToString(CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var amount);
                Amount = amount;
            }
        }

        [XmlElement("Tipo")]
        public string Type { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
