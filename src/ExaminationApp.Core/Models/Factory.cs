using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

[Table("Factory")]
public partial class Factory
{
    [Key]
    [Column("FactoryID")]
    public int FactoryId { get; set; }

    [StringLength(255)]
    public string? FactoryName { get; set; }

    [InverseProperty("Factory")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Factory")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("Factory")]
    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
