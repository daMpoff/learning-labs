using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

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

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryParameter> CategoryParameters { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Good> Goods { get; set; }

    public virtual DbSet<GoodParameter> GoodParameters { get; set; }

    public virtual DbSet<GoodPhoto> GoodPhotos { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderType> OrderTypes { get; set; }

    public virtual DbSet<Parameter> Parameters { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewPhoto> ReviewPhotos { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=127.0.0.1;port=3306;database=marketplace;uid=root;pwd=[REDACTED]", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.35-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("account");

            entity.HasIndex(e => e.UserId, "account_user_idx");

            entity.HasIndex(e => e.Login, "login_UNIQUE").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.VerificationCode).HasColumnName("verificationCode");

            entity.HasOne(d => d.User).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.UserId)
                .HasConstraintName("account_user");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PRIMARY");

            entity.ToTable("cart");

            entity.HasIndex(e => e.CustomerId, "cart_customer_idx");

            entity.HasIndex(e => e.GoodId, "cart_good_idx");

            entity.Property(e => e.CartId)
                .ValueGeneratedNever()
                .HasColumnName("cart_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.GoodId).HasColumnName("good_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_customer");

            entity.HasOne(d => d.Good).WithMany(p => p.Carts)
                .HasForeignKey(d => d.GoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_good");
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

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.ToTable("customer");

            entity.HasIndex(e => e.UserId, "customer_user_idx");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Adress)
                .HasMaxLength(255)
                .HasColumnName("adress");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Customers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("customer_user");
        });

        modelBuilder.Entity<Good>(entity =>
        {
            entity.HasKey(e => e.GoodId).HasName("PRIMARY");

            entity.ToTable("good");

            entity.HasIndex(e => e.GoodId, "goodId_UNIQUE").IsUnique();

            entity.HasIndex(e => e.SubcategoryId, "subcategoryGoodId_idx");

            entity.Property(e => e.GoodId)
                .ValueGeneratedOnAdd()
                .HasColumnName("good_id");
            entity.Property(e => e.IsModerate).HasColumnName("isModerate");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ShopId).HasColumnName("shop_id");
            entity.Property(e => e.SubcategoryId).HasColumnName("subcategory_id");

            entity.HasOne(d => d.GoodNavigation).WithOne(p => p.Good)
                .HasForeignKey<Good>(d => d.GoodId)
                .HasConstraintName("ShopGood");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Goods)
                .HasForeignKey(d => d.SubcategoryId)
                .HasConstraintName("subcategoryGoodId");
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

        modelBuilder.Entity<GoodPhoto>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PRIMARY");

            entity.ToTable("good_photo");

            entity.HasIndex(e => e.GoodId, "photo_good_idx");

            entity.Property(e => e.PhotoId)
                .ValueGeneratedNever()
                .HasColumnName("photo_id");
            entity.Property(e => e.GoodId).HasColumnName("good_id");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.Good).WithMany(p => p.GoodPhotos)
                .HasForeignKey(d => d.GoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("photo_good");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("order");

            entity.HasIndex(e => e.GoodId, "order_good_idx");

            entity.HasIndex(e => e.OrderType, "order_type_idx");

            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("order_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.GoodId).HasColumnName("good_id");
            entity.Property(e => e.OrderType).HasColumnName("order_type");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");

            entity.HasOne(d => d.Good).WithMany(p => p.Orders)
                .HasForeignKey(d => d.GoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_good");

            entity.HasOne(d => d.OrderTypeNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_type");
        });

        modelBuilder.Entity<OrderType>(entity =>
        {
            entity.HasKey(e => e.OrderTypeId).HasName("PRIMARY");

            entity.ToTable("order_type");

            entity.Property(e => e.OrderTypeId)
                .ValueGeneratedNever()
                .HasColumnName("order_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
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

            entity.ToTable("review", tb => tb.HasComment("			"));

            entity.HasIndex(e => e.CustomerId, "review_customer_idx");

            entity.HasIndex(e => e.GoodId, "review_good_idx");

            entity.Property(e => e.ReviewId)
                .ValueGeneratedNever()
                .HasColumnName("review_id");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .HasColumnName("comment");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.GoodId).HasColumnName("good_id");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("review_customer");

            entity.HasOne(d => d.Good).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.GoodId)
                .HasConstraintName("review_good");
        });

        modelBuilder.Entity<ReviewPhoto>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PRIMARY");

            entity.ToTable("review_photo");

            entity.HasIndex(e => e.ReviewId, "photo_review_idx");

            entity.Property(e => e.PhotoId)
                .ValueGeneratedNever()
                .HasColumnName("photo_id");
            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewPhotos)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("photo_review");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("PRIMARY");

            entity.ToTable("seller");

            entity.HasIndex(e => e.UserId, "seller_user_idx");

            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Sellers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("seller_user");
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.ShopId).HasName("PRIMARY");

            entity.ToTable("shop");

            entity.HasIndex(e => e.SellerId, "shop_seller_fk_idx");

            entity.Property(e => e.ShopId).HasColumnName("shop_id");
            entity.Property(e => e.Description)
                .HasMaxLength(45)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");

            entity.HasOne(d => d.Seller).WithMany(p => p.Shops)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("shop_seller_fk");
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
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.IsModerator).HasColumnName("isModerator");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Patronum)
                .HasMaxLength(255)
                .HasColumnName("patronum");
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
