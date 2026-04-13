using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simple_Hotel_System.Logic;
using Simple_Hotel_System.Models;

namespace Simple_Hotel_System.Controllers
{
    public class RoomMenuController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult AddRoom()
        {
            return View();
        }

        [Authorize(Roles = "Guest")]
        public IActionResult ShowRoom()
        {
            var rooms = RoomSave.GetRoomInfo();

            var room = rooms.FirstOrDefault(r => r.Status == "Available");
            if (room == null)
                return NotFound();

            ViewBag.RoomTypes = rooms
                .Where(r => r.Status == "Available")
                .Select(r => r.Type)
                .Distinct()
                .ToList();

            ViewBag.AllRooms = rooms;


            return View(room);
        }

        public IActionResult TestAuth()
        {
            return Content(User.Identity.IsAuthenticated.ToString());
        }

        public IActionResult TestRole()
        {
            return Content(User.IsInRole("Guest").ToString());
        }

        public IActionResult DebugClaims()
        {
            return Content(string.Join("\n",
                User.Claims.Select(c => $"{c.Type} = {c.Value}")
            ));
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SetRoom(RoomInfo room, IFormFile file)
        {
            var uploadResult = FileSave.SaveImage(file, "rooms");

            if (!uploadResult.bOk)
            {
                TempData["Fail"] = uploadResult.sMsg;
                return RedirectToAction("AddRoom", "RoomMenu");
            }

            room.PicUrl = uploadResult.fileUrl;

            var result = RoomSave.SetRoom(room);

            if (result.bOk)
            {
                TempData["Success"] = result.sMsg;
            }
            else
            {
                FileSave.DeleteImage(uploadResult.fileUrl);
                TempData["Fail"] = result.sMsg;
                return RedirectToAction("AddRoom", "RoomMenu");
            }

            return RedirectToAction("AdminRoomTable", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GetAllRooms()
        {
            var rooms = RoomSave.GetRoomInfo();

            var data = rooms.Select((r, index) => new
            {
                no = index + 1,
                id = r.Id,
                name = r.Name,
                type = r.Type,
                status = r.Status,
                price = r.Price,
                picUrl = r.PicUrl
            }).ToList();

            return Json(new { data = data });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult UpdateRoom (RoomInfo room, IFormFile file)
        {
            if (file != null)
            {
                var uploadResult = FileSave.SaveImage(file, "rooms");
                if (uploadResult.bOk)
                {
                    room.PicUrl = uploadResult.fileUrl;
                }
            }

            var result = RoomSave.UpdateRoom(room);

            return Json(new
            {
                success = result.bOk,
                message = result.sMsg
            });

            //if (result.bOk)
            //{
            //    return Json(new { success = true, message = result.sMsg });
            //}
            //else
            //{
            //    return Json(new { success = false, message = result.sMsg });
            //}
        }

        public IActionResult DeleteRoom(int id)
        {
            var result = RoomSave.DeleteRoom(id);

            return Json(new
            {
                success = result.bOk,
                message = result.sMsg
            });
        }

        //public IActionResult Create(int roomId)
        //{
        //    var rooms = RoomSave.GetRoomInfo();

        //    var room = rooms.FirstOrDefault(r => r.Id == roomId);
        //    if (room == null)
        //        return NotFound();

        //    ViewBag.RoomTypes = rooms
        //        .Where(r => r.Status == "Available")
        //        .Select(r => r.Type)
        //        .Distinct()
        //        .ToList();

        //    return View("ShowRoom", room);
        //}


    }
}