using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Controllers
{
    public class UlkeController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<UlkeController> _logger;

        public UlkeController(UygulamaDbContext context, ILogger<UlkeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Listele()
        {
            var ulkeler = await _context.Ulkeler.ToListAsync();
            return View(ulkeler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var ulke = await _context.Ulkeler.FirstOrDefaultAsync(u => u.Id == id);
            if (ulke == null) return NotFound();
            return View(ulke);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(Ulke model)
        {
            if (ModelState.IsValid)
            {
                _context.Ulkeler.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Ülke eklendi: {model.Isim} (ID: {model.Id})");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var ulke = await _context.Ulkeler.FindAsync(id);
            if (ulke == null) return NotFound();
            return View(ulke);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Ulke model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Ülke güncellendi: {model.Isim} (ID: {model.Id})");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var ulke = await _context.Ulkeler.FindAsync(id);
            if (ulke == null) return NotFound();
            return View(ulke);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SilOnay(int id)
        {
            var ulke = await _context.Ulkeler.FindAsync(id);
            if (ulke == null) return NotFound();
            _context.Ulkeler.Remove(ulke);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Ülke silindi: {ulke.Isim} (ID: {ulke.Id})");
            return RedirectToAction("Listele");
        }
    }
} 