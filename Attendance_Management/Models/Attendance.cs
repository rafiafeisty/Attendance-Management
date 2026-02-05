using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Attendance_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Attendance")]
public partial class Attendance
{
    [Key]
    [Column("at_id")]
    public int AtId { get; set; }

    [Column("stc_id")]
    public int StcId { get; set; }

    [Column("att_date")]
    public DateOnly AttDate { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [Column("start_time")]
    public TimeOnly StartTime { get; set; }

    [Column("end_time")]
    public TimeOnly EndTime { get; set; }

    [ForeignKey("StcId")]
    [InverseProperty("Attendances")]
    public virtual StudentTeacherCourse Stc { get; set; } = null!;
}
