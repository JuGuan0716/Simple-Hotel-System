namespace Simple_Hotel_System.Models
{
    public class CheckoutItemDto
    {
        public int BookingId { get; set; }
        public string RoomName { get; set; }
        public string RoomType { get; set; }
        public int Nights { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CheckoutInfo
    {
        public string GuestName { get; set; }
        public List<CheckoutItemDto> Items { get; set; } = new();
        public decimal GrandTotal { get; set; }
    }
}
