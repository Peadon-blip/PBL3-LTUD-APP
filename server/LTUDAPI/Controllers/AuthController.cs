using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTUDAPI.Data;
using LTUDAPI.Models;

namespace LTUDAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context) { _context = context; }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Tìm user trong DB (Trong SQLQuery2 bạn có pass là 'pass1', 'pass_sv'...)
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == request.Username && a.PasswordHash == request.Password);

            if (account == null) return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });

            // Lấy thông tin User (họ tên) để trả về cho Client
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdAcc == account.IdAcc);

            return Ok(new { 
                idAcc = account.IdAcc, 
                username = account.Username,
                fullName = user?.HoTen ?? "Người dùng",
                roleId = account.IdRole 
            });
        }
    }

    public class LoginRequest {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}