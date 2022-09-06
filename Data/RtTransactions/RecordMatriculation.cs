using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    [XmlRoot("RecordImmatricolazione")]

    public class RecordMatriculation
    {
       

        [XmlElement("Misuratore")]
        public Measurer Measurer { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}