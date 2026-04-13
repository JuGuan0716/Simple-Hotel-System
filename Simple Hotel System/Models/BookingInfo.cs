namespace Simple_Hotel_System.Models
{
    public class BookingInfo
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public decimal TotalPrice { get; set; }
        public RoomInfo Room { get; set; }
    }

    public class BookingInfoDto
    {
        public string? GuestName { get; set; }
        public string? RoomName { get; set; }
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }

    }
}
