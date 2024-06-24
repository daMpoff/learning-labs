using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Good
{
    public int GoodId { get; set; }

    public int SubcategoryId { get; set; }

    public decimal Price { get; set; }

    public int ShopId { get; set; }

    public int Quantity { get; set; }

    public sbyte IsModerate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Shop GoodNavigation { get; set; } = null!;

    public virtual ICollection<GoodParameter> GoodParameters { get; set; } = new List<GoodParameter>();

    public virtual ICollection<GoodPhoto> GoodPhotos { get; set; } = new List<GoodPhoto>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Subcategory Subcategory { get; set; } = null!;
}
