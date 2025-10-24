using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System;
using contract_monthly__claim_system_prototype_part2.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using contract_monthly__claim_system_prototype_part2.Models;
using contract_monthly__claim_system_prototype_part2.Hubs;

namespace contract_monthly__claim_system_prototype_part2.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<ClaimHub> _hub;

        private readonly string[] permittedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
        private const long FileSizeLimit = 5 * 1024 * 1024;// 5MB

        public ClaimsController(ApplicationDBContext db, IWebHostEnvironment env, IHubContext<ClaimHub> hub)
        {
            _db = db; _env = env; _hub = hub;
        }
        [HttpGet]
        public IActionResult Submit() => View(new Claim());
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Claim model, IFormFile upload)
        {
            if (!ModelState.IsValid) return View(model);
            if (upload != null)
            {
                if (upload.Length > FileSizeLimit)
                {
                    ModelState.AddModelError("", "File too larger.Max 5 MB.");
                    return View(model);
                }
                var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
                if (!permittedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("", "File type not permitted.");
                    return View(model);
                }
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var filename = $"{Guid.NewGuid()}{ext}";
                var filepath = Path.Combine(uploads, filename);
                using (var fs = System.IO.File.Create(filepath))
                {
                    await upload.CopyToAsync(fs);
                }
                model.UploadedFileName = filename;
            }
            model.DateSubmitted = DateTime.UtcNow;
            model.Status = ClaimStatus.Pending;
            model.lecturerId = User?.Identity?.Name ?? "anonymous";

            _db.Claims.Add(model);
            await _db.SaveChangesAsync();

            //notify admins (real time)
            await _hub.Clients.Group("Admins").SendAsync("NewClaim", model.Id);
            return RedirectToAction("Details", new { id = model.Id });
        }
        [HttpPost]
        public async Task<IActionResult> Details(int id)
        {
        var claim = await _db.Claims.FindAsync(id);
            if (claim == null) return NotFound();
            return View(claim);
            
            }


    }
}


