using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yildiz.Models;

namespace Yildiz.Controllers
{
    public class IslemController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly ILogger<IslemController> _logger;

        public IslemController(UygulamaDbContext context, ILogger<IslemController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Listele()
        {
            var islemler = await _context.Islemler.Include(i => i.SanatEseri).ToListAsync();
            return View(islemler);
        }

        public async Task<IActionResult> Detay(int id)
        {
            var islem = await _context.Islemler.Include(i => i.SanatEseri).FirstOrDefaultAsync(i => i.Id == id);
            if (islem == null) return NotFound();
            return View(islem);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Ekle(Islem model)
        {
            if (ModelState.IsValid)
            {
                _context.Islemler.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"İşlem eklendi: {model.Id}");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id)
        {
            var islem = await _context.Islemler.FindAsync(id);
            if (islem == null) return NotFound();
            return View(islem);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duzenle(int id, Islem model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"İşlem güncellendi: {model.Id}");
                return RedirectToAction("Listele");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Sil(int id)
        {
            var islem = await _context.Islemler.FindAsync(id);
            if (islem == null) return NotFound();
            return View(islem);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SilOnay(int id)
        {
            var islem = await _context.Islemler.FindAsync(id);
            if (islem == null) return NotFound();
            _context.Islemler.Remove(islem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"İşlem silindi: {islem.Id}");
            return RedirectToAction("Listele");
        }
    }
} 