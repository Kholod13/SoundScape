using System.ComponentModel.DataAnnotations;

namespace SoundScape.DTOs
{
    public class GenreDTO
    {
        public int? Id { get; set; } // Використовуємо int? (nullable) на випадок, якщо поле не буде задано при створенні

        [Required]
        public string Name { get; set; }
    }



}
