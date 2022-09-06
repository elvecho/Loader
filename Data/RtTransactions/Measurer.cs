using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    [XmlRoot("Misuratore")]
    public class Measurer
    {

        [XmlElement("Marchio")]
        public string Mark { get; set; }

        [XmlElement("Modello")]
        public string Model { get; set; }
        [XmlElement("Matricola")]
        public string ServerRT { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}