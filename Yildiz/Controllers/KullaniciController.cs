using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Yildiz.Models;
using Yildiz.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Yildiz.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly UserManager<Kullanici> _userManager;
        private readonly SignInManager<Kullanici> _signInManager;
        private readonly RoleManager<Rol> _roleManager;

        public KullaniciController(UserManager<Kullanici> userManager, SignInManager<Kullanici> signInManager, RoleManager<Rol> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult KayitOl()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KayitOl(KayitOlViewModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = new Kullanici { UserName = model.Eposta, Email = model.Eposta, Ad = model.Ad, Soyad = model.Soyad };
                var sonuc = await _userManager.CreateAsync(kullanici, model.Sifre);
                if (sonuc.Succeeded)
                {
                    await _userManager.AddToRoleAsync(kullanici, "Kullanici");
                    await _signInManager.SignInAsync(kullanici, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var hata in sonuc.Errors)
                {
                    ModelState.AddModelError("", hata.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GirisYap(GirisYapViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sonuc = await _signInManager.PasswordSignInAsync(model.Eposta, model.Sifre, model.BeniHatirla, false);
                if (sonuc.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Geçersiz giriş denemesi.");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> CikisYap()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
} 