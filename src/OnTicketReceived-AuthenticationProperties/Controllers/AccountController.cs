using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationBasic.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return new ChallengeResult("GitHub", new AuthenticationProperties() { RedirectUri = "/" });
        }

        public IActionResult Logout()
        {
            HttpContext.Authentication.SignOutAsync("GitHub");
            HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
