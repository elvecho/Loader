using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class VatInfo
    {

        [XmlIgnore] private CultureInfo CultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlIgnore]
        public decimal GrossAmount { get; set; }

        [XmlElement("Aliquota")]
        public string RateFormatted
        {
            get => Rate.ToString(CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var rate);
                Rate = rate;
            }
        }

        [XmlIgnore]
        public decimal Rate { get; set; }

        [XmlElement("CodiceIva")]
        public int VatCode { get; set; }

        [XmlElement("Natura")]
        public string Nature { get; set; }

        [XmlElement("Attiva")]
        public int Active { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
