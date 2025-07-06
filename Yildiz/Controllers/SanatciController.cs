using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Controllers
{
    public class SanatciController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<SanatciController> _logger;

        public SanatciController(UygulamaDbContext context, ILogger<SanatciController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Tüm kullanıcılar görebilir
        public async Task<IActionResult> Listele()
        {
            var sanatcilar = await _context.Sanatcilar.Include(s => s.Uyruk).ToListAsync();
            return View(sanatcilar);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var sanatci = await _context.Sanatcilar.Include(s => s.Uyruk).FirstOrDefaultAsync(s => s.Id == id);
            if (sanatci == null) return NotFound();
            return View(sanatci);
        }

        // Sadece Admin
        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(Sanatci model)
        {
            if (ModelState.IsValid)
            {
                _context.Sanatcilar.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Sanatçı eklendi: {model.Isim} (ID: {model.Id})");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var sanatci = await _context.Sanatcilar.FindAsync(id);
            if (sanatci == null) return NotFound();
            return View(sanatci);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Sanatci model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Sanatçı güncellendi: {model.Isim} (ID: {model.Id})");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var sanatci = await _context.Sanatcilar.FindAsync(id);
            if (sanatci == null) return NotFound();
            return View(sanatci);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SilOnay(int id)
        {
            var sanatci = await _context.Sanatcilar.FindAsync(id);
            if (sanatci == null) return NotFound();
            _context.Sanatcilar.Remove(sanatci);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Sanatçı silindi: {sanatci.Isim} (ID: {sanatci.Id})");
            return RedirectToAction("Listele");
        }
    }
} 