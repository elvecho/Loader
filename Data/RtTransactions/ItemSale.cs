using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    public class ItemSale
    {
        [XmlIgnore] private CultureInfo CultureInfo { get; set; } = CultureInfo.CreateSpecificCulture("it-IT");

        [XmlElement("Descrizione")]
        public string Description { get; set; }

        [XmlIgnore]
        public decimal Amount { get; set; }

        [XmlElement("Importo")]
        public string AmountFormatted
        {
            get => Amount.ToString(CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var amount);
                Amount = amount; 
            }
        }

        [XmlIgnore]
        public decimal Quantity { get; set; }

        [XmlElement("Quantita")]
        public string QuantityFormatted
        {
            get => Quantity.ToString(CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var quantity);
                Quantity = quantity;
            }
        }

        [XmlIgnore]
        public decimal Price { get; set; }

        [XmlElement("PrezzoUnitario")]
        public string PriceFormatted
        {
            get => Price.ToString(CultureInfo);
            set {
                decimal.TryParse(value, NumberStyles.Any, CultureInfo, out var price);
                Price = price;
            }
        }

        [XmlElement("CodiceIVA")]
        public VatDetail VatCode { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
