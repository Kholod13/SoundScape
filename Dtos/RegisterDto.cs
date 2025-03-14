namespace SoundScape.Models
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public string Gender { get; set; }
        public string AvatarUrl { get; set; } = "/images/default-avatar.png";
    }
}
