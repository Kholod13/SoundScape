public class Song
{
    public int Id { get; set; }

    // Модифікатор required вказує, що властивості повинні бути обов'язково ініціалізовані
    public required string Title { get; set; }
    public required string Artist { get; set; }
    public required string Genre { get; set; }
    public required string FilePath { get; set; }

    public int DurationInSeconds { get; set; }
}
