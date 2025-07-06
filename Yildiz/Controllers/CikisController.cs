using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Controllers
{
    public class CikisController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<CikisController> _logger;

        public CikisController(UygulamaDbContext context, ILogger<CikisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Listele()
        {
            var cikislar = await _context.Cikislar.Include(c => c.SanatEseri).ToListAsync();
            return View(cikislar);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var cikis = await _context.Cikislar.Include(c => c.SanatEseri).FirstOrDefaultAsync(c => c.Id == id);
            if (cikis == null) return NotFound();
            return View(cikis);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(Cikis model)
        {
            if (ModelState.IsValid)
            {
                _context.Cikislar.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Çıkış eklendi: {model.Id}");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var cikis = await _context.Cikislar.FindAsync(id);
            if (cikis == null) return NotFound();
            return View(cikis);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Cikis model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Çıkış güncellendi: {model.Id}");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var cikis = await _context.Cikislar.FindAsync(id);
            if (cikis == null) return NotFound();
            return View(cikis);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SilOnay(int id)
        {
            var cikis = await _context.Cikislar.FindAsync(id);
            if (cikis == null) return NotFound();
            _context.Cikislar.Remove(cikis);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Çıkış silindi: {cikis.Id}");
            return RedirectToAction("Listele");
        }
    }
} 