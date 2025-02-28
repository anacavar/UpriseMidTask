using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpriseMidTask.Controllers.Data;

namespace UpriseMidLevel.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            try
            {
                await _context.Database.OpenConnectionAsync();
                return Ok("✅ Database connection successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during manual connection: {ex.Message}");
                return StatusCode(500, $"❌ Error: {ex.Message}");
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        //// register
        //[HttpPost("register")]
        //public IActionResult Register([FromBody] User user)
        //{
        //    // check if user exists
        //    // hash password
        //    // save user to db
        //    // return user
        //}
        //// login
        //[HttpPost("login")]
        //public IActionResult Login([FromBody] User user)
        //{
        //    // check if user exists
        //    // check if password is correct
        //    // return JWT
        //}
        //// auth using JWT
    }
}
