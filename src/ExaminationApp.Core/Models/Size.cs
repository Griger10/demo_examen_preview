using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

public partial class Size
{
    [Key]
    [Column("SizeID")]
    public int SizeId { get; set; }

    [Column("Size", TypeName = "decimal(3, 1)")]
    public decimal? Size1 { get; set; }

    [InverseProperty("Size")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Size")]
    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
