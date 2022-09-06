using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    [XmlRoot("RichiestaErroriCassa")]
    public class DeviceErrorsRequest
    {
        [XmlElement("Data")]
        public string Date { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
