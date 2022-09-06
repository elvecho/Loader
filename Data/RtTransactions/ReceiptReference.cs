using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class ReceiptReference
    {
        [XmlElement("DataRegistrazione")]
        public string Date { get; set; }

        [XmlElement("OrarioRegistrazione")]
        public string Time { get; set; }

        [XmlElement("NumeroProgressivo")]
        public string Sequence { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
