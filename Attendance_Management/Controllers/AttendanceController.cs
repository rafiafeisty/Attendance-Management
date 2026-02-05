using Attendance_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Attendance_Management.Controllers
{
    public class studentAtt
    {
        private DateTime date;
        private bool status;
        private string course_name;
        public DateTime Date { get => date; set => date = value; }
        public bool Status { get => status; set => status = value; }
        public string Course_name { get => course_name; set => course_name = value; }
    }
    public class teacherlist
    {
        public string Course_name { get; set; }
        public string Session_name { get; set; }
        public string Section_name { get; set; }
        public string Program_name { get; set; } // NEW

        public int Course_id { get; set; }
        public int Session_id { get; set; }
        public int Section_id { get; set; }
        public int Program_id { get; set; } // NEW
    }

    public class TimesAttVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public string Username { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Day { get; set; }
    }
    public class AdminAttendanceViewVM
    {
        public List<ProgramVM> Programs { get; set; }
        public List<SessionVM> Sessions { get; set; }
        public List<SectionVM> Sections { get; set; }
        public List<CourseVM> Courses { get; set; }
    }
    public class AdminAttendanceVM
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public int ProgramId { get; set; }
        public int SessionId { get; set; }
        public int SectionId { get; set; }

        public string Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
    public class StudentAttendanceAvgVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string RollNumber { get; set; }
        public double AverageAttendance { get; set; }
    }


    public class AttendanceController : Controller
    {
        private string GetUserIdFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private string GetRoleFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity?.FindFirst(ClaimTypes.Role)?.Value;
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult attendanceS()
        {
            var role=GetRoleFromToken();
            if (role=="student")
            {
                var pkid=GetUserIdFromToken();
                AttendanceDbContext db = new AttendanceDbContext();
                var att = (
                    from stc in db.StudentTeacherCourses
                    join at in db.Attendances on stc.StcId equals at.StcId
                    join c in db.Courses on stc.Cid equals c.Cid
                    where stc.StId == int.Parse(pkid)
                    select new
                    {
                        at.AttDate,
                        at.Status,
                        c.Cname
                    }
                    ).AsEnumerable() 
                    .Select(x => new studentAtt
                    {
                        Date = x.AttDate.ToDateTime(TimeOnly.MinValue),
                        Status = x.Status,
                        Course_name = x.Cname
                    })
                    .ToList();
                return View(att);
            }
            return RedirectToAction("LoginDashboard", "Login");
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult ViewT()
        {
            var role = GetRoleFromToken();
            if (role != "teacher")
                return RedirectToAction("LoginDashboard", "Login");

            int teacherId = int.Parse(GetUserIdFromToken());
            using AttendanceDbContext db = new AttendanceDbContext();

            var data = (
                from cs in db.CourseSessions
                join c in db.Courses on cs.Cid equals c.Cid
                join s in db.Sessions on cs.Sid equals s.Sid
                join p in db.Programs on s.Pid equals p.Pid
                join sec in db.Sections on s.Sid equals sec.Sid into secGroup
                from sec in secGroup.DefaultIfEmpty()
                where cs.TecId == teacherId
                select new teacherlist
                {
                    Course_name = c.Cname,
                    Session_name = s.SName,
                    Section_name = sec != null ? sec.SecName : null,
                    Program_name = p.Pname,

                    Course_id = c.Cid,
                    Session_id = s.Sid,
                    Section_id = sec != null ? sec.SecId : 0,
                    Program_id = p.Pid
                }
            )
            .Distinct()
            .ToList();

            return View(data); // ✅ NEVER null
        }


        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult attendanceT()
        {
            var role = GetRoleFromToken();
            if (role != "teacher")
                return RedirectToAction("LoginDashboard", "Login");

            int teacherId = int.Parse(GetUserIdFromToken());
            AttendanceDbContext db = new AttendanceDbContext();

            var teacherl = (
                from cs in db.CourseSessions
                join c in db.Courses on cs.Cid equals c.Cid
                join s in db.Sessions on cs.Sid equals s.Sid
                join p in db.Programs on s.Pid equals p.Pid
                join sec in db.Sections on s.Sid equals sec.Sid into secGroup
                from sec in secGroup.DefaultIfEmpty()

                where cs.TecId == teacherId

                select new teacherlist
                {
                    Course_name = c.Cname,
                    Session_name = s.SName,
                    Section_name = sec != null ? sec.SecName : null,
                    Program_name = p.Pname,

                    Course_id = c.Cid,
                    Session_id = s.Sid,
                    Section_id = sec != null ? sec.SecId : 0,
                    Program_id = p.Pid
                }
            )
            .Distinct()
            .ToList();

            return View(teacherl);
        }


        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public List<TimesAttVM> fetchingstudents(string cid, string sid, string secid, string pid)
        {
            var role = GetRoleFromToken();
            if (role != "teacher")
                return new List<TimesAttVM>();

            int teacherId = int.Parse(GetUserIdFromToken());
            int courseId = int.Parse(cid);
            int sessionId = int.Parse(sid);
            int sectionId = int.Parse(secid);
            int programId = int.Parse(pid);

            AttendanceDbContext db = new AttendanceDbContext();

            string today = DateTime.Now.DayOfWeek.ToString();

            var timetable = db.Timetables
                .Where(tt =>
                    tt.Cid == courseId &&
                    tt.SecId == sectionId &&
                    tt.TecId == teacherId
                )
                .Select(tt => new
                {
                    tt.StartTime,
                    tt.EndTime,
                    tt.DaysOfWeek
                })
                .FirstOrDefault();

            if (timetable == null)
                return new List<TimesAttVM>();

            var students = (
                from s in db.Students
                join sec in db.Sections on s.SecId equals sec.SecId
                join ses in db.Sessions on sec.Sid equals ses.Sid
                join stc in db.StudentTeacherCourses on s.StId equals stc.StId
                where ses.Sid == sessionId
                   && ses.Pid == programId
                   && sec.SecId == sectionId
                   && stc.Cid == courseId
                   && stc.TecId == teacherId
                select new TimesAttVM
                {
                    StudentId = s.StId,
                    StudentName = s.StName,
                    RollNumber = s.RollNumber,
                    Username = s.Username,
                    StartTime = timetable.StartTime,
                    EndTime = timetable.EndTime,
                    Day = timetable.DaysOfWeek
                }
            )
            .AsEnumerable()        // <-- Fetch data into memory
            .DistinctBy(s => s.StudentId)
            .OrderBy(x => x.RollNumber)
            .ToList();


            return students;
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string marking(
    string cid,
    string sec_id,
    string sid,
    string pid,
    List<string> st_id,
    List<string> status,
    bool isEdit = false
)
        {
            var role = GetRoleFromToken();
            if (role != "teacher")
                return "Error";

            int teacherId = int.Parse(GetUserIdFromToken());
            int courseId = int.Parse(cid);
            int sectionId = int.Parse(sec_id);
            int sessionId = int.Parse(sid);
            int programId = int.Parse(pid);

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            using AttendanceDbContext db = new AttendanceDbContext();

            for (int i = 0; i < st_id.Count; i++)
            {
                int studentId = int.Parse(st_id[i]);
                bool attStatus = status[i] == "true";

                int stcId = (
                    from stc in db.StudentTeacherCourses
                    join s in db.Students on stc.StId equals s.StId
                    join sec in db.Sections on s.SecId equals sec.SecId
                    join ses in db.Sessions on sec.Sid equals ses.Sid
                    where stc.StId == studentId
                          && stc.Cid == courseId
                          && stc.TecId == teacherId
                          && sec.SecId == sectionId
                          && ses.Sid == sessionId
                          && ses.Pid == programId
                    select stc.StcId
                ).FirstOrDefault();

                if (stcId == 0) continue;

                var existing = db.Attendances
                    .FirstOrDefault(a => a.StcId == stcId && a.AttDate == today);

                if (existing != null)
                {
                    if (!isEdit) continue;

                    // UPDATE
                    existing.Status = attStatus;
                    existing.StartTime = TimeOnly.FromDateTime(DateTime.Now);
                    existing.EndTime = TimeOnly.FromDateTime(DateTime.Now);
                }
                else
                {
                    // INSERT
                    db.Attendances.Add(new Attendance
                    {
                        StcId = stcId,
                        AttDate = today,
                        Status = attStatus,
                        StartTime = TimeOnly.FromDateTime(DateTime.Now),
                        EndTime = TimeOnly.FromDateTime(DateTime.Now)
                    });
                }
            }

            db.SaveChanges();
            return "Ok";
        }


        [Authorize(AuthenticationSchemes = "JwtAuth")]
        
        

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpGet]
        public IActionResult GetCoursesBySession(int sessionId)
        {
            var role = GetRoleFromToken();
            if (role != "admin")
                return Unauthorized();

            using AttendanceDbContext db = new AttendanceDbContext();

            var courses = (
                from cs in db.CourseSessions
                join c in db.Courses on cs.Cid equals c.Cid
                where cs.Sid == sessionId
                select new
                {
                    courseId = c.Cid,
                    courseName = c.Cname
                }
            )
            .Distinct()
            .ToList();

            return Json(courses);
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpGet]
        public IActionResult CheckAttendance(
    int cid, int secid, int sid, int pid)
        {
            var role = GetRoleFromToken();
            if (role != "teacher")
                return Unauthorized();

            int teacherId = int.Parse(GetUserIdFromToken());
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            using AttendanceDbContext db = new AttendanceDbContext();

            bool exists = (
                from a in db.Attendances
                join stc in db.StudentTeacherCourses on a.StcId equals stc.StcId
                join s in db.Students on stc.StId equals s.StId
                join sec in db.Sections on s.SecId equals sec.SecId
                join ses in db.Sessions on sec.Sid equals ses.Sid
                where a.AttDate == today
                      && stc.Cid == cid
                      && stc.TecId == teacherId
                      && sec.SecId == secid
                      && ses.Sid == sid
                      && ses.Pid == pid
                select a
            ).Any();

            return Json(new { alreadyMarked = exists });
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpGet]
        public IActionResult FetchRegisteredStudents(
    int cid, int sid, int secid, int pid)
        {
            var role = GetRoleFromToken();
            if (role != "teacher")
                return Unauthorized();

            int teacherId = int.Parse(GetUserIdFromToken());

            using AttendanceDbContext db = new AttendanceDbContext();

            var students = (
                 from stc in db.StudentTeacherCourses
                 join s in db.Students on stc.StId equals s.StId
                 join sec in db.Sections on s.SecId equals sec.SecId
                 join ses in db.Sessions on sec.Sid equals ses.Sid
                 where stc.Cid == cid
                       && stc.TecId == teacherId
                       && sec.SecId == secid
                       && ses.Sid == sid
                       && ses.Pid == pid
                 select new StudentAttendanceAvgVM
                 {
                     StudentId = s.StId,
                     StudentName = s.StName,
                     RollNumber = s.RollNumber,

                     AverageAttendance =
                         db.Attendances.Any(a => a.StcId == stc.StcId)
                         ? Math.Round(
                             db.Attendances.Count(a => a.StcId == stc.StcId && a.Status) * 100.0 /
                             db.Attendances.Count(a => a.StcId == stc.StcId),
                             2)
                         : 0
                 }
             )
             .OrderBy(x => x.RollNumber)
             .ToList();


            return Json(students);
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpGet]
        public IActionResult GetStudentAttendanceRecord(
    int studentId, int cid)
        {
            int teacherId = int.Parse(GetUserIdFromToken());

            using AttendanceDbContext db = new AttendanceDbContext();

            var record = (
                from a in db.Attendances
                join stc in db.StudentTeacherCourses on a.StcId equals stc.StcId
                where stc.StId == studentId
                      && stc.Cid == cid
                      && stc.TecId == teacherId
                orderby a.AttDate descending
                select new
                {
                    date = a.AttDate.ToString("yyyy-MM-dd"),
                    status = a.Status ? "Present" : "Absent"
                }
            ).ToList();

            return Json(record);
        }

    }
}
