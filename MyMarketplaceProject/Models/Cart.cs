using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int GoodId { get; set; }

    public int Quantity { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Good Good { get; set; } = null!;
}
