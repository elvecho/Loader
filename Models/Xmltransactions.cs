using System;

namespace Loader.Models
{
    public class Xmltransactions
    {
        public int Id { get; set; }
        public string BodyTransavtion { get; set; }
        public DateTime? LoadedDateTime { get; set; }
        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public DateTime? DtBusinessDate { get; set; }
        public int? LPosWorkstationNmbr { get; set; }
        public int? LPosTaNmbr { get; set; }
    }
}
