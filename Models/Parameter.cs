using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Parameter
{
    public int ParameterId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<CategoryParameter> CategoryParameters { get; set; } = new List<CategoryParameter>();

    public virtual ICollection<GoodParameter> GoodParameters { get; set; } = new List<GoodParameter>();
}
