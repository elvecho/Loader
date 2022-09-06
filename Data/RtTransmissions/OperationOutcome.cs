using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class OperationOutcome
    {
        [XmlElement("IdOperazione", Namespace = "")]
        public string OperationId { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
