using SoundScape.Models;

public class Order
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public User User { get; set; }
	public ICollection<Product> Products { get; set; } = new List<Product>();
}
