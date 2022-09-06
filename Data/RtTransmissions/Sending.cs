using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransmissions
{
    public class Sending
    {
        [XmlElement("DatiCorrispettivi", Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/corrispettivi/dati/v1.0")]
        public AmountsData AmountsData { get; set; }

        [XmlElement("EsitoOperazione", Namespace = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/corrispettivi/v1.0")]
        public OperationOutcome OperationOutcome { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
