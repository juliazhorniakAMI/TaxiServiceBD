using System;
using System.Collections.Generic;

#nullable disable

namespace TaxiServiceBD.Models
{
    public partial class TaxiClass
    {
        public TaxiClass()
        {
            CategoriesClassDetails = new HashSet<CategoriesClassDetail>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }

        public virtual ICollection<CategoriesClassDetail> CategoriesClassDetails { get; set; }
    }
}
