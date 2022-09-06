using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    [XmlRoot("RestituzioneErroriCassa")]
    public class DeviceErrorsResponse
    {
        [XmlElement("ProgressivoPacchetto")]
        public int SequenceNumber { get; set; }

        [XmlElement("Misuratore")]
        public Device Device { get; set; }

        [XmlElement("Errore")]
        public List<Error> Errors { get; set; }

        public DeviceErrorsResponse()
        {
            Errors = new List<Error>();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
