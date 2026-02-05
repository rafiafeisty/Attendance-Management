using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Teacher")]
[Index("Username", Name = "UQ__Teacher__F3DBC572D609414D", IsUnique = true)]
public partial class Teacher
{
    [Key]
    [Column("tec_id")]
    public int TecId { get; set; }

    [Column("tec_name")]
    [StringLength(50)]
    public string TecName { get; set; } = null!;

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(50)]
    public string Password { get; set; } = null!;

    [InverseProperty("Tec")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [InverseProperty("Tec")]
    public virtual ICollection<StudentTeacherCourse> StudentTeacherCourses { get; set; } = new List<StudentTeacherCourse>();

    [InverseProperty("Tec")]
    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
