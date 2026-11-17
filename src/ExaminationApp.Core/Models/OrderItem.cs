using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

public partial class OrderItem
{
    [Key]
    [Column("OrderRowID")]
    public int OrderRowId { get; set; }

    public int OrderNumber { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }

    [Column("SizeID")]
    public int SizeId { get; set; }

    [Column("FactoryID")]
    public int FactoryId { get; set; }

    [Column("CategoryID")]
    public int CategoryId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("OrderItems")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("FactoryId")]
    [InverseProperty("OrderItems")]
    public virtual Factory Factory { get; set; } = null!;

    [ForeignKey("OrderNumber")]
    [InverseProperty("OrderItems")]
    public virtual OrderHeader OrderNumberNavigation { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderItems")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("SizeId")]
    [InverseProperty("OrderItems")]
    public virtual Size Size { get; set; } = null!;
}
