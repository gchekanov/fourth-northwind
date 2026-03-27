using NorthwindDemo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NorthwindDemo.Domain.Entities;

namespace NorthwindDemo.Infrastructure.Data
{
    public class NorthwindDbContext : DbContext
    {

        public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options)
    : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(c => c.CustomerId);
                entity.Property(c => c.CustomerId).HasMaxLength(5).IsRequired();
                entity.HasMany(c => c.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.OrderId);
                entity.HasMany(o => o.OrderDetails)
                      .WithOne(od => od.Order)
                      .HasForeignKey(od => od.OrderId);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("Order Details");
                entity.HasKey(od => new { od.OrderId, od.ProductId });
                entity.HasOne(od => od.Product)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(od => od.ProductId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.ProductId);
                entity.HasMany(p => p.OrderDetails)
                      .WithOne(od => od.Product)
                      .HasForeignKey(od => od.ProductId);
            });
        }
    }
}
