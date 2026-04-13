using cinema_ticketing.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Simple_Hotel_System.Logic;
using Simple_Hotel_System.Models;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Simple_Hotel_System.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Guest")]
        public IActionResult CheckOut()
        {
            int guestId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var items = BookingSave.GetCheckoutItemdByGuestId(guestId);

            CheckoutInfo model = new CheckoutInfo
            {
                GuestName = User.Identity.Name,
                Items = items,
                GrandTotal = items.Sum(x => x.TotalPrice)
            };

            return View(model);
        }

        [Authorize]
        public IActionResult BookingReceipt(int id)
        {
            var receipt = BookingSave.GetReceiptByBookingId(id);

            if (receipt == null)
            {
                TempData["Fail"] = "Receipt not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(receipt);
        }

        [Authorize]
        public IActionResult PrintReceipt (int id)
        {
            var receipt = BookingSave.GetReceiptByBookingId(id);
            if (receipt == null)
            {
                TempData["Fail"] = "Receipt not found.";
                return RedirectToAction("Index", "Home");
            }

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Templates/receipt.html");
            string html = System.IO.File.ReadAllText(filePath);

            html = html.Replace("{{BookingId}}", receipt.Booking.Id.ToString());
            html = html.Replace("{{GuestId}}", receipt.Guest.Id.ToString());
            html = html.Replace("{{GuestName}}", receipt.Guest.Name);
            html = html.Replace("{{BookingRoomId}}", receipt.Room.Id.ToString());
            html = html.Replace("{{RoomType}}", receipt.Room.Type);
            html = html.Replace("{{CheckIn}}", receipt.Booking.CheckIn.ToString("dd/MM/yyyy"));
            html = html.Replace("{{CheckOut}}", receipt.Booking.CheckOut.ToString("dd/MM/yyyy"));
            html = html.Replace("{{Nights}}", receipt.Booking.Nights.ToString());
            html = html.Replace("{{RoomPrice}}", receipt.Room.Price.ToString("F2"));
            html = html.Replace("{{TotalPrice}}", receipt.Booking.TotalPrice.ToString("F2"));

            return Content(html, "text/html");
        }

        [Authorize(Roles = "Guest")]
        [HttpPost]
        public IActionResult Reserve(int RoomId, DateTime CheckIn, DateTime CheckOut)
        {
            var room = RoomSave.GetRoomById(RoomId);
            if (room == null)
            {
                return NotFound();
            }
            int nights = (CheckOut - CheckIn).Days;
            decimal totalPrice = nights * room.Price;

            BookingInfo booking = new BookingInfo
            {
                GuestId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                RoomId = RoomId,
                CheckIn = CheckIn,
                CheckOut = CheckOut,
                Nights = nights,
                TotalPrice = totalPrice
            };

            var result = BookingSave.SetBooking(booking);

            if (!result.bOk)
            {
                TempData["Fail"] = result.sMsg;
                return RedirectToAction("ShowRoom", "RoomMenu");
            }

            int bookingId = result.newId;

            return RedirectToAction("BookingReceipt", new { id = bookingId });
        }

        [Authorize]
        public IActionResult GetBookedDates(int roomId)
        {
            var bookedDates = BookingSave.GetBookedDates(roomId);

            var disabledDates = bookedDates.Select(b => new
            {
                from = b.CheckIn.ToString("yyyy-MM-dd"),
                to = b.CheckOut.AddDays(-1).ToString("yyyy-MM-dd")
            });

            return Json(disabledDates);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GetAllBookings()
        {
            var bookings = BookingSave.GetBookingInfo();

            var data = bookings.Select((r, index) => new
            {
                no = index + 1,
                id = r.Id,
                guestid = r.GuestId,
                roomid = r.RoomId,
                checkin = r.CheckIn.ToString("yyyy-MM-dd"),
                checkout = r.CheckOut.ToString("yyyy-MM-dd"),
                nights = r.Nights,
                totalprice = r.TotalPrice.ToString("F2")
            }).ToList();

            return Json(new { data = data });
        }
    }
}

