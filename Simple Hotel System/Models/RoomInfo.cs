namespace Simple_Hotel_System.Models
{
    public class RoomInfo
    {
        public int Id { get; set; }
        public string ? Name { get; set; }
        public string ? Type { get; set; }
        public string? Status { get; set; }
        public decimal Price { get; set; }
        public string ? PicUrl { get; set; }
        public bool isActive { get; set; }
    }

}
