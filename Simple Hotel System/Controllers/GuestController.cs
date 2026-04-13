using Microsoft.AspNetCore.Mvc;
using Simple_Hotel_System.Logic;
using Simple_Hotel_System.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Simple_Hotel_System.Controllers
{
    public class GuestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GuestForm(GuestInfo guest)
        {
            var result = SaveLogic.SaveGuest(guest);
            if (result.bOk)
            {
                TempData["Success"] = result.sMsg;
            }
            else
            {
                TempData["Fail"] = result.sMsg;
            }

            return RedirectToAction("Form", "Home");
        }

        //[HttpPost]
        //public IActionResult GuestLogin(GuestInfo guest)
        //{
        //    var result = SaveLogic.FindGuest(guest.Email, guest.Password);

        //    if (!result.bOk)
        //    {
        //        TempData["Fail"] = result.sMsg;
        //        return RedirectToAction("Login", "Home");
        //    }

        //    Response.Cookies.Append("GuestAuth", Crypto.Encrypt(result.guest.Id.ToString()),
        //        new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict
        //        });
        //    Response.Cookies.Append("Role", Crypto.Encrypt("Guest"),
        //        new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict
        //        });

        //    TempData["Success"] = result.sMsg + " Welcome, " + result.guest.Name;
        //    return RedirectToAction("ShowRoom", "RoomMenu");
        //}

        public async Task<IActionResult> GuestLogin(GuestInfo guest)
        {
            var result = SaveLogic.FindGuest(guest.Email, guest.Password);

            if (!result.bOk)
            {
                TempData["Fail"] = result.sMsg;
                return RedirectToAction("Login", "Home");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.guest.Id.ToString()),
                new Claim(ClaimTypes.Name, result.guest.Name),
                new Claim(ClaimTypes.Role, "Guest")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            TempData["Success"] = result.sMsg + " Welcome, " + result.guest.Name;
            return RedirectToAction("ShowRoom", "RoomMenu");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GetAllGuests()
        {
            var guests = SaveLogic.GetGuestInfo();

            var data = guests.Select((r, index) => new
            {
                no = index + 1,
                id = r.Id,
                name = r.Name,
                gender = r.Gender.ToString(),
                phonenum = r.PhoneNum,
                email = r.Email
            }).ToList();

            return Json(new { data = data });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UpdateGuest(GuestInfo guest)
        {
            if (string.IsNullOrEmpty(guest.Name))
            {
                return Json(new { success = false, message = "Guest Name is needed." });
            }

            var existingGuest = SaveLogic.GetGuestById(guest.Id);

            if(existingGuest == null)
            {
                return Json(new { success = false, message = "Guest not found." });
            }

            guest.Password = existingGuest.Password;
            var result = SaveLogic.UpdateGuest(guest);

            return Json(new
            {
                success = result.bOk,
                message = result.sMsg
            });           
        }

        [Authorize(Roles ="Admin")]
        public IActionResult DeleteGuest(int id)
        {
            var result = SaveLogic.DeleteGuest(id);

            return Json(new
            {
                success = result.bOk,
                message = result.sMsg
            });
        }

        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync();
        //    return RedirectToAction("Login", "Home");
        //}

    }

}
