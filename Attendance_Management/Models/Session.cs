using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Session")]
public partial class Session
{
    [Key]
    [Column("sid")]
    public int Sid { get; set; }

    [Column("s_name")]
    [StringLength(50)]
    public string SName { get; set; } = null!;

    [Column("pid")]
    public int Pid { get; set; }

    [InverseProperty("SidNavigation")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [ForeignKey("Pid")]
    [InverseProperty("Sessions")]
    public virtual Program PidNavigation { get; set; } = null!;

    [InverseProperty("SidNavigation")]
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
}
