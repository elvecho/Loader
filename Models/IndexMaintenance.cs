using System;

namespace Loader.Models
{
    public class IndexMaintenance
    {
        public DateTime DDate { get; set; }
        public string SzDbName { get; set; }
        public string SzSchemaName { get; set; }
        public string SzTableName { get; set; }
        public string SzIndexName { get; set; }
        public double? LPrePercFragmentation { get; set; }
        public string SzActionToDo { get; set; }
        public double? LPostPercFragmentation { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }
    }
}
