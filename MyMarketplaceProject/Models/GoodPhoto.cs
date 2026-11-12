using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class GoodPhoto
{
    public int PhotoId { get; set; }

    public int GoodId { get; set; }

    public string Url { get; set; } = null!;

    public virtual Good Good { get; set; } = null!;
}
