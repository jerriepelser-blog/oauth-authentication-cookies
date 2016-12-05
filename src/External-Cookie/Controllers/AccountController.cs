using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationBasic.Controllers
{
    public class AccountController : Controller
    {
        public async Task<IActionResult> Login()
        {
            // Construct the redirect url to go to the RemoteLoginCallback action
            var redirectUrl = Url.Action("RemoteLoginCallback", "Account", new { ReturnUrl = "/" });

            // Ensure we are signed out of the remote cookie auth
            await HttpContext.Authentication.SignOutAsync("RemoteAuthCookie");

            // Challenge the GH provider
            return new ChallengeResult("GitHub", new AuthenticationProperties() { RedirectUri = redirectUrl });
        }

        public IActionResult Logout()
        {
            HttpContext.Authentication.SignOutAsync("GitHub");
            HttpContext.Authentication.SignOutAsync("ApplicationCookie");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> RemoteLoginCallback(string returnUrl)
        {
            var auth = new AuthenticateContext("RemoteAuthCookie");

            // Get auth ticket from remote cookie
            await HttpContext.Authentication.AuthenticateAsync(auth);

            if (auth.Accepted)
            {
                // Sign out of remote cookie once we used it
                await HttpContext.Authentication.SignOutAsync("RemoteAuthCookie");

                // Sign the user in
                await HttpContext.Authentication.SignInAsync("ApplicationCookie", auth.Principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                });

                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If we don't have an external auth cookie, redirect to login action
                return RedirectToAction(nameof(Login));
            }

        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
