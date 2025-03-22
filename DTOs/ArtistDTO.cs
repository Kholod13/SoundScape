public class ArtistDTO
{
    public int Id { get; set; }


    public string Name { get; set; }
    public string Bio { get; set; }  
    public string AvatarUrl { get; set; }  
    public List<AlbumDTO> Albums { get; set; }  
}
