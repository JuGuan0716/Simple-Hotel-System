using Microsoft.AspNetCore.Mvc;
using Simple_Hotel_System.Logic;
using Simple_Hotel_System.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Simple_Hotel_System.Controllers
{
    public class AdminController : Controller
    {
        [Authorize (Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AdminLoginPage()
        {

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminRegisterPage()
        {

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminTable()
        {

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminRoomTable()
        {

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminGuestTable()
        {

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminBookingTable()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin(AdminInfo admin)
        {
            var result = AdminSave.GetAdmin(admin.Name, admin.Password);
            if (!result.bOk)
            {
                TempData["Fail"] = result.sMsg;
                return RedirectToAction("AdminLoginPage", "Admin");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.admin.Id.ToString()),
                new Claim(ClaimTypes.Name, result.admin.Name),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );
            TempData["Success"] = result.sMsg;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminRegister(AdminInfo admin)
        {
            var result = AdminSave.SetAdmin(admin);
            if (result.bOk)
            {
                TempData["Success"] = result.sMsg;
            }
            else
            {
                TempData["Fail"] = result.sMsg;
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GetGuest()
        {
            var guest = SaveLogic.GetGuestInfo();
            return Json(guest);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GetTable()
        {
            try
            {
                var result = DataTableSave.GetGuest();
                var data = result.Select((r, index) => new
                {
                    no = index + 1,
                    Name = r.Name,
                    Id = r.Id
                }).ToList();

                return Json(new
                {
                    draw = 1,
                    recordsTotal = data.Count,
                    recordsFiltered = data.Count,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = 1,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = ex.Message
                });
            }
        }

    }
}
