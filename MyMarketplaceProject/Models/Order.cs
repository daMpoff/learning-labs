using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int GoodId { get; set; }

    public decimal Price { get; set; }

    public int CustomerId { get; set; }

    public int OrderType { get; set; }

    public DateOnly Date { get; set; }

    public virtual Good Good { get; set; } = null!;

    public virtual OrderType OrderTypeNavigation { get; set; } = null!;
}
