using Attendance_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Attendance_Management.Controllers
{
    public class studentAttribute
    {
        private DateTime date;
        private bool status;
        private string course_name;
        public DateTime Date { get => date; set => date = value; }
        public bool Status { get => status; set => status = value; }
        public string Course_name { get => course_name; set => course_name = value; }
    }

        public class ReportController : Controller
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
        public IActionResult reportS()
        {
            var role = GetRoleFromToken();
            if (role=="student")
            {
                var pkid = GetUserIdFromToken();
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
                    .Select(x => new studentAttribute
                    {
                        Date = x.AttDate.ToDateTime(TimeOnly.MinValue),
                        Status = x.Status,
                        Course_name = x.Cname
                    })
                    .ToList();
                return View(att);
            }
            else
            {
                return RedirectToAction("LoginDashboard", "Login()");
            }
        }

    }
}
