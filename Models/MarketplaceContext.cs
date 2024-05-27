using Microsoft.EntityFrameworkCore;

namespace MyMarketplaceProject.Models;

public partial class MarketplaceContext : DbContext
{
    public MarketplaceContext()
    {
    }

    public MarketplaceContext(DbContextOptions<MarketplaceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryParameter> CategoryParameters { get; set; }

    public virtual DbSet<Good> Goods { get; set; }

    public virtual DbSet<GoodParameter> GoodParameters { get; set; }

    public virtual DbSet<Parameter> Parameters { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=marketplace;uid=root;pwd=[REDACTED]", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.36-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PRIMARY");

            entity.ToTable("account");

            entity.HasIndex(e => e.UserId, "account_user_idx");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Login)
                .HasMaxLength(45)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("account_user");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("category");

            entity.HasIndex(e => e.CategoryId, "categoryId_UNIQUE").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<CategoryParameter>(entity =>
        {
            entity.HasKey(e => e.CategoryParameterId).HasName("PRIMARY");

            entity.ToTable("category_parameter");

            entity.HasIndex(e => e.ParameterId, "ParameterId_idx");

            entity.HasIndex(e => e.CategoryParameterId, "category_parameter_id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.SubcategoryId, "subcategory_id_idx");

            entity.Property(e => e.CategoryParameterId).HasColumnName("category_parameter_id");
            entity.Property(e => e.ParameterId).HasColumnName("parameter_id");
            entity.Property(e => e.SubcategoryId).HasColumnName("subcategory_id");

            entity.HasOne(d => d.Parameter).WithMany(p => p.CategoryParameters)
                .HasForeignKey(d => d.ParameterId)
                .HasConstraintName("ParameterId");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.CategoryParameters)
                .HasForeignKey(d => d.SubcategoryId)
                .HasConstraintName("subcategory_id");
        });

        modelBuilder.Entity<Good>(entity =>
        {
            entity.HasKey(e => e.GoodId).HasName("PRIMARY");

            entity.ToTable("good");

            entity.HasIndex(e => e.CategoryId, "CategoryId_idx");

            entity.HasIndex(e => e.GoodId, "goodId_UNIQUE").IsUnique();

            entity.Property(e => e.GoodId)
                .ValueGeneratedOnAdd()
                .HasColumnName("good_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Price)
                .HasPrecision(10)
                .HasColumnName("price");
            entity.Property(e => e.ShopId).HasColumnName("shop_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Goods)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("CategoryGoodId");

            entity.HasOne(d => d.GoodNavigation).WithOne(p => p.Good)
                .HasForeignKey<Good>(d => d.GoodId)
                .HasConstraintName("ShopGood");
        });

        modelBuilder.Entity<GoodParameter>(entity =>
        {
            entity.HasKey(e => e.GoodParameterId).HasName("PRIMARY");

            entity.ToTable("good_parameter");

            entity.HasIndex(e => e.GoodId, "GoodId_idx");

            entity.HasIndex(e => e.ParameterId, "ParametrId_idx");

            entity.HasIndex(e => e.GoodParameterId, "good_parameter_id_UNIQUE").IsUnique();

            entity.Property(e => e.GoodParameterId).HasColumnName("good_parameter_id");
            entity.Property(e => e.GoodId).HasColumnName("good_id");
            entity.Property(e => e.ParameterId).HasColumnName("parameter_id");
            entity.Property(e => e.Value)
                .HasMaxLength(45)
                .HasColumnName("value");

            entity.HasOne(d => d.Good).WithMany(p => p.GoodParameters)
                .HasForeignKey(d => d.GoodId)
                .HasConstraintName("GoodId");

            entity.HasOne(d => d.Parameter).WithMany(p => p.GoodParameters)
                .HasForeignKey(d => d.ParameterId)
                .HasConstraintName("ParametrId");
        });

        modelBuilder.Entity<Parameter>(entity =>
        {
            entity.HasKey(e => e.ParameterId).HasName("PRIMARY");

            entity.ToTable("parameter");

            entity.HasIndex(e => e.ParameterId, "parameterid_UNIQUE").IsUnique();

            entity.Property(e => e.ParameterId).HasColumnName("parameter_id");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PRIMARY");

            entity.ToTable("review");

            entity.HasIndex(e => e.GoodId, "review_good_idx");

            entity.HasIndex(e => e.UserId, "review_user_idx");

            entity.Property(e => e.ReviewId)
                .ValueGeneratedNever()
                .HasColumnName("review_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .HasColumnName("comment");
            entity.Property(e => e.GoodId).HasColumnName("good_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Good).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("review_good");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("review_user");
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.ShopId).HasName("PRIMARY");

            entity.ToTable("shop");

            entity.HasIndex(e => e.UserId, "UserShop_idx");

            entity.Property(e => e.ShopId).HasColumnName("shop_id");
            entity.Property(e => e.Description)
                .HasMaxLength(45)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Shops)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserShop");
        });

        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.HasKey(e => e.SubcategoryId).HasName("PRIMARY");

            entity.ToTable("subcategory");

            entity.HasIndex(e => e.CategoryId, "category_idx");

            entity.Property(e => e.SubcategoryId).HasColumnName("subcategory_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.Category).WithMany(p => p.Subcategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("category");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
