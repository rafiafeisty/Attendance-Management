using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

[Table("Student")]
[Index("Username", Name = "UQ__Student__F3DBC572C4844899", IsUnique = true)]
public partial class Student
{
    [Key]
    [Column("st_id")]
    public int StId { get; set; }

    [Column("st_name")]
    [StringLength(50)]
    public string StName { get; set; } = null!;

    [Column("sec_id")]
    public int SecId { get; set; }

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(50)]
    public string Password { get; set; } = null!;

    [Column("roll_number")]
    [StringLength(20)]
    public string RollNumber { get; set; } = null!;

    [ForeignKey("SecId")]
    [InverseProperty("Students")]
    public virtual Section Sec { get; set; } = null!;

    [InverseProperty("St")]
    public virtual ICollection<StudentTeacherCourse> StudentTeacherCourses { get; set; } = new List<StudentTeacherCourse>();
}
