using Microsoft.AspNetCore.Mvc;
using LTUDAPI.Data;
using LTUDAPI.Models;

namespace LTUDAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class DanhGiaController : ControllerBase {
        private readonly ApplicationDbContext _context;
        public DanhGiaController(ApplicationDbContext context) { _context = context; }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DanhGia dg) {
            dg.CreatedAt = DateTime.Now;
            _context.DanhGias.Add(dg);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã lưu đánh giá!" });
        }
    }
}