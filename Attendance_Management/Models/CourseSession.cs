using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("CourseSession")]
public partial class CourseSession
{
    [Key]
    [Column("cs_id")]
    public int CsId { get; set; }

    [Column("cid")]
    public int Cid { get; set; }

    [Column("sid")]
    public int Sid { get; set; }

    [Column("tec_id")]
    public int TecId { get; set; }

    [ForeignKey("Cid")]
    [InverseProperty("CourseSessions")]
    public virtual Course CidNavigation { get; set; } = null!;

    [ForeignKey("Sid")]
    [InverseProperty("CourseSessions")]
    public virtual Session SidNavigation { get; set; } = null!;

    [ForeignKey("TecId")]
    [InverseProperty("CourseSessions")]
    public virtual Teacher Tec { get; set; } = null!;
}
