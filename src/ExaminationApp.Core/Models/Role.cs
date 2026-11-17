using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationApp.Core.Models;

public partial class Role
{
    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [Column("Role")]
    [StringLength(255)]
    public string? Role1 { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
