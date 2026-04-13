using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Simple_Hotel_System.Logic;
using Simple_Hotel_System.Models;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Simple_Hotel_System.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Form()
        {
            return View();
        }

        public IActionResult TestUI()
        {

            return View();
        }

        public IActionResult Login()
        {

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult GetEncryption(AdminInfo admin)
        {
            var encrypted = Crypto.Encrypt(admin.Name);
            return Json(encrypted);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        

        //public IActionResult DELETEANY()
        //{

        //    return View();
        //}

        //public static (bool bOk, string sMsg) DeletewithId(int bookingId)
        //{
        //    var result = Delete.DeleteAny(bookingId);

        //    if (result.bOk)
        //        return (true, result.sMsg);
        //    else
        //        return (false, result.sMsg);
        //}

    }
}
