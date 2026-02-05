using Attendance_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using ClosedXML.Excel;


namespace Attendance_Management.Controllers
{
    public class Teacherdata
    {
        public int TeacherId { get; set; }
        public string Teachername { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class ProgramVM
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
    }

    public class SessionVM
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public int ProgramId { get; set; }
    }

    public class CourseVM
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }
    public class SectionVM
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int SessionId { get; set; }
    }
    public class ManageTeacherVM
    {
        public List<ProgramVM> Programs { get; set; }
        public List<SessionVM> Sessions { get; set; }
        public List<CourseVM> Courses { get; set; }
        public List<SectionVM> Sections { get; set; }
    }
    public class StudentData
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RollNumber { get; set; }
    }
    public class ManageStudentVM
    {
        public List<ProgramVM> Programs { get; set; }
        public List<SessionVM> Sessions { get; set; }
        public List<SectionVM> Sections { get; set; }
        public List<CourseVM> Courses { get; set; }
        public List<CourseSessionVM> CourseSessions { get; set; }
    }

    // New VM for CourseSession
    public class CourseSessionVM
    {
        public int CourseId { get; set; }   // cid
        public int SessionId { get; set; }  // sid
    }
    public class SectionDataVM
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string SessionName { get; set; }
        public string ProgramName { get; set; }
        public int StudentCount { get; set; }
    }

    public class SectionStudentVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RollNumber { get; set; }
    }
    public class StudentData2
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RollNumber { get; set; }
        public int SecId { get; set; } // Add this
    }
    public class SectionSearchVM
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string ProgramName { get; set; }
        public string SessionName { get; set; }
        public int StudentCount { get; set; }
    }

    public class AddSectionVM
    {
        public int ProgramId { get; set; }
        public int SessionId { get; set; }
        public string SectionName { get; set; }
    }
    public class BadgeSectionVM
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
    }
    public class BadgeVM
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string ProgramName { get; set; }
        public int ProgramId { get; set; }
        public int SectionCount { get; set; }
        public int StudentCount { get; set; }
    }

    public class BadgeSearchResult
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string ProgramName { get; set; }
        public int SectionCount { get; set; }
        public int StudentCount { get; set; }
        public List<SectionStudentCount> Sections { get; set; }
    }

    public class SectionStudentCount
    {
        public string SectionName { get; set; }
        public int StudentCount { get; set; }
    }
    public class BadgeStudentVM
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RollNumber { get; set; }
        public string SectionName { get; set; }
        public int SectionId { get; set; }
    }

    public class ManageController : Controller
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
        public IActionResult manageS()
        {
            var role = GetRoleFromToken();
            if (role != "admin")
                return RedirectToAction("LoginDashboard", "Login");

            AttendanceDbContext db = new AttendanceDbContext();

            // Fetch all Programs
            var programs = db.Programs
                             .Select(p => new ProgramVM
                             {
                                 ProgramId = p.Pid,
                                 ProgramName = p.Pname
                             }).ToList();

            // Fetch all Sessions
            var sessions = db.Sessions
                             .Select(s => new SessionVM
                             {
                                 SessionId = s.Sid,
                                 SessionName = s.SName,
                                 ProgramId = s.Pid
                             }).ToList();

            // Fetch all Sections
            var sections = db.Sections
                             .Select(s => new SectionVM
                             {
                                 SectionId = s.SecId,
                                 SectionName = s.SecName,
                                 SessionId = s.Sid
                             }).ToList();

            // Fetch all Courses
            var courses = db.Courses
                            .Select(c => new CourseVM
                            {
                                CourseId = c.Cid,
                                CourseName = c.Cname
                            }).ToList();
            var CourseSessions = db.CourseSessions.Select(cs => new CourseSessionVM { CourseId = cs.Cid, SessionId = cs.Sid }).ToList();

            var vm = new ManageStudentVM
            {
                Programs = programs,
                Sessions = sessions,
                Sections = sections,
                Courses = courses,
                CourseSessions = CourseSessions
            };

            return View(vm);
        }


        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult manageT()
        {
            var role = GetRoleFromToken();
            if (role != "admin")
                return RedirectToAction("LoginDashboard", "Login");

            AttendanceDbContext db = new AttendanceDbContext();

            var programs = db.Programs
                             .Select(p => new ProgramVM
                             {
                                 ProgramId = p.Pid,
                                 ProgramName = p.Pname
                             }).ToList();

            var sessions = db.Sessions
                             .Select(s => new SessionVM
                             {
                                 SessionId = s.Sid,
                                 SessionName = s.SName,
                                 ProgramId = s.Pid
                             }).ToList();

            var courses = db.Courses
                            .Select(c => new CourseVM
                            {
                                CourseId = c.Cid,
                                CourseName = c.Cname
                            }).ToList();

            var sections = db.Sections
                             .Select(s => new SectionVM
                             {
                                 SectionId = s.SecId,
                                 SectionName = s.SecName,
                                 SessionId = s.Sid
                             }).ToList();

            var vm = new ManageTeacherVM
            {
                Programs = programs,
                Sessions = sessions,
                Courses = courses,
                Sections = sections
            };

            return View(vm);
        }




        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult managesection()
        {
            var role = GetRoleFromToken();
            if (role != "admin")
            {
                return RedirectToAction("LoginDashboard", "Login");
            }
            AttendanceDbContext db = new AttendanceDbContext();
            var programs = db.Programs
                             .Select(p => new ProgramVM
                             {
                                 ProgramId = p.Pid,
                                 ProgramName = p.Pname
                             }).ToList();

            var sessions = db.Sessions
                             .Select(s => new SessionVM
                             {
                                 SessionId = s.Sid,
                                 SessionName = s.SName,
                                 ProgramId = s.Pid
                             }).ToList();

            var courses = db.Courses
                            .Select(c => new CourseVM
                            {
                                CourseId = c.Cid,
                                CourseName = c.Cname
                            }).ToList();

            var sections = db.Sections
                             .Select(s => new SectionVM
                             {
                                 SectionId = s.SecId,
                                 SectionName = s.SecName,
                                 SessionId = s.Sid
                             }).ToList();
            var vm = new ManageTeacherVM
            {
                Programs = programs,
                Sessions = sessions,
                Courses = courses,
                Sections = sections
            };
            return View(vm);
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult managebadge()
        {
            var role = GetRoleFromToken();
            if (role != "admin")
            {
                return RedirectToAction("LoginDashboard", "Login");
            }
            AttendanceDbContext db = new AttendanceDbContext();
            var programs = db.Programs
                             .Select(p => new ProgramVM
                             {
                                 ProgramId = p.Pid,
                                 ProgramName = p.Pname
                             }).ToList();

            var sessions = db.Sessions
                             .Select(s => new SessionVM
                             {
                                 SessionId = s.Sid,
                                 SessionName = s.SName,
                                 ProgramId = s.Pid
                             }).ToList();

            var courses = db.Courses
                            .Select(c => new CourseVM
                            {
                                CourseId = c.Cid,
                                CourseName = c.Cname
                            }).ToList();

            var sections = db.Sections
                             .Select(s => new SectionVM
                             {
                                 SectionId = s.SecId,
                                 SectionName = s.SecName,
                                 SessionId = s.Sid
                             }).ToList();
            var vm = new ManageTeacherVM
            {
                Programs = programs,
                Sessions = sessions,
                Courses = courses,
                Sections = sections
            };
            return View(vm);
        }
        //-------------STUDENT REGISTERING COURSE-----------------
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult assignS()
        {
            var role = GetRoleFromToken();
            if (role == "student")
            {
                var pkid = GetUserIdFromToken();
                AttendanceDbContext db = new AttendanceDbContext();
                var courses = (
                from s in db.Students
                join sec in db.Sections on s.SecId equals sec.SecId
                join sees in db.Sessions on sec.Sid equals sees.Sid
                join cs in db.CourseSessions on sees.Sid equals cs.Sid
                join c in db.Courses on cs.Cid equals c.Cid
                join tc in db.Teachers on cs.TecId equals tc.TecId
                where s.StId == int.Parse(pkid)
                select new
                {
                    Course_name = c.Cname,
                    teacher_name = tc.TecName,
                    course_id = c.Cid

                }
                ).Distinct().ToList();
                var enrolled = db.StudentTeacherCourses.Where(x => x.StId == int.Parse(pkid)).Select(x => x.Cid).ToList();
                var notenrolled = courses.Where(x => !enrolled.Contains(x.course_id)).ToList();
                return View(notenrolled);
            }
            else
            {
                return RedirectToAction("LoginDashboard", "Login");
            }
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public string assigncourse(string cid)
        {
            var role = GetRoleFromToken();
            if (role == "student")
            {
                var pkid = GetUserIdFromToken();
                int studentId = int.Parse(pkid);
                int courseId = int.Parse(cid);

                AttendanceDbContext db = new AttendanceDbContext();
                bool alreadyAssigned = db.StudentTeacherCourses.Any(stc => stc.StId == studentId && stc.Cid == courseId);
                var teacherid = db.CourseSessions.FirstOrDefault(x => x.Cid == courseId);
                if (alreadyAssigned)
                {
                    return "AlreadyAssigned";
                }
                StudentTeacherCourse stc = new StudentTeacherCourse
                {
                    StId = studentId,
                    Cid = courseId,
                    TecId = teacherid.TecId
                };
                db.StudentTeacherCourses.Add(stc);
                db.SaveChanges();
                return "Ok";
            }
            else
            {
                return "Error";
            }
        }

        //-------------TEACHER MANAGEMENT-----------------
        public string Addteacher(string name)
        {
            var role = GetRoleFromToken();
            if (role == "admin")
            {
                Random nb = new Random();
                int number = nb.Next(100, 9999);
                string tec_name = name;
                string username = $"{name}${number}@gmail.com";
                const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%";
                using var rng = RandomNumberGenerator.Create();
                int length = 6;
                byte[] data = new byte[length];
                rng.GetBytes(data);

                var result = new StringBuilder(length);
                foreach (byte b in data)
                {
                    result.Append(chars[b % chars.Length]);
                }
                string password = result.ToString();
                AttendanceDbContext db = new AttendanceDbContext();
                Teacher teacher = new Teacher
                {
                    TecName = tec_name,
                    Username = username,
                    Password = password
                };
                db.Teachers.Add(teacher);
                db.SaveChanges();
                return "Ok";
            }
            else
            {
                return "null";
            }
        }
        public List<Teacherdata> SearchTeacher(string name)
        {
            var role = GetRoleFromToken();
            if (role == "admin")
            {
                using (AttendanceDbContext db = new AttendanceDbContext())
                {
                    var teacherdataList = db.Teachers
                        .Where(x => x.TecName.ToLower().Contains(name.ToLower()))
                        .Select(teacher => new Teacherdata
                        {
                            TeacherId = teacher.TecId,
                            Teachername = teacher.TecName,
                            Username = teacher.Username,
                            Password = teacher.Password
                        })
                        .ToList();

                    return teacherdataList.Any() ? teacherdataList : null;
                }
            }
            else
            {
                return null;
            }
        }
        public string DeleteTeacher(string pkid)
        {
            var role = GetRoleFromToken();
            if (role != "admin")
                return "Error";

            int teacherId = int.Parse(pkid);
            AttendanceDbContext db = new AttendanceDbContext();

            using var transaction = db.Database.BeginTransaction();
            try
            {
                var stcList = db.StudentTeacherCourses.Where(x => x.TecId == teacherId).ToList();
                if (stcList.Any())
                {
                    var stcIds = stcList.Select(x => x.StcId).ToList();
                    var attendances = db.Attendances.Where(a => stcIds.Contains(a.StcId)).ToList();
                    db.Attendances.RemoveRange(attendances);

                    db.StudentTeacherCourses.RemoveRange(stcList);
                }

                var courseSessions = db.CourseSessions.Where(cs => cs.TecId == teacherId).ToList();
                db.CourseSessions.RemoveRange(courseSessions);

                var timetables = db.Timetables.Where(t => t.TecId == teacherId).ToList();
                db.Timetables.RemoveRange(timetables);

                var teacher = db.Teachers.FirstOrDefault(t => t.TecId == teacherId);
                if (teacher != null)
                    db.Teachers.Remove(teacher);

                db.SaveChanges();
                transaction.Commit();

                return "Ok";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return "Error";
            }
        }

        public string AssigncourseT(string pkid, string cid, string sid)
        {
            var role = GetRoleFromToken();
            if (role == "admin")
            {
                int courseId = int.Parse(cid);
                int sessionId = int.Parse(sid);
                int teacherId = int.Parse(pkid);

                AttendanceDbContext db = new AttendanceDbContext();

                // Check if teacher is already assigned to this course in this session
                bool alreadyAssigned = db.CourseSessions.Any(stc =>
                    stc.Cid == courseId &&
                    stc.Sid == sessionId &&
                    stc.TecId == teacherId);

                if (alreadyAssigned)
                {
                    return "AlreadyAssigned";
                }

                CourseSession stc = new CourseSession
                {
                    Cid = courseId,
                    Sid = sessionId,
                    TecId = teacherId
                };
                db.CourseSessions.Add(stc);
                db.SaveChanges();
                return "Ok";
            }
            else
            {
                return "Error";
            }
        }

        //-------------STUDENT MANAGEMENT-----------------
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string AddStudent(string name, int programId, int sessionId, int sectionId)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return "Error";

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                var program = db.Programs.FirstOrDefault(p => p.Pid == programId);
                if (program == null) return "InvalidProgram";

                string programCode = GetProgramCode(program.Pname);
                int year = DateTime.Now.Year;

                Random nb = new Random();
                int randomNumber = nb.Next(100, 999);

                string rollNumber = $"{year}-{programCode}-{randomNumber}";

                while (db.Students.Any(s => s.RollNumber == rollNumber))
                {
                    randomNumber = nb.Next(100, 999);
                    rollNumber = $"{year}-{programCode}-{randomNumber}";
                }

                int emailNumber = nb.Next(100, 9999);
                string username = $"{name}${emailNumber}@gmail.com";

                const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%";
                using var rng = RandomNumberGenerator.Create();
                int length = 6;
                byte[] data = new byte[length];
                rng.GetBytes(data);

                var result = new StringBuilder(length);
                foreach (byte b in data)
                    result.Append(chars[b % chars.Length]);
                string password = result.ToString();

                Student student = new Student
                {
                    StName = name,
                    SecId = sectionId,
                    Username = username,
                    Password = password,
                    RollNumber = rollNumber
                };

                db.Students.Add(student);
                db.SaveChanges();
                return "Ok";
            }
        }

        private string GetProgramCode(string programName)
        {
            string[] words = programName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 1)
            {
                return programName.Length >= 3 ? programName.Substring(0, 3).ToUpper() : programName.ToUpper();
            }

            // Take first letter of each word
            var code = new StringBuilder();
            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                    code.Append(word[0]);
            }

            return code.ToString().ToUpper();
        }



        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public List<StudentData2> SearchStudent(string name)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return null;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                var students = db.Students
                                 .Where(s => s.StName.ToLower().Contains(name.ToLower()))
                                 .Select(s => new StudentData2
                                 {
                                     StudentId = s.StId,
                                     StudentName = s.StName,
                                     Username = s.Username,
                                     Password = s.Password,
                                     RollNumber = s.RollNumber,
                                     SecId = s.SecId
                                 })
                                 .ToList();

                return students.Any() ? students : null;
            }
        }

        // Add method to get session by section
        [HttpGet]
        public IActionResult GetSessionBySection(int sectionId)
        {
            try
            {
                using (var db = new AttendanceDbContext())
                {
                    var section = db.Sections
                                   .Where(s => s.SecId == sectionId)
                                   .Select(s => new { sessionId = s.Sid })
                                   .FirstOrDefault();

                    if (section == null)
                        return NotFound();

                    return Ok(new { sessionId = section.sessionId });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // Add method to get courses by session
        [HttpGet]
        public IActionResult GetCoursesBySession(int sessionId)
        {
            try
            {
                using (var db = new AttendanceDbContext())
                {
                    var courses = db.CourseSessions
                                   .Where(cs => cs.Sid == sessionId)
                                   .Join(db.Courses,
                                         cs => cs.Cid,
                                         c => c.Cid,
                                         (cs, c) => new
                                         {
                                             courseId = c.Cid,
                                             courseName = c.Cname
                                         })
                                   .Distinct()
                                   .ToList();

                    return Ok(courses);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string DeleteStudent(string stid)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return "Error";

            if (!int.TryParse(stid, out int studentId))
                return "InvalidStudentId";

            using (AttendanceDbContext db = new AttendanceDbContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var stcList = db.StudentTeacherCourses.Where(stc => stc.StId == studentId).ToList();

                    if (stcList.Any())
                    {
                        var stcIds = stcList.Select(x => x.StcId).ToList();

                        var attendances = db.Attendances.Where(a => stcIds.Contains(a.StcId)).ToList();
                        db.Attendances.RemoveRange(attendances);

                        db.StudentTeacherCourses.RemoveRange(stcList);
                    }

                    var student = db.Students.FirstOrDefault(s => s.StId == studentId);
                    if (student != null)
                    {
                        db.Students.Remove(student);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                    return "Ok";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return $"Error: {ex.Message}";
                }
            }
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string AssignCourseS(string stid, string cid)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return "Error";

            if (!int.TryParse(stid, out int studentId) || !int.TryParse(cid, out int courseId))
                return "InvalidParameters";

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                var student = db.Students.FirstOrDefault(s => s.StId == studentId);
                if (student == null) return "StudentNotFound";

                var section = db.Sections.FirstOrDefault(s => s.SecId == student.SecId);
                if (section == null) return "SectionNotFound";

                // Check if course is available for this session
                var courseSession = db.CourseSessions
                                     .FirstOrDefault(cs => cs.Cid == courseId && cs.Sid == section.Sid);

                if (courseSession == null) return "CourseNotAvailableForSession";

                bool alreadyAssigned = db.StudentTeacherCourses
                                         .Any(stc => stc.StId == studentId && stc.Cid == courseId);

                if (alreadyAssigned) return "AlreadyAssigned";

                StudentTeacherCourse stc = new StudentTeacherCourse
                {
                    StId = studentId,
                    Cid = courseId,
                    TecId = courseSession.TecId
                };

                db.StudentTeacherCourses.Add(stc);
                db.SaveChanges();
                return "Ok";
            }
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public List<SectionSearchVM> SearchSection(string name)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return null;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                var sections = (
                    from sec in db.Sections
                    join ses in db.Sessions on sec.Sid equals ses.Sid
                    join prog in db.Programs on ses.Pid equals prog.Pid
                    where sec.SecName.ToLower().Contains(name.ToLower())
                    select new SectionSearchVM
                    {
                        SectionId = sec.SecId,
                        SectionName = sec.SecName,
                        ProgramName = prog.Pname,
                        SessionName = ses.SName,
                        StudentCount = db.Students.Count(s => s.SecId == sec.SecId)
                    }
                ).ToList();

                return sections.Any() ? sections : null;
            }
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string AddSection(int programId, int sessionId, string sectionName)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return "Error";

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                bool exists = db.Sections.Any(s =>
                    s.Sid == sessionId && s.SecName == sectionName);

                if (exists) return "AlreadyExists";

                Attendance_Management.Models.Section section =
                new Attendance_Management.Models.Section
                {
                    Sid = sessionId,
                    SecName = sectionName
                };


                db.Sections.Add(section);
                db.SaveChanges();
                return "Ok";
            }
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string DeleteSection(string secid)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return "Error";

            if (!int.TryParse(secid, out int sectionId))
                return "InvalidSection";

            using (AttendanceDbContext db = new AttendanceDbContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var students = db.Students.Where(s => s.SecId == sectionId).ToList();

                    if (students.Any())
                    {
                        var studentIds = students.Select(s => s.StId).ToList();

                        var stc = db.StudentTeacherCourses
                                    .Where(x => studentIds.Contains(x.StId)).ToList();

                        var stcIds = stc.Select(x => x.StcId).ToList();
                        var attendance = db.Attendances
                                           .Where(a => stcIds.Contains(a.StcId)).ToList();

                        db.Attendances.RemoveRange(attendance);
                        db.StudentTeacherCourses.RemoveRange(stc);
                        db.Students.RemoveRange(students);
                    }

                    var section = db.Sections.FirstOrDefault(s => s.SecId == sectionId);
                    if (section != null)
                        db.Sections.Remove(section);

                    db.SaveChanges();
                    transaction.Commit();
                    return "Ok";
                }
                catch
                {
                    transaction.Rollback();
                    return "Error";
                }
            }
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string UploadSectionStudents(
    IFormFile file,
    int programId,
    int sessionId,
    int sectionId)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return "Error";

            if (file == null || file.Length == 0)
                return "NoFile";

            using (AttendanceDbContext db = new AttendanceDbContext())
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1); // skip header

                foreach (var row in rows)
                {
                    string name = row.Cell(1).GetString();
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    string programCode = GetProgramCode(
                        db.Programs.First(p => p.Pid == programId).Pname);

                    int year = DateTime.Now.Year;
                    int rand = new Random().Next(100, 999);
                    string roll = $"{year}-{programCode}-{rand}";

                    while (db.Students.Any(s => s.RollNumber == roll))
                        roll = $"{year}-{programCode}-{new Random().Next(100, 999)}";

                    string username = $"{name}${rand}@gmail.com";
                    string password = GeneratePassword();

                    Student st = new Student
                    {
                        StName = name,
                        SecId = sectionId,
                        Username = username,
                        Password = password,
                        RollNumber = roll
                    };

                    db.Students.Add(st);
                }

                db.SaveChanges();
                return "Ok";
            }
        }
        private string GeneratePassword()
        {
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%";
            using var rng = RandomNumberGenerator.Create();
            byte[] data = new byte[6];
            rng.GetBytes(data);

            var sb = new StringBuilder();
            foreach (byte b in data)
                sb.Append(chars[b % chars.Length]);

            return sb.ToString();
        }
        [HttpGet]
        public IActionResult GetSessionsByProgram(int programId)
        {
            using var db = new AttendanceDbContext();
            var sessions = db.Sessions
                .Where(s => s.Pid == programId)
                .Select(s => new { s.Sid, s.SName })
                .ToList();

            return Ok(sessions);
        }
        [HttpGet]
        public IActionResult GetSectionsBySession(int sessionId)
        {
            using var db = new AttendanceDbContext();
            var sections = db.Sections
                .Where(s => s.Sid == sessionId)
                .Select(s => new { s.SecId, s.SecName })
                .ToList();

            return Ok(sections);
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string UploadBadgeStudents(
    IFormFile file,
    int programId,
    string sessionName, // Changed from sessionId to sessionName
    int numberOfSections)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return "Error";

            if (file == null || file.Length == 0)
                return "NoFile";

            if (numberOfSections <= 0 || numberOfSections > 10)
                return "InvalidSectionCount";

            if (string.IsNullOrWhiteSpace(sessionName))
                return "InvalidSessionName";

            using (AttendanceDbContext db = new AttendanceDbContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Check if session already exists
                    bool sessionExists = db.Sessions.Any(s =>
                        s.SName.ToLower() == sessionName.ToLower() && s.Pid == programId);

                    if (sessionExists)
                        return "SessionAlreadyExists";

                    // 2️⃣ Create the session (badge) first
                    var newSession = new Session
                    {
                        SName = sessionName,
                        Pid = programId
                    };

                    db.Sessions.Add(newSession);
                    db.SaveChanges(); // Save to get the SessionId

                    int sessionId = newSession.Sid; // Get the generated ID

                    /* =======================
                       3️⃣ READ EXCEL
                    ========================*/
                    List<string> studentNames = new();

                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        using var workbook = new XLWorkbook(stream);
                        var worksheet = workbook.Worksheet(1);

                        foreach (var row in worksheet.RowsUsed().Skip(1))
                        {
                            string name = row.Cell(1).GetString();
                            if (!string.IsNullOrWhiteSpace(name))
                                studentNames.Add(name.Trim());
                        }
                    }

                    if (!studentNames.Any())
                    {
                        transaction.Rollback();
                        return "NoStudents";
                    }

                    /* =======================
                       4️⃣ CREATE SECTIONS
                    ========================*/
                    List<Attendance_Management.Models.Section> sections = new();

                    for (int i = 1; i <= numberOfSections; i++)
                    {
                        var section = new Attendance_Management.Models.Section
                        {
                            Sid = sessionId,
                            SecName = $"B{i}"
                        };
                        db.Sections.Add(section);
                        sections.Add(section);
                    }

                    db.SaveChanges(); // important to get SecId

                    /* =======================
                       5️⃣ DISTRIBUTE STUDENTS
                    ========================*/
                    int totalStudents = studentNames.Count;
                    int baseCount = totalStudents / numberOfSections;
                    int remainder = totalStudents % numberOfSections;

                    string programCode = GetProgramCode(
                        db.Programs.First(p => p.Pid == programId).Pname);

                    int index = 0;
                    Random random = new Random();

                    for (int i = 0; i < sections.Count; i++)
                    {
                        int studentsInThisSection = baseCount + (i < remainder ? 1 : 0);

                        for (int j = 0; j < studentsInThisSection; j++)
                        {
                            if (index >= studentNames.Count) break;

                            string name = studentNames[index++];

                            int rand = random.Next(100, 999);
                            string roll = $"{DateTime.Now.Year}-{programCode}-{rand}";

                            // Ensure unique roll number
                            while (db.Students.Any(s => s.RollNumber == roll))
                            {
                                rand = random.Next(100, 999);
                                roll = $"{DateTime.Now.Year}-{programCode}-{rand}";
                            }

                            string username = $"{name}${rand}@gmail.com";
                            string password = GeneratePassword();

                            Student st = new Student
                            {
                                StName = name,
                                SecId = sections[i].SecId,
                                Username = username,
                                Password = password,
                                RollNumber = roll
                            };

                            db.Students.Add(st);
                        }
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    // Log success
                    Console.WriteLine($"Badge created successfully: {sessionName} with {sections.Count} sections and {studentNames.Count} students");
                    return "Ok";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log the exception
                    Console.WriteLine($"Error creating badge: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    return $"Error: {ex.Message}";
                }
            }
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public List<BadgeSearchResult> SearchBadge(string name)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return null;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                // Search sessions (badges) by name
                var sessions = db.Sessions
                    .Where(s => s.SName.ToLower().Contains(name.ToLower()))
                    .Join(db.Programs,
                        s => s.Pid,
                        p => p.Pid,
                        (s, p) => new
                        {
                            SessionId = s.Sid,
                            SessionName = s.SName,
                            ProgramName = p.Pname,
                            ProgramId = p.Pid
                        })
                    .ToList();

                var results = new List<BadgeSearchResult>();

                foreach (var session in sessions)
                {
                    // Get sections for this session
                    var sections = db.Sections
                        .Where(sec => sec.Sid == session.SessionId)
                        .Select(sec => new SectionStudentCount
                        {
                            SectionName = sec.SecName,
                            StudentCount = db.Students.Count(st => st.SecId == sec.SecId)
                        })
                        .ToList();

                    var result = new BadgeSearchResult
                    {
                        SessionId = session.SessionId,
                        SessionName = session.SessionName,
                        ProgramName = session.ProgramName,
                        SectionCount = sections.Count,
                        StudentCount = sections.Sum(s => s.StudentCount),
                        Sections = sections
                    };

                    results.Add(result);
                }

                return results.Any() ? results : null;
            }
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public string DeleteBadge(string sessionId)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return "Error";

            if (!int.TryParse(sessionId, out int sid))
                return "InvalidBadgeId";

            using (AttendanceDbContext db = new AttendanceDbContext())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Get all sections in this session
                    var sections = db.Sections.Where(s => s.Sid == sid).ToList();
                    var sectionIds = sections.Select(s => s.SecId).ToList();

                    if (sectionIds.Any())
                    {
                        // Get all students in these sections
                        var students = db.Students.Where(s => sectionIds.Contains(s.SecId)).ToList();
                        var studentIds = students.Select(s => s.StId).ToList();

                        if (studentIds.Any())
                        {
                            // Get student-teacher-course records
                            var stcList = db.StudentTeacherCourses
                                            .Where(stc => studentIds.Contains(stc.StId))
                                            .ToList();
                            var stcIds = stcList.Select(x => x.StcId).ToList();

                            if (stcIds.Any())
                            {
                                // Delete attendances
                                var attendances = db.Attendances
                                                   .Where(a => stcIds.Contains(a.StcId))
                                                   .ToList();
                                db.Attendances.RemoveRange(attendances);
                            }

                            // Delete student-teacher-course records
                            db.StudentTeacherCourses.RemoveRange(stcList);

                            // Delete students
                            db.Students.RemoveRange(students);
                        }

                        // Delete timetables for these sections
                        var timetables = db.Timetables
                                         .Where(t => sectionIds.Contains(t.SecId))
                                         .ToList();
                        db.Timetables.RemoveRange(timetables);

                        // Delete course sessions for this session
                        var courseSessions = db.CourseSessions
                                             .Where(cs => cs.Sid == sid)
                                             .ToList();
                        db.CourseSessions.RemoveRange(courseSessions);

                        // Delete sections
                        db.Sections.RemoveRange(sections);
                    }

                    // Delete the session (badge)
                    var session = db.Sessions.FirstOrDefault(s => s.Sid == sid);
                    if (session != null)
                        db.Sessions.Remove(session);

                    db.SaveChanges();
                    transaction.Commit();
                    return "Ok";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return $"Error: {ex.Message}";
                }
            }
        }
        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public List<BadgeStudentVM> GetStudentsByBadge(int sessionId)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return null;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                // Get all sections for this session
                var sections = db.Sections
                    .Where(s => s.Sid == sessionId)
                    .ToList();

                var result = new List<BadgeStudentVM>();

                foreach (var section in sections)
                {
                    // Get students in this section
                    var students = db.Students
                        .Where(s => s.SecId == section.SecId)
                        .Select(s => new BadgeStudentVM
                        {
                            StudentId = s.StId,
                            StudentName = s.StName,
                            Username = s.Username,
                            Password = s.Password,
                            RollNumber = s.RollNumber,
                            SectionName = section.SecName,
                            SectionId = section.SecId
                        })
                        .ToList();

                    result.AddRange(students);
                }

                return result.Any() ? result.OrderBy(s => s.SectionName).ThenBy(s => s.RollNumber).ToList() : null;
            }
        }

        // Add this new ViewModel class
       

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpPost]
        public List<SectionStudentVM> GetStudentsBySection(int sectionId)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return null;

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                // Get section details
                var section = db.Sections
                    .Where(s => s.SecId == sectionId)
                    .Select(s => new
                    {
                        s.SecName,
                        SessionId = s.Sid
                    })
                    .FirstOrDefault();

                if (section == null) return null;

                // Get session and program details
                var session = db.Sessions
                    .Where(s => s.Sid == section.SessionId)
                    .Select(s => new
                    {
                        s.SName,
                        s.Pid
                    })
                    .FirstOrDefault();

                var program = session != null ?
                    db.Programs.FirstOrDefault(p => p.Pid == session.Pid)?.Pname : null;

                // Get students in this section
                var students = db.Students
                    .Where(s => s.SecId == sectionId)
                    .Select(s => new SectionStudentVM
                    {
                        StudentId = s.StId,
                        StudentName = s.StName,
                        Username = s.Username,
                        Password = s.Password,
                        RollNumber = s.RollNumber
                    })
                    .OrderBy(s => s.RollNumber)
                    .ToList();

                return students.Any() ? students : null;
            }
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        [HttpGet]
        public IActionResult GetSectionStudents(int sectionId)
        {
            var role = GetRoleFromToken();
            if (role != "admin") return Unauthorized();

            using (AttendanceDbContext db = new AttendanceDbContext())
            {
                var students = (
                    from s in db.Students
                    join sec in db.Sections on s.SecId equals sec.SecId
                    where s.SecId == sectionId
                    select new
                    {
                        StudentId = s.StId,
                        StudentName = s.StName,
                        RollNo = s.RollNumber
                    }
                ).ToList();

                return Ok(students);
            }
        }

    }
}
