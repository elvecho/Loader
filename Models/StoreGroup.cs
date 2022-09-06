using System;
using System.Collections.Generic;

namespace Loader.Models
{
    public sealed class StoreGroup
    {
        public StoreGroup()
        {
            Store = new HashSet<Store>();
        }

        public int LStoreGroupId { get; set; }
        public string SzDescription { get; set; }
        public DateTime? DLastUpdateLocal { get; set; }

        public ICollection<Store> Store { get; set; }
    }
}
