using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yildiz.Models;
using Microsoft.EntityFrameworkCore;

namespace Yildiz.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UygulamaDbContext _context;
        public AdminController(UygulamaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewBag.KullaniciSayisi = _context.Users.Count();
            ViewBag.SanatciSayisi = _context.Sanatcilar.Count();
            ViewBag.EserSayisi = _context.SanatEserleri.Count();
            ViewBag.SergiSayisi = _context.Sergiler.Count();
            ViewBag.IslemSayisi = _context.Islemler.Count();
            ViewBag.GirisSayisi = _context.Girisler.Count();
            ViewBag.CikisSayisi = _context.Cikislar.Count();
            ViewBag.UlkeSayisi = _context.Ulkeler.Count();
            ViewBag.GaleriKonumSayisi = _context.GaleriKonumlari.Count();
            return View();
        }
    }
} 