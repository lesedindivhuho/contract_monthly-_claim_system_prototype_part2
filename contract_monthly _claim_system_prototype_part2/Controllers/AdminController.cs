using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using contract_monthly__claim_system_prototype_part2.Data;
using contract_monthly__claim_system_prototype_part2.Models;
using contract_monthly__claim_system_prototype_part2.Hubs;

namespace contract_monthly__claim_system_prototype_part2.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly IHubContext<ClaimHub> _hub;

        public AdminController(ApplicationDBContext db,IHubContext<ClaimHub> hub)   
        {
            _db = db; _hub = hub;
        }
        [HttpGet("claims")]
        public async Task<IActionResult> Index()
        {
            var claims = await _db.Claims.OrderByDescending(c=> c.DateSubmitted).ToListAsync();
            return View(claims);
        }
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            claim.Status = ClaimStatus.Approved;
            await _db.SaveChangesAsync();

            await _hub.Clients.Group(claim.lecturerId).SendAsync("StatusUpdated", claim.Id, claim.Status.ToString());
            return Ok(new { success = true });
        }
    }
}
