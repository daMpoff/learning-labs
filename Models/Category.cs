using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();

    public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}
