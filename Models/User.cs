using System.ComponentModel.DataAnnotations;

namespace UpriseMidLevel.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } 
        public string Uuid { get; set; } = Guid.NewGuid().ToString();
        required public string Email { get; set; }
        required public string Password { get; set; } // hashed, salted
    }
}
