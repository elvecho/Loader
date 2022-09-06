using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class Document
    {
        [XmlElement("Data")]
        public string Date { get; set; }

        [XmlElement("Ora")]
        public string Time { get; set; }

        [XmlElement("NumeroAzzeramento")]
        public int ZNumber { get; set; }

        [XmlElement("NumeroDocumento")]
        public int DocumentNumber { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
