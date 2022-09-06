using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtStatus
{
    [XmlRoot("RestituzioneStatoServer")]
    public class RtStatus
    {
        [XmlElement("UltimaChiusura")]
        public short LastClosureNumber { get; set; }

        [XmlElement("GranTotale")]
        public long GrandTotal { get; set; }

        [XmlElement("MemoriaDettaglio")]
        public short DetailMemory { get; set; }

        [XmlElement("TrasmissioniPendenti")]
        public short PendingTransmissions { get; set; }

        [XmlElement("GiorniPendenti")]
        public short PendingDays { get; set; }

        [XmlElement("TrasmissioneInCorso")]
        public bool ActiveTransmission { get; set; }

        [XmlElement("SchedulazioneChiusure")]
        public RtClosureSchedule ClosureSchedule { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
