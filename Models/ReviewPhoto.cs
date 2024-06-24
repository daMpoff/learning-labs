using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class ReviewPhoto
{
    public int PhotoId { get; set; }

    public int ReviewId { get; set; }

    public string Url { get; set; } = null!;

    public virtual Review Review { get; set; } = null!;
}
