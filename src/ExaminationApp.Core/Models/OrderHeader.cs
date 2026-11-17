using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

public partial class OrderHeader
{
    [Key]
    public int OrderNumber { get; set; }

    public DateTime OrderDate { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [InverseProperty("OrderNumberNavigation")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("UserId")]
    [InverseProperty("OrderHeaders")]
    public virtual User User { get; set; } = null!;
}
