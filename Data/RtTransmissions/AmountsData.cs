using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class AmountsData
    {
        [XmlElement("Trasmissione", Namespace = "")]
        public Transmission Transmission { get; set; }

        [XmlElement("DataOraRilevazione", Namespace = "")]
        public DateTime CollectionDate { get; set; }

        [XmlElement("DatiRT", Namespace = "")]
        public RtData RtData { get; set; }

        [XmlElement("Segnalazione", Namespace = "")]
        public List<Warning> Warnings { get; set; }

        public AmountsData()
        {
            Warnings = new List<Warning>();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
