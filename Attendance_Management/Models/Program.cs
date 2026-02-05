using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Program")]
public partial class Program
{
    [Key]
    [Column("pid")]
    public int Pid { get; set; }

    [Column("pname")]
    [StringLength(50)]
    public string Pname { get; set; } = null!;

    [InverseProperty("PidNavigation")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
