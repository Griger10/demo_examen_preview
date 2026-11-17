using System;
using System.Collections.Generic;
using ExaminationApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Factory> Factories { get; set; }

    public virtual DbSet<OrderHeader> OrderHeaders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<StockItem> StockItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-62AA05E\\SQLEXPRESS;Database=DemoExPrDB;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_100_CI_AI_KS_SC_UTF8");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CategoryId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Factory>(entity =>
        {
            entity.Property(e => e.FactoryId).ValueGeneratedNever();
        });

        modelBuilder.Entity<OrderHeader>(entity =>
        {
            entity.HasKey(e => e.OrderNumber).HasName("PK__OrderHea__CAC5E7422A26DE2D");

            entity.Property(e => e.OrderNumber).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.OrderHeaders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderHead__UserI__49C3F6B7");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderRowId).HasName("PK__OrderIte__4D7AD202ADB6B19D");

            entity.Property(e => e.OrderRowId).ValueGeneratedNever();

            entity.HasOne(d => d.Category).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Categ__5070F446");

            entity.HasOne(d => d.Factory).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Facto__4F7CD00D");

            entity.HasOne(d => d.OrderNumberNavigation).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__4CA06362");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__4D94879B");

            entity.HasOne(d => d.Size).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__SizeI__4E88ABD4");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Factory).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Factory");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.ProductSubcategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.Property(e => e.SizeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.Property(e => e.StockId).ValueGeneratedNever();

            entity.HasOne(d => d.Factory).WithMany(p => p.StockItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stock_Items_Factory");

            entity.HasOne(d => d.Product).WithMany(p => p.StockItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stock_Items_Products");

            entity.HasOne(d => d.Size).WithMany(p => p.StockItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Stock_Items_Sizes");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
