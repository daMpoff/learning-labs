using System;
using System.Collections.Generic;

namespace MyMarketplaceProject.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
