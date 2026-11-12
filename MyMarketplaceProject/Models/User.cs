namespace MyMarketplaceProject.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Patronum { get; set; }

    public string Surname { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public sbyte IsModerator { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Seller> Sellers { get; set; } = new List<Seller>();
}
