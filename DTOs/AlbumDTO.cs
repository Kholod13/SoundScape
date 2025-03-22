using System.ComponentModel.DataAnnotations;

public class AlbumDTO
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Range(1900, 2100)] // Пример валидации для года выпуска
    public int Year { get; set; }

    // Список ID жанров
    public List<int> GenreIds { get; set; } = new List<int>();

    // Список ID артистов
    public List<int> ArtistIds { get; set; } = new List<int>();

    // Поле для изображения альбома (можно сделать его необязательным)
    public string? CoverUrl { get; set; }  // Додано поле для збереження URL зображення
}
