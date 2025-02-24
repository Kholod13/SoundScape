public class AlbumDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public int Year { get; set; }
    public List<string> Artists { get; set; }
    public List<string> Songs { get; set; }
    public List<int> ArtistIds { get; set; }
}
