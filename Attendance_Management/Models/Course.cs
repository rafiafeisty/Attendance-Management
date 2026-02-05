using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Course")]
public partial class Course
{
    [Key]
    [Column("cid")]
    public int Cid { get; set; }

    [Column("cname")]
    [StringLength(50)]
    public string Cname { get; set; } = null!;

    [InverseProperty("CidNavigation")]
    public virtual ICollection<CourseSession> CourseSessions { get; set; } = new List<CourseSession>();

    [InverseProperty("CidNavigation")]
    public virtual ICollection<StudentTeacherCourse> StudentTeacherCourses { get; set; } = new List<StudentTeacherCourse>();

    [InverseProperty("CidNavigation")]
    public virtual ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
}
