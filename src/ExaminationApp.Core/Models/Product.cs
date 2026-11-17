using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

public partial class Product
{
    [Key]
    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [Column("SubcategoryID")]
    public int SubcategoryId { get; set; }

    [StringLength(255)]
    public string? Image { get; set; }

    [StringLength(255)]
    public string? ProductName { get; set; }

    [Column("FactoryID")]
    public int FactoryId { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? Details { get; set; }

    [Column(TypeName = "money")]
    public decimal? Price { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ProductCategories")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("FactoryId")]
    [InverseProperty("Products")]
    public virtual Factory Factory { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Product")]
    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();

    [ForeignKey("SubcategoryId")]
    [InverseProperty("ProductSubcategories")]
    public virtual Category Subcategory { get; set; } = null!;
}
