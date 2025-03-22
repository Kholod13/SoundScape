public class SongDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Duration { get; set; }  // Duration теперь строка
    public int AlbumId { get; set; }
    public string AlbumTitle { get; set; }
    public int? AlbumYear { get; set; }  // Рік альбому
    public string AlbumCoverUrl { get; set; }  // URL обкладинки альбому
    public string FilePath { get; set; }
    public string CoverImageUrl { get; set; } // Для обкладинки

    public List<string> Artists { get; set; }  // Список артистов
    public string Lyrics { get; set; }


}
