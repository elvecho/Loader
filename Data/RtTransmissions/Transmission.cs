using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class Transmission
    {
        [XmlElement("Dispositivo")]
        public Device Device { get; set; }

        [XmlElement("Formato")]
        public string Format { get; set; }

        [XmlElement("Progressivo")]
        public int Sequence { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
