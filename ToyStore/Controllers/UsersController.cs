using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToyStore.Application.DTOs.User;
using ToyStore.Models;
using ToyStore.Repositories;
using ToyStore.Application.Common.Exceptions;

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IConfiguration _configuration; // JWT ayarlarını oxumaq üçün

        public UsersController(IGenericRepository<User> userRepo, IConfiguration configuration)
        {
            _userRepo = userRepo;
            _configuration = configuration;
        }

        // 1. READ ALL (CRUD-un R hissəsi - Çatışmırdı)
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepo.GetAllAsync();
            var result = users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                CreatedAt = u.Created_at
            }).ToList();

            return Ok(result);
        }

        // 2. READ BY ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) throw new NotFoundException(nameof(User), id);

            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                CreatedAt = user.Created_at
            });
        }

        // 3. CREATE (Register)
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserCreateDto request)
        {
            var allUsers = await _userRepo.GetAllAsync();
            if (allUsers.Any(u => u.Email == request.Email))
                throw new BadRequestException("Bu email artıq qeydiyyatlıdır.");

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Password_hash = request.Password, // Diqqət: Gələcəkdə bura Hashing əlavə edəcəyik
                Created_at = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new { user.Id, Message = "İstifadəçi uğurla yaradıldı" });
        }

        // 4. LOGIN (Kimlik doğrulaması və Token qaytarılması)
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var allUsers = await _userRepo.GetAllAsync();
            var user = allUsers.FirstOrDefault(u => u.Email == request.Email && u.Password_hash == request.Password);

            if (user == null)
                throw new BadRequestException("Email və ya şifrə yanlışdır.");

            // JWT Token yaradılması
            var token = GenerateJwtToken(user);

            return Ok(new { Token = token, Message = "Giriş uğurludur" });
        }

        // 5. UPDATE
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(UserUpdateDto request)
        {
            var user = await _userRepo.GetByIdAsync(request.Id);
            if (user == null) throw new NotFoundException(nameof(User), request.Id);

            user.Name = request.Name;
            user.Phone = request.Phone;

            await _userRepo.SaveChangesAsync();
            return NoContent();
        }

        // 6. DELETE (CRUD-un D hissəsi - Çatışmırdı)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) throw new NotFoundException(nameof(User), id);

            _userRepo.Delete(user);
            await _userRepo.SaveChangesAsync();
            return NoContent();
        }

        // Token yaradan köməkçi metod
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, "User") // Rolları gələcəkdə dinamik edə bilərsən
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3), // 3 saatlıq token
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}