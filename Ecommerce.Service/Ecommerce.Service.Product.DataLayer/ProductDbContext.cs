using Microsoft.EntityFrameworkCore;
using Ecommerce.Common.Entities;
using Ecommerce.Service.Product.Domain.Entities;

namespace Ecommerce.Service.Product.DataLayer
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Ecommerce.Service.Product.Domain.Entities.Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<SubProductCategory> SubProductCategories { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product Configuration
            modelBuilder.Entity<Ecommerce.Service.Product.Domain.Entities.Product>(entity =>
            {
                entity.ToTable("Product");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.SubCategory)
                      .WithMany(p => p.Products)
                      .HasForeignKey(d => d.SubCategoryId);

                entity.HasOne(d => d.Gender)
                      .WithMany(g => g.Products)
                      .HasForeignKey(d => d.GenderId);
            });

            // ProductCategory Configuration
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            });

            // SubProductCategory Configuration
            modelBuilder.Entity<SubProductCategory>(entity =>
            {
                entity.ToTable("SubProductCategory");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SubCategory).IsRequired().HasMaxLength(100);

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.SubCategories)
                      .HasForeignKey(d => d.CategoryId);
            });

            // Gender Configuration
            modelBuilder.Entity<Gender>(entity =>
            {
                entity.ToTable("Gender");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            // Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(d => d.ProductId);
            });

            // Transaction Configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Order)
                      .WithOne(o => o.Transaction)
                      .HasForeignKey<Transaction>(d => d.OrderId);
            });

            // Log Configuration
            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LogLevel).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Message).HasMaxLength(500);
                entity.Property(e => e.TableName).HasMaxLength(100);
            });
        }
    }
}
