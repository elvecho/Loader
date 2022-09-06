using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace Loader.Data.RtTransactions
{
    [XmlType(TypeName = "SalesDocument")]

    public class SalesDocumentSerialize
    {
        [XmlElement("CCDCPrecedente")]
        public string PreviousCCDC { get; set; }

        [XmlElement("PuntoCassa")]
        public string RegisterId { get; set; }
        /*Editor:SoukainaInformation 
        *Edit: added fields for publisher
         */
        [XmlElement("IdentificativoDGFE")]
       
        public string IdentificativoDGFE { get; set; }
        [XmlElement("Cassa")]
       
        public string Cassa { get; set; }
        [XmlElement("Marchio")]

        public string Marchio { get; set; }
        [XmlElement("Modello")]

        public string Modello { get; set; }
        [XmlElement("Matricola")]

        public string Matricola { get; set; }
        /*End changes*/
        [XmlElement("Scontrino")]
        public Receipt Receipt { get; set; }

        [XmlElement("RettificaScontrino")]
        public CanceledReceipt CanceledReceipt { get; set; }

        [XmlElement("CCDC")]
        public string CCDC { get; set; }

        [XmlElement("NonConforme")]
        public int NonCompliant { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
