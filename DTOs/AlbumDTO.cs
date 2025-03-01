public class AlbumDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int Year { get; set; }

    // Список ID жанров
    public List<int> GenreIds { get; set; } = new List<int>();

    // Список ID артистов
    public List<int> ArtistIds { get; set; } = new List<int>();
}
