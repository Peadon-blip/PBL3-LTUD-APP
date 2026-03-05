using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReminderAPI.Data;
using ReminderAPI.Models;
using ReminderAPI.DTOs;

namespace ReminderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReminderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách cấu hình nhắc lịch của tất cả người dùng (Admin)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReminderConfig>>> GetReminders()
        {
            return await _context.ReminderConfigs.ToListAsync();
        }

        // 2. Lấy cấu hình nhắc lịch của 1 tài khoản cụ thể
        [HttpGet("{idAcc}")]
        public async Task<ActionResult<ReminderConfig>> GetReminderByAccount(long idAcc)
        {
            var config = await _context.ReminderConfigs
                .FirstOrDefaultAsync(r => r.IdAcc == idAcc);

            if (config == null) return NotFound("Không tìm thấy cấu hình cho tài khoản này.");
            return config;
        }

        // 3. Thêm hoặc cập nhật cấu hình nhắc lịch
        [HttpPost]
        public async Task<ActionResult<ReminderConfig>> PostReminder(ReminderRequest request)
        {
            // Kiểm tra xem đã có cấu hình chưa
            var existingConfig = await _context.ReminderConfigs
                .FirstOrDefaultAsync(r => r.IdAcc == request.IdAcc);

            if (existingConfig != null)
            {
                // Nếu có rồi thì cập nhật
                existingConfig.MinsBefore = request.MinsBefore;
                existingConfig.IsEnabled = request.IsEnabled;
                existingConfig.Channel = request.Channel;
            }
            else
            {
                // Nếu chưa có thì tạo mới
                var newConfig = new ReminderConfig
                {
                    IdAcc = request.IdAcc,
                    MinsBefore = request.MinsBefore,
                    IsEnabled = request.IsEnabled,
                    Channel = request.Channel
                };
                _context.ReminderConfigs.Add(newConfig);
            }

            // Ghi log vào bảng USER_LOG theo file SQL của bạn
            var log = new UserLog
            {
                IdAcc = request.IdAcc,
                HanhDong = $"Cập nhật nhắc lịch: {request.MinsBefore} phút trước",
                ThoiGian = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            _context.UserLogs.Add(log);

            await _context.SaveChangesAsync();
            return Ok("Đã lưu cấu hình nhắc lịch thành công.");
        }

        // 4. Xóa cấu hình nhắc lịch
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(long id)
        {
            var config = await _context.ReminderConfigs.FindAsync(id);
            if (config == null) return NotFound();

            _context.ReminderConfigs.Remove(config);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}