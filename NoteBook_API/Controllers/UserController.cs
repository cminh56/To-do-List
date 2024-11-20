using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NoteBook_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;
using NoteBook_API.DTO.UserDTO;
using NoteBook_API.DTO.UserDTO.NoteBook_API.Models;

namespace NoteBook_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PRN231_NoteContext _context;
        private readonly IConfiguration _configuration;

        public UserController(PRN231_NoteContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/User/signup
        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            // Check if the username or email already exists
            if (await _context.Users.AnyAsync(u =>  u.Email == userDto.Email))
            {
                return BadRequest("Username or Email already exists.");
            }

            // Create a new User object
            var user = new User
            {
                Username = userDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password), // Hash the password
                Email = userDto.Email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token for the new user
            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        // Method to generate a JWT token
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Check if the password matches
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized("Invalid password");
            }

            // Generate JWT token or any other required response here
            var token = GenerateJwtToken(user); // Assuming you have a method for this
            return Ok(new {userid=user.UserId , Token = token });
        }
    }
}
