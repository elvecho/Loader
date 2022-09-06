using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    [XmlRoot("RestituzioneTabellaIva")]
    public class VatTableResponse
    {
        [XmlElement("ElementoIva")]
        public List<VatInfo> VatInfos { get; set; }

        public VatTableResponse()
        {
            VatInfos = new List<VatInfo>();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
