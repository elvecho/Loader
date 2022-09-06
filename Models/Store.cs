using System;
using System.Collections.Generic;

namespace Loader.Models
{
    public sealed class Store
    {
        public Store()
        {
            RtServer = new HashSet<RtServer>();
        }

        public int LRetailStoreId { get; set; }
        public int LStoreGroupId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public StoreGroup LStoreGroup { get; set; }
        public ICollection<RtServer> RtServer { get; set; }
    }
}
