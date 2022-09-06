using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    [XmlRoot("RichiestaTabellaIva")]
    public class VatTableRequest
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
