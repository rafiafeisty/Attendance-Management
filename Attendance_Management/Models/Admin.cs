using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Admin")]
[Index("Username", Name = "UQ__Admin__F3DBC572CF32F9ED", IsUnique = true)]
public partial class Admin
{
    [Key]
    [Column("ad_id")]
    public int AdId { get; set; }

    [Column("ad_name")]
    [StringLength(50)]
    public string AdName { get; set; } = null!;

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(50)]
    public string Password { get; set; } = null!;
}
