using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Controllers
{
    public class SanatEseriController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<SanatEseriController> _logger;

        public SanatEseriController(UygulamaDbContext context, ILogger<SanatEseriController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Tüm kullanıcılar görebilir
        public async Task<IActionResult> Listele()
        {
            var eserler = await _context.SanatEserleri.Include(e => e.Sanatci).ToListAsync();
            return View(eserler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var eser = await _context.SanatEserleri.Include(e => e.Sanatci).FirstOrDefaultAsync(e => e.Id == id);
            if (eser == null) return NotFound();
            return View(eser);
        }

        // Sadece Admin
        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(SanatEseri model)
        {
            if (ModelState.IsValid)
            {
                _context.SanatEserleri.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Sanat eseri eklendi: {model.Baslik} (ID: {model.Id})");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var eser = await _context.SanatEserleri.FindAsync(id);
            if (eser == null) return NotFound();
            return View(eser);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, SanatEseri model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Sanat eseri güncellendi: {model.Baslik} (ID: {model.Id})");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var eser = await _context.SanatEserleri.FindAsync(id);
            if (eser == null) return NotFound();
            return View(eser);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SilOnay(int id)
        {
            var eser = await _context.SanatEserleri.FindAsync(id);
            if (eser == null) return NotFound();
            _context.SanatEserleri.Remove(eser);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Sanat eseri silindi: {eser.Baslik} (ID: {eser.Id})");
            return RedirectToAction("Listele");
        }
    }
} 