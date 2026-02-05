using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Section")]
public partial class Section
{
    [Key]
    [Column("sec_id")]
    public int SecId { get; set; }

    [Column("sec_name")]
    [StringLength(50)]
    public string SecName { get; set; } = null!;

    [Column("sid")]
    public int Sid { get; set; }

    [ForeignKey("Sid")]
    [InverseProperty("Sections")]
    public virtual Session SidNavigation { get; set; } = null!;

    [InverseProperty("Sec")]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    [InverseProperty("Sec")]
    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
