namespace UpriseMidLevel.Models
{
    public class User
    {
        required public int Id { get; set; } // ide li required svuda?
        required public string Uuid { get; set; }
        required public string Email { get; set; }
        required public string Password { get; set; } // hashed, salted
    }
}
