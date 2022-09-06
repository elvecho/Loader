using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class VatDetail
    {
        [XmlIgnore] private CultureInfo CultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlIgnore]
        public decimal Rate { get; set; }

        [XmlElement("Aliquota")]
        public string RateFormatted
        {

            get => Rate.ToString("00.00",CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var rate);
                Rate = rate;
            }
        }

        [XmlElement("CodiceEsenzioneIVA")]
        public string VatExemption { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
