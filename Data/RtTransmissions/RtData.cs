using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class RtData
    {
        [XmlElement("Riepilogo")]
        public List<Summary> Summaries { get; set; }

        public RtData()
        {
            Summaries = new List<Summary>();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
