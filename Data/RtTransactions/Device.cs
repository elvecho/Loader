using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class Device
    {
        [XmlElement("Marchio")]
        public string Brand { get; set; }

        [XmlElement("Modello")]
        public string Model { get; set; }

        [XmlElement("Matricola")]
        public string SerialNumber { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
