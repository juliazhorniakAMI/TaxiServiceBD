using System;
using System.Collections.Generic;

#nullable disable

namespace TaxiServiceBD.Models
{
    public partial class Driver
    {
        public Driver()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int? Rate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
