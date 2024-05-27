using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Good
{
    public int GoodId { get; set; }

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public int ShopId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Shop GoodNavigation { get; set; } = null!;

    public virtual ICollection<GoodParameter> GoodParameters { get; set; } = new List<GoodParameter>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
