using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Controllers
{
    public class GirisController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<GirisController> _logger;

        public GirisController(UygulamaDbContext context, ILogger<GirisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Listele()
        {
            var girisler = await _context.Girisler.Include(g => g.SanatEseri).ToListAsync();
            return View(girisler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var giris = await _context.Girisler.Include(g => g.SanatEseri).FirstOrDefaultAsync(g => g.Id == id);
            if (giris == null) return NotFound();
            return View(giris);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(Giris model)
        {
            if (ModelState.IsValid)
            {
                _context.Girisler.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Giriş eklendi: {model.Id}");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var giris = await _context.Girisler.FindAsync(id);
            if (giris == null) return NotFound();
            return View(giris);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Giris model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Giriş güncellendi: {model.Id}");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var giris = await _context.Girisler.FindAsync(id);
            if (giris == null) return NotFound();
            return View(giris);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SilOnay(int id)
        {
            var giris = await _context.Girisler.FindAsync(id);
            if (giris == null) return NotFound();
            _context.Girisler.Remove(giris);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Giriş silindi: {giris.Id}");
            return RedirectToAction("Listele");
        }
    }
} 