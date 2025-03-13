
namespace SoundScape.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailConfirmed { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public string Gender { get; set; }
        public string AvatarUrl { get; set; } = "/images/default-avatar.png";
        public ICollection<Track> FavoriteTracks { get; set; }
        public object Orders { get; internal set; }
    }
}