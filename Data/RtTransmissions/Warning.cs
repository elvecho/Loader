using System;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class Warning
    {
        [XmlElement("Matricola")]
        public string SerialCode { get; set; }

        [XmlElement("Codice")]
        public string Code { get; set; }

        [XmlElement("DataOra")]
        public DateTime Date { get; set; }

        [XmlElement("Note")]
        public string Notes { get; set; }

    }
}
