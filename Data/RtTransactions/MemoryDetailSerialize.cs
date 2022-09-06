using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    [XmlRoot("RestituzioneMemDettaglio")]

    public class MemoryDetailSerialize
    {
        [XmlElement("ProgressivoPacchetto")]
        public int PackageSequenceNumber { get; set; }

        [XmlElement("DataRichiesta")]
        public string RequestDate { get; set; }

        [XmlElement("IdentificativoDGFE")]
        public string DGFEId { get; set; }

        [XmlElement("PuntoCassa")]
        public string DeviceId { get; set; }

        [XmlElement("Sessione")]
        public string Session { get; set; }
        /*Editor:SoukainaInformation 
        *Edit: added RtServer information Object used by Publisher
         */
        [XmlElement("RecordImmatricolazione")]
        public RecordMatriculation RecordMatriculation { get; set; }
        /*End changes*/

        [XmlElement("DocumentoCommerciale")]
        public List<SalesDocumentSerialize> SalesDocuments { get; set; }

        [XmlElement("RichiestaMemDettaglio")]
        public MemoryDetailRequest MemoryDetailRequest { get; set; }

        public MemoryDetailSerialize()
        {
            SalesDocuments = new List<SalesDocumentSerialize>();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
