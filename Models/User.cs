

public class User
{
    public int Id { get; set; }

    // Додавання модифікатора required для обов'язкових властивостей
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}
