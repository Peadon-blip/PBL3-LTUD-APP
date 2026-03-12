using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTUDAPI.Data;

namespace LTUDAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class GiangVienController : ControllerBase {
        private readonly ApplicationDbContext _context;
        public GiangVienController(ApplicationDbContext context) { _context = context; }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _context.GiangViens.ToListAsync());
    }
}