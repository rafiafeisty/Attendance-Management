using System;
using System.Collections.Generic;
using Attendance_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management.Models;

public partial class AttendanceDbContext : DbContext
{
    public AttendanceDbContext()
    {
    }

    public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseSession> CourseSessions { get; set; }

    public virtual DbSet<Program> Programs { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentTeacherCourse> StudentTeacherCourses { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Timetable> Timetables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Attendance;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdId).HasName("PK__Admin__CAA4A627355A5C01");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AtId).HasName("PK__Attendan__61F85988A24E701B");

            entity.ToTable("Attendance", tb => tb.HasTrigger("trg_Attendance_SetTime"));

            entity.HasOne(d => d.Stc).WithMany(p => p.Attendances)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendance_STC");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Cid).HasName("PK__Course__D837D05FADA92272");
        });

        modelBuilder.Entity<CourseSession>(entity =>
        {
            entity.HasKey(e => e.CsId).HasName("PK__CourseSe__138C55F43428784F");

            entity.HasOne(d => d.CidNavigation).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseSession_Course");

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseSession_Session");

            entity.HasOne(d => d.Tec).WithMany(p => p.CourseSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseSession_Teacher");
        });

        modelBuilder.Entity<Program>(entity =>
        {
            entity.HasKey(e => e.Pid).HasName("PK__Program__DD37D91A90298965");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SecId).HasName("PK__Section__E9DE06E76DBF72A6");

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.Sections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Section_Session");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Sid).HasName("PK__Session__DDDFDD36EF55B7D3");

            entity.HasOne(d => d.PidNavigation).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Session_Program");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StId).HasName("PK__Student__A85E81CF7B866D29");

            entity.HasOne(d => d.Sec).WithMany(p => p.Students)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Section");
        });

        modelBuilder.Entity<StudentTeacherCourse>(entity =>
        {
            entity.HasKey(e => e.StcId).HasName("PK__StudentT__B5DD53467417A120");

            entity.HasOne(d => d.CidNavigation).WithMany(p => p.StudentTeacherCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_STC_Course");

            entity.HasOne(d => d.St).WithMany(p => p.StudentTeacherCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_STC_Student");

            entity.HasOne(d => d.Tec).WithMany(p => p.StudentTeacherCourses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_STC_Teacher");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TecId).HasName("PK__Teacher__6C54E3C0FDC07D79");
        });

        modelBuilder.Entity<Timetable>(entity =>
        {
            entity.HasKey(e => e.TimeId).HasName("PK__Timetabl__0FB76BB94DF2EAC3");

            entity.HasOne(d => d.CidNavigation).WithMany(p => p.Timetables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timetable_Course");

            entity.HasOne(d => d.Sec).WithMany(p => p.Timetables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timetable_Section");

            entity.HasOne(d => d.Tec).WithMany(p => p.Timetables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timetable_Teacher");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
