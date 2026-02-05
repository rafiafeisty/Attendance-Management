using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("StudentTeacherCourse")]
[Index("StId", "Cid", "TecId", Name = "UQ_STC", IsUnique = true)]
public partial class StudentTeacherCourse
{
    [Key]
    [Column("stc_id")]
    public int StcId { get; set; }

    [Column("st_id")]
    public int StId { get; set; }

    [Column("cid")]
    public int Cid { get; set; }

    [Column("tec_id")]
    public int TecId { get; set; }

    [InverseProperty("Stc")]
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    [ForeignKey("Cid")]
    [InverseProperty("StudentTeacherCourses")]
    public virtual Course CidNavigation { get; set; } = null!;

    [ForeignKey("StId")]
    [InverseProperty("StudentTeacherCourses")]
    public virtual Student St { get; set; } = null!;

    [ForeignKey("TecId")]
    [InverseProperty("StudentTeacherCourses")]
    public virtual Teacher Tec { get; set; } = null!;
}
