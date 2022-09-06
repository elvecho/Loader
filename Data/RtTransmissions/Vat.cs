using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class Vat
    {
        [XmlElement("AliquotaIVA")]
        public decimal Rate { get; set; }

        [XmlElement("Imposta")]
        public decimal Tax { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
