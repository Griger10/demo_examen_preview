using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

public partial class Category
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(255)]
    public string? CategoryName { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Category")]
    public virtual ICollection<Product> ProductCategories { get; set; } = new List<Product>();

    [InverseProperty("Subcategory")]
    public virtual ICollection<Product> ProductSubcategories { get; set; } = new List<Product>();
}
