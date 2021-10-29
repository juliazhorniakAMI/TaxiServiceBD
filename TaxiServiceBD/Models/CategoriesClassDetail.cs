using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace TaxiServiceBD.Models
{
    public partial class CategoriesClassDetail
    {
        public CategoriesClassDetail()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int TaxiClassId { get; set; }
        public bool IsActive { get; set; }

        public virtual Category Category { get; set; }
        public virtual TaxiClass TaxiClass { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }
        [NotMapped]
        public string TaxiName { get; set; }

    }

}
