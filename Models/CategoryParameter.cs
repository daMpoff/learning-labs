using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class CategoryParameter
{
    public int CategoryParameterId { get; set; }

    public int SubcategoryId { get; set; }

    public int ParameterId { get; set; }

    public virtual Parameter Parameter { get; set; } = null!;

    public virtual Subcategory Subcategory { get; set; } = null!;
}
