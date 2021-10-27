﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TaxiServiceBD.Models
{
    public partial class Order
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? DriverId { get; set; }
        public int? CategoryClassId { get; set; }
        public DateTime? DateOfCreation { get; set; }

        public virtual CategoriesClassDetail CategoryClass { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual User User { get; set; }
    }
}
