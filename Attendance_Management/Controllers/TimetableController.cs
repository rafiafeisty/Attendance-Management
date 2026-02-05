using Attendance_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Attendance_Management.Controllers
{
    public class TimetableVM
    {
        public string Day { get; set; }         
        public int Cid { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string CourseName { get; set; }
        public string TeacherName { get; set; }
        public string SectionName { get; set; }
    }

    public class TimetableController : Controller
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
        public IActionResult ShowTimetableS()
        {
            var role = GetRoleFromToken();
            if (role=="student")
            {
                var pkid=GetUserIdFromToken();
                AttendanceDbContext db = new AttendanceDbContext();

                var timetable = (
                    from t in db.Timetables
                    join s in db.Students on t.SecId equals s.SecId
                    join c in db.Courses on t.Cid equals c.Cid
                    join tec in db.Teachers on t.TecId equals tec.TecId
                    join sec in db.Sections on t.SecId equals sec.SecId
                    where s.StId == int.Parse(pkid)
                    orderby
                        t.DaysOfWeek == "Monday" ? 1 :
                        t.DaysOfWeek == "Tuesday" ? 2 :
                        t.DaysOfWeek == "Wednesday" ? 3 :
                        t.DaysOfWeek == "Thursday" ? 4 :
                        t.DaysOfWeek == "Friday" ? 5 :
                        t.DaysOfWeek == "Saturday" ? 6 :
                        t.DaysOfWeek == "Sunday" ? 7 : 8,
                        t.StartTime
                    select new TimetableVM
                    {
                        Day = t.DaysOfWeek,
                        StartTime = t.StartTime,
                        EndTime = t.EndTime,
                        CourseName = c.Cname,
                        TeacherName = tec.TecName,
                        SectionName = sec.SecName
                    }
                ).ToList();

                return View(timetable);
            }
            return RedirectToAction("LoginDashboard", "Login");
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult ShowTimetableT()
        {
            var role = GetRoleFromToken();
            if (role=="teacher")
            {
                var pkid = GetUserIdFromToken();
                AttendanceDbContext db = new AttendanceDbContext();

                var timetable = (
                    from t in db.Timetables
                    join tec in db.Teachers on t.TecId equals tec.TecId
                    join c in db.Courses on t.Cid equals c.Cid
                    join sec in db.Sections on t.SecId equals sec.SecId
                    where t.TecId == int.Parse(pkid)
                    orderby
                        t.DaysOfWeek == "Monday" ? 1 :
                        t.DaysOfWeek == "Tuesday" ? 2 :
                        t.DaysOfWeek == "Wednesday" ? 3 :
                        t.DaysOfWeek == "Thursday" ? 4 :
                        t.DaysOfWeek == "Friday" ? 5 :
                        t.DaysOfWeek == "Saturday" ? 6 :
                        t.DaysOfWeek == "Sunday" ? 7 : 8,
                        t.StartTime
                    select new TimetableVM
                    {
                        Day = t.DaysOfWeek,
                        StartTime = t.StartTime,
                        EndTime = t.EndTime,
                        CourseName = c.Cname,
                        TeacherName = tec.TecName,
                        SectionName = sec.SecName
                    }
                ).ToList(); 

                return View(timetable);
            }
            return RedirectToAction("LoginDashboard", "Login");
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult ShowTimetableA()
        {
            var role = GetRoleFromToken();
            if (role != "admin")
                return RedirectToAction("LoginDashboard", "Login");

            AttendanceDbContext db = new AttendanceDbContext();
            ViewBag.Programs = db.Programs.ToList(); 
            return View();
        }

        [HttpGet]
        public JsonResult GetSessions(int pid)
        {
            AttendanceDbContext db = new AttendanceDbContext();
            var sessions = db.Sessions
                             .Where(s => s.Pid == pid)
                             .Select(s => new { s.Sid, s.SName })
                             .ToList();
            return Json(sessions);
        }

        [HttpGet]
        public JsonResult GetSections(int sid)
        {
            AttendanceDbContext db = new AttendanceDbContext();
            var sections = db.Sections
                             .Where(sec => sec.Sid == sid)
                             .Select(sec => new { sec.SecId, sec.SecName })
                             .ToList();
            return Json(sections);
        }

        // Return timetable for selected program/session/section
        [HttpGet]
        public JsonResult GetTimetable(int pid, int sid, int secid)
        {
            AttendanceDbContext db = new AttendanceDbContext();

            var timetable = (
                from t in db.Timetables
                join c in db.Courses on t.Cid equals c.Cid
                join tec in db.Teachers on t.TecId equals tec.TecId
                join sec in db.Sections on t.SecId equals sec.SecId
                join s in db.Sessions on sec.Sid equals s.Sid
                where s.Pid == pid && s.Sid == sid && sec.SecId == secid
                orderby
                    t.DaysOfWeek == "Monday" ? 1 :
                    t.DaysOfWeek == "Tuesday" ? 2 :
                    t.DaysOfWeek == "Wednesday" ? 3 :
                    t.DaysOfWeek == "Thursday" ? 4 :
                    t.DaysOfWeek == "Friday" ? 5 :
                    t.DaysOfWeek == "Saturday" ? 6 :
                    t.DaysOfWeek == "Sunday" ? 7 : 8,
                    t.StartTime
                select new
                {
                    t.DaysOfWeek,
                    t.StartTime,
                    t.EndTime,
                    CourseName = c.Cname,
                    TeacherName = tec.TecName,
                    SectionName = sec.SecName
                }
            ).ToList();

            return Json(timetable);
        }
    }
}


