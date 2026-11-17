using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

[Table("Stock_Items")]
public partial class StockItem
{
    [Key]
    [Column("StockID")]
    public int StockId { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column("FactoryID")]
    public int FactoryId { get; set; }

    [Column("SizeID")]
    public int SizeId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("FactoryId")]
    [InverseProperty("StockItems")]
    public virtual Factory Factory { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("StockItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("SizeId")]
    [InverseProperty("StockItems")]
    public virtual Size Size { get; set; } = null!;
}
