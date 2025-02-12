using System.ComponentModel.DataAnnotations;

namespace SoundScape.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Artist { get; set; }

        public string Album { get; set; }

        public decimal Duration { get; set; }  // Використовуємо decimal для тривалості

        public string FilePath { get; set; } // Додаємо шлях до файлу
    }
}
