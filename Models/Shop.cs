using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Shop
{
    public int ShopId { get; set; }

    public int SellerId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual Good? Good { get; set; }

    public virtual Seller Seller { get; set; } = null!;
}
