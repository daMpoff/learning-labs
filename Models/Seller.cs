using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Seller
{
    public int SellerId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();

    public virtual User User { get; set; } = null!;
}
