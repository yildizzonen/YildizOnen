using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Controllers
{
    public class SergiController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<SergiController> _logger;

        public SergiController(UygulamaDbContext context, ILogger<SergiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Listele()
        {
            var sergiler = await _context.Sergiler.Include(s => s.Konum).ToListAsync();
            return View(sergiler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var sergi = await _context.Sergiler.Include(s => s.Konum).FirstOrDefaultAsync(s => s.Id == id);
            if (sergi == null) return NotFound();
            return View(sergi);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle()
        {
            ViewBag.Konumlar = await _context.GaleriKonumlari.ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(Sergi model)
        {
            if (ModelState.IsValid)
            {
                _context.Sergiler.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Sergi eklendi: {model.Baslik} (ID: {model.Id})");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var sergi = await _context.Sergiler.FindAsync(id);
            if (sergi == null) return NotFound();
            ViewBag.Konumlar = await _context.GaleriKonumlari.ToListAsync();
            return View(sergi);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Sergi model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Sergi güncellendi: {model.Baslik} (ID: {model.Id})");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var sergi = await _context.Sergiler.FindAsync(id);
            if (sergi == null) return NotFound();
            return View(sergi);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SilOnay(int id)
        {
            var sergi = await _context.Sergiler.FindAsync(id);
            if (sergi == null) return NotFound();
            _context.Sergiler.Remove(sergi);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Sergi silindi: {sergi.Baslik} (ID: {sergi.Id})");
            return RedirectToAction("Listele");
        }
    }
} 