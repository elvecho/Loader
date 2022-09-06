using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class Summary
    {
        [XmlElement("IVA")]
        public Vat Vat { get; set; }

        [XmlElement("Natura")]
        public string Nature { get; set; }

        [XmlElement("Ammontare")]
        public decimal Amount { get; set; }

        [XmlElement("TotaleAmmontareResi")]
        public decimal ReturnsAmount { get; set; }

        [XmlElement("TotaleAmmontareAnnulli")]
        public decimal CancellationsAmount { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
