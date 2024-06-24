using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class GoodParameter
{
    public int GoodParameterId { get; set; }

    public int GoodId { get; set; }

    public int ParameterId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Good Good { get; set; } = null!;

    public virtual Parameter Parameter { get; set; } = null!;
}
