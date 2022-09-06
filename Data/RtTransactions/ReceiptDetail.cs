using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class ReceiptDetail
    {
        [XmlElement("Vendita")]
        public ItemSale ItemSale { get; set; }

        [XmlElement("ModificatoreSuArticolo")]
        public ItemModifier ItemModifier { get; set; }

        [XmlElement("Pagamento")]
        public Tender Tender { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
