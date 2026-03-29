using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.User;
using ToyStore.Models;
using ToyStore.Repositories;

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepo;

        public UsersController(IGenericRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                CreatedAt = user.Created_at
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserCreateDto request)
        {
            var allUsers = await _userRepo.GetAllAsync();
            if (allUsers.Any(u => u.Email == request.Email))
                return BadRequest(new { Message = "Bu email artıq qeydiyyatlıdır" });

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Password_hash = "HASH_SONRA_ELAVE_EDILECEK",
                Created_at = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            return Ok(new { user.Id, Message = "İstifadəçi yaradıldı" });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserUpdateDto request)
        {
            var user = await _userRepo.GetByIdAsync(request.Id);
            if (user == null) return NotFound();

            user.Name = request.Name;
            user.Phone = request.Phone;

            await _userRepo.SaveChangesAsync();
            return NoContent();
        }
    }
}