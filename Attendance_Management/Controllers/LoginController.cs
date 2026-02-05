using Microsoft.AspNetCore.Mvc;
using Attendance_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;


namespace Attendance_Management.Controllers
{
    public class registeredsd
    {
        public string Coursename { get; set; }
        public string Teachername { get; set; }
    }

    public class assignedc
    {
        public string Coursename { get; set; }
        public string Teachername { get; set; }
    }
    public class Adata
    {
        public int Stnumber { get; set; }
        public int Tecnumber { get; set; }
        public int Bnumber { get; set; }
        public int Secnumber { get; set; }
        public int Coursenumber { get; set; }
        public int Pnumber { get; set; }
    }
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        private string generatetoken(string userid, string role)
        {
            var jwt = _config.GetSection("jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier,userid),
                    new Claim(ClaimTypes.Role,role)
                };
            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwt["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
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

        public IActionResult LoginDashboard()
        {
            return View();
        }
        public IActionResult dashboardA()
        {
            var role = GetRoleFromToken();
            if (role == "admin")
            {
                Adata data=Admindata();
                return View(data);
            }
            else
            {
                return RedirectToAction("LoginDashboard", "Login");
            }
        }
        public IActionResult dashboardS()
        {
            AttendanceDbContext context = new AttendanceDbContext();
            var role = GetRoleFromToken();
            if (role == "student")
            {
                var pkid = GetUserIdFromToken();
                int courses = number(Convert.ToInt32(pkid));
                return View(courses);
            }
            else
            {
                return RedirectToAction("LoginDashboard", "Login");
            }
        }
        public IActionResult dashboardT()
        {
            var role=GetRoleFromToken();
            if (role == "teacher")
            {
                var pkid = GetUserIdFromToken();
                int coursest = number(Convert.ToInt32(pkid));
                return View(coursest);
            }
            else
            {
                return RedirectToAction("LoginDashboard", "Login");
            }
        }
        [HttpPost]
        public IActionResult check(string username, string password)
        {
            AttendanceDbContext context = new AttendanceDbContext();
            var login_username = username;
            var login_password = password;
            
                var user = context.Students
                    .FromSqlRaw(
                        "SELECT * FROM dbo.Student WHERE [Username] = @username AND [Password] = @password",
                        new SqlParameter("@username", login_username),
                        new SqlParameter("@password", login_password)
                    )
                    .AsNoTracking()
                    .FirstOrDefault();
                if (user != null)
                {
                    var token = generatetoken(user.StId.ToString(), "student");
                    Response.Cookies.Append("jwtToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddMinutes(60)
                    });
                    return RedirectToAction("dashboardS", "Login");
                }
            
            var teacher = context.Teachers.FromSqlRaw(
                "SELECT * FROM dbo.Teacher WHERE [Username] = @username AND [Password] = @password",
                new SqlParameter("@username", login_username),
                new SqlParameter("@password", login_password)
                ).AsNoTracking().FirstOrDefault();
            if (teacher != null)
            {
                var token = generatetoken(teacher.TecId.ToString(), "teacher");
                Response.Cookies.Append("jwtToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                });
                int coursest = number(Convert.ToInt32(teacher.TecId));
                return RedirectToAction("dashboardT","Login",coursest.ToString());
            }
            var admin = context.Admins.FromSqlRaw(
                "SELECT * FROM dbo.Admin WHERE [Username] = @username AND [Password] = @password",
                new SqlParameter("@username", login_username),
                new SqlParameter("@password", login_password)
                ).AsNoTracking().FirstOrDefault();
            if (admin != null)
            {
                var token = generatetoken(admin.AdId.ToString(), "admin");
                Response.Cookies.Append("jwtToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                });
                return RedirectToAction("dashboardA","Login");
            }
            TempData["LoginError"] = "invalid credentials";
            return RedirectToAction("LoginDashboard", "Login");

        }


        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string editing(string password)
        {
            var role = GetRoleFromToken();
            var pkid = GetUserIdFromToken();
            if (role == "student")
            {
                AttendanceDbContext db = new AttendanceDbContext();
                int rows = db.Database.ExecuteSqlRaw(
                "UPDATE Students SET Password = @password WHERE StId = @id",
                new SqlParameter("@password", password),
                new SqlParameter("@id", int.Parse(pkid))
                );
                return rows > 0 ? "Ok" : "Error";
            }
            if (role == "teacher")
            {
                AttendanceDbContext db = new AttendanceDbContext();
                int rows = db.Database.ExecuteSqlRaw(
                    "UPDATE Teachers SET Password = @password WHERE TecId = @id",
                    new SqlParameter("@password", password),
                    new SqlParameter("@id", int.Parse(pkid))
                );
                return rows > 0 ? "Ok" : "Error";
            }
            if (role == "admin")
            {
                AttendanceDbContext db = new AttendanceDbContext();
                int rows = db.Database.ExecuteSqlRaw(
                "UPDATE Admins SET Password = @password WHERE AdId = @id",
                new SqlParameter("@password", password),
                new SqlParameter("@id", int.Parse(pkid))
                );
                return rows > 0 ? "Ok" : "Error";
            }
            else
            {
                return "Error";
            }
        }

        public IActionResult Logout()
        {
            var token = Request.Cookies["jwtToken"];
            if (token != null)
            {
                Response.Cookies.Delete("jwtToken");
            }
            return RedirectToAction("LoginDashboard", "Login");
        }
        public int number(int pkid)
        {
            var role = GetRoleFromToken();
            if (role == "student")
            {
                AttendanceDbContext db = new AttendanceDbContext();
                int total_courses = db.StudentTeacherCourses.Count(x => x.StId == pkid);
                return total_courses;
            }
            else if (role == "teacher")
            {
                AttendanceDbContext db = new AttendanceDbContext();
                int total_courses = db.CourseSessions.Where(x=>x.TecId==pkid).Select(x=>x.Cid).Distinct().Count();
                return total_courses;
            }
            else
            {
                return 0;
            }
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpGet]
        public String data()
        {
            var role = GetRoleFromToken();
            if (role == "student")
            {
                AttendanceDbContext db = new AttendanceDbContext();
                var pkid = GetUserIdFromToken();
                var name=db.Students.FirstOrDefault(x => x.StId == int.Parse(pkid)).StName;
                var roll=db.Students.FirstOrDefault(x=>x.StId==int.Parse(pkid)).RollNumber;
                return name+","+roll;
            }
            else if (role == "teacher")
            {
                AttendanceDbContext db = new AttendanceDbContext();
                var pkid = GetUserIdFromToken();
                var name = db.Teachers.FirstOrDefault(x => x.TecId == int.Parse(pkid)).TecName;
                return name;
            }
            else if(role=="admin")
            {
                AttendanceDbContext db = new AttendanceDbContext();
                var pkid = GetUserIdFromToken();
                var name = db.Admins.FirstOrDefault(x => x.AdId == int.Parse(pkid)).AdName;
                return "admin";
            }
            else
            {
                return "";
            }
        }
        public List<registeredsd> Register()
        {
            AttendanceDbContext db = new AttendanceDbContext();
            var role=GetRoleFromToken();
            if (role == "student")
            {
                var pkid = GetUserIdFromToken();
                var registered=(
                    from stc in db.StudentTeacherCourses
                    join c in db.Courses on stc.Cid equals c.Cid
                    join t in db.Teachers on stc.TecId equals t.TecId
                    where stc.StId==int.Parse(pkid)
                    select new registeredsd
                    {
                        Coursename=c.Cname,
                        Teachername=t.TecName
                    }).ToList();
                return registered;
            }
            else
            {
                return null;
            }
        }
        public List<assignedc> Assignc()
        {
            AttendanceDbContext db = new AttendanceDbContext();
            var role=GetRoleFromToken();
            if (role == "teacher")
            {
                var pkid = GetUserIdFromToken();
                var assigned=(
                    from t in db.Teachers
                    join cs in db.CourseSessions on t.TecId equals cs.TecId
                    join c in db.Courses on cs.Cid equals c.Cid
                    where t.TecId==int.Parse(pkid)
                    select new assignedc
                    {
                        Coursename=c.Cname,
                        Teachername=t.TecName
                    }).Distinct().ToList();
                return assigned;
            }
            else
            {
                return null;
            }
        }
        public Adata Admindata()
        {
            AttendanceDbContext db = new AttendanceDbContext();
            var role = GetRoleFromToken();
            if (role == "admin")
            {
                var pkid = GetUserIdFromToken();
                int badge = db.Sessions.Count();
                int section=db.Sections.Count();
                int student=db.Students.Count();
                int teacher=db.Teachers.Count();
                int program=db.Programs.Count();
                int courses=db.Courses.Count();
                var data=new Adata();
                data.Bnumber=badge;
                data.Secnumber=section;
                data.Stnumber=student;
                data.Coursenumber=courses;
                data.Pnumber=program;
                data.Tecnumber=teacher;
                return data;
            }
            else
            {
                return null;
            }
        }

        
    }

}
