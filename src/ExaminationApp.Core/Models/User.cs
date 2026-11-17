using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(255)]
    public string? LastName { get; set; }

    [StringLength(255)]
    public string? FirstName { get; set; }

    [StringLength(255)]
    public string? Fathername { get; set; }

    [StringLength(255)]
    public string? Login { get; set; }

    [Column("RoleID")]
    public int RoleId { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<OrderHeader> OrderHeaders { get; set; } = new List<OrderHeader>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;
}
