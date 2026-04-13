namespace Simple_Hotel_System.Models
{
    public class ReceiptInfo
    {
        public GuestInfo Guest { get; set; }
        public BookingInfo Booking { get; set; }
        public RoomInfo Room { get; set; }
    }
}
