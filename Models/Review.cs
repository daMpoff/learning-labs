using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public string Comment { get; set; } = null!;

    public int GoodId { get; set; }

    public virtual Good Good { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
