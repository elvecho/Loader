using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtStatus
{
    [XmlRoot("RichiestaStatoServer")]
    public class ServerStatusRequest
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
