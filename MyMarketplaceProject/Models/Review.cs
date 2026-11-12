using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

/// <summary>
/// 			
/// </summary>
public partial class Review
{
    public int ReviewId { get; set; }

    public string Comment { get; set; } = null!;

    public int GoodId { get; set; }

    public int CustomerId { get; set; }

    public bool Rating { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Good Good { get; set; } = null!;

    public virtual ICollection<ReviewPhoto> ReviewPhotos { get; set; } = new List<ReviewPhoto>();
}
