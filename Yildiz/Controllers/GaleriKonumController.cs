using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Controllers
{
    public class GaleriKonumController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<GaleriKonumController> _logger;

        public GaleriKonumController(UygulamaDbContext context, ILogger<GaleriKonumController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Listele()
        {
            var konumlar = await _context.GaleriKonumlari.ToListAsync();
            return View(konumlar);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var konum = await _context.GaleriKonumlari.FirstOrDefaultAsync(k => k.Id == id);
            if (konum == null) return NotFound();
            return View(konum);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(GaleriKonum model)
        {
        
                _context.GaleriKonumlari.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Galeri konumu eklendi: {model.Isim} (ID: {model.Id})");
                return RedirectToAction("Listele");
            
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var konum = await _context.GaleriKonumlari.FindAsync(id);
            if (konum == null) return NotFound();
            return View(konum);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, GaleriKonum model)
        {
            if (id != model.Id) return NotFound();

                _context.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Galeri konumu güncellendi: {model.Isim} (ID: {model.Id})");
                return RedirectToAction("Listele");
            
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var konum = await _context.GaleriKonumlari.FindAsync(id);
            if (konum == null) return NotFound();
            return View(konum);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SilOnay(int id)
        {
            var konum = await _context.GaleriKonumlari.FindAsync(id);
            if (konum == null) return NotFound();
            _context.GaleriKonumlari.Remove(konum);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Galeri konumu silindi: {konum.Isim} (ID: {konum.Id})");
            return RedirectToAction("Listele");
        }
    }
} 