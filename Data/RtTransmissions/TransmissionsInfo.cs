using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    [XmlRoot("RestituzioneTrasmissioni")]
    public class TransmissionsInfo
    {
        [XmlElement("Invio")]
        public List<Sending> Sendings { get; set; }

        public TransmissionsInfo()
        {
            Sendings = new List<Sending>();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
