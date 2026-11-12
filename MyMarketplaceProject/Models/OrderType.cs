using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class OrderType
{
    public int OrderTypeId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
