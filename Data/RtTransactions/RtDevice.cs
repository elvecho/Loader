using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class RtDevice
    {
        [XmlElement("PuntoCassa")]
        public string DeviceId { get; set; }

        [XmlElement("CassaAperta")]
        public bool Open { get; set; }

        [XmlElement("Simulazione")]
        public bool Simulate { get; set; }

        [XmlElement("Ventilazione")] 
        public bool Spread { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
