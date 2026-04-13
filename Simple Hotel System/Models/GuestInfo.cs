namespace Simple_Hotel_System.Models
{
    public class GuestInfo
    {
        public int Id {  get; set; }
        public string Name {  get; set; }
        public string Password { get; set; }
        public Gender Gender { get; set; }
        public string? PhoneNum { get; set; }
        public string? Email { get; set; }
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        PreferNotToSay = 3
    }
}
