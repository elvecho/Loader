using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtStatus
{
    public class RtClosureSchedule
    {
        [XmlElement("TraMinuti")]
        public short InMinutes { get; set; }

        [XmlElement("RipetiOgniOre")]
        public short RepeatEveryHours { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
