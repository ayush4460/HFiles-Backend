using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest("Email already registered.");

            var newUser = new User
            {
                FullName = user.FullName,
                Email = user.Email,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash) // assuming raw password is in PasswordHash
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Start session
            HttpContext.Session.SetInt32("UserId", newUser.Id);

            return Ok(new { message = "User created", userId = newUser.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            // Set session
            HttpContext.Session.SetInt32("UserId", user.Id);

            return Ok(new { message = "Login successful", userId = user.Id });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully" });
        }

    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
