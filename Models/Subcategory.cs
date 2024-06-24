using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Subcategory
{
    public int SubcategoryId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<CategoryParameter> CategoryParameters { get; set; } = new List<CategoryParameter>();

    public virtual ICollection<Good> Goods { get; set; } = new List<Good>();
}
