using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class Device
    {
        [XmlElement("IdDispositivo")]
        public string Id { get; set; }

        [XmlElement("Tipo")]
        public string Type { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
