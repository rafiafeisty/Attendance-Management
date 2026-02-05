using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Attendance_Management.Controllers
{
    public class PasswordController : Controller
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
        public IActionResult PasswordChangingA()
        {
            var role = GetRoleFromToken();
            if (role!="admin")
            {
                return RedirectToAction("LoginDashboard","Login");
            }
            return View();
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult PasswordChangingS()
        {
            var role = GetRoleFromToken();
            if (role!="student")
            {
                return RedirectToAction("LoginDashboard", "Login");
            }
            return View();
        }

        [Authorize(AuthenticationSchemes = "JwtAuth")]
        public IActionResult PasswordChangingT()
        {
            var role = GetRoleFromToken();
            if (role!="teacher")
            {
                return RedirectToAction("LoginDashboard", "Login");
            }
            return View();
        }
    }
}
