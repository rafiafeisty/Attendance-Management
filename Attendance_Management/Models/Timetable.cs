using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Timetable")]
public partial class Timetable
{
    [Key]
    [Column("time_id")]
    public int TimeId { get; set; }

    [Column("sec_id")]
    public int SecId { get; set; }

    [Column("cid")]
    public int Cid { get; set; }

    [Column("tec_id")]
    public int TecId { get; set; }

    [Column("start_time")]
    public TimeOnly StartTime { get; set; }

    [Column("end_time")]
    public TimeOnly EndTime { get; set; }

    [Column("days_of_week")]
    [StringLength(20)]
    public string DaysOfWeek { get; set; } = null!;

    [ForeignKey("Cid")]
    [InverseProperty("Timetables")]
    public virtual Course CidNavigation { get; set; } = null!;

    [ForeignKey("SecId")]
    [InverseProperty("Timetables")]
    public virtual Section Sec { get; set; } = null!;

    [ForeignKey("TecId")]
    [InverseProperty("Timetables")]
    public virtual Teacher Tec { get; set; } = null!;
}
