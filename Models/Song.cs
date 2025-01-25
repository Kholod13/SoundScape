using System.ComponentModel.DataAnnotations;

namespace SoundScape.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public decimal Duration { get; set; }  // Використовуємо decimal для тривалості
    }

}
