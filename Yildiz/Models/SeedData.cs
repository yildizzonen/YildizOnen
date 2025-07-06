using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Yildiz.Models
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new UygulamaDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<UygulamaDbContext>>());

            // Roller
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Rol>>();
            string[] roles = new[] { "Admin", "Kullanici" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Rol { Name = role });
                }
            }

            // Kullanıcılar
            var userManager = serviceProvider.GetRequiredService<UserManager<Kullanici>>();
            if (await userManager.FindByEmailAsync("admin@yildiz.com") == null)
            {
                var admin = new Kullanici { UserName = "admin@yildiz.com", Email = "admin@yildiz.com", Ad = "Admin", Soyad = "Yönetici", EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            if (await userManager.FindByEmailAsync("kullanici@yildiz.com") == null)
            {
                var user = new Kullanici { UserName = "kullanici@yildiz.com", Email = "kullanici@yildiz.com", Ad = "Ali", Soyad = "Veli", EmailConfirmed = true };
                await userManager.CreateAsync(user, "Kullanici123!");
                await userManager.AddToRoleAsync(user, "Kullanici");
            }

            // Ülkeler
            if (!context.Ulkeler.Any())
            {
                context.Ulkeler.AddRange(
                    new Ulke { Isim = "Türkiye" },
                    new Ulke { Isim = "Fransa" }
                );
                await context.SaveChangesAsync();
            }

            // Sanatçılar
            if (!context.Sanatcilar.Any())
            {
                var turkiye = context.Ulkeler.FirstOrDefault(u => u.Isim == "Türkiye");
                context.Sanatcilar.AddRange(
                    new Sanatci { Isim = "Ahmet Yılmaz", DogumTarihi = new DateTime(1980, 5, 10), Uyruk = turkiye }
                );
                await context.SaveChangesAsync();
            }

            // Sanat Eserleri
            if (!context.SanatEserleri.Any())
            {
                var sanatci = context.Sanatcilar.FirstOrDefault();
                context.SanatEserleri.AddRange(
                    new SanatEseri { Baslik = "Güneşin Doğuşu", Sanatci = sanatci, Yil = 2020, Aciklama = "Modern bir tablo.", Resim = "gunes.jpg" }
                );
                await context.SaveChangesAsync();
            }

            // Galeri Konumları
            if (!context.GaleriKonumlari.Any())
            {
                context.GaleriKonumlari.AddRange(
                    new GaleriKonum { Isim = "Merkez Galeri", Adres = "İstanbul" }
                );
                await context.SaveChangesAsync();
            }

            // Sergiler
            if (!context.Sergiler.Any())
            {
                var konum = context.GaleriKonumlari.FirstOrDefault();
                context.Sergiler.AddRange(
                    new Sergi { Baslik = "İlkbahar Sergisi", BaslangicTarihi = DateTime.Now.AddDays(-10), BitisTarihi = DateTime.Now.AddDays(10), Konum = konum }
                );
                await context.SaveChangesAsync();
            }

            // Giriş, Çıkış, İşlem örnekleri
            if (!context.Girisler.Any())
            {
                var eser = context.SanatEserleri.FirstOrDefault();
                context.Girisler.Add(new Giris { SanatEseri = eser, GirisTarihi = DateTime.Now.AddDays(-5) });
                await context.SaveChangesAsync();
            }
            if (!context.Cikislar.Any())
            {
                var eser = context.SanatEserleri.FirstOrDefault();
                context.Cikislar.Add(new Cikis { SanatEseri = eser, CikisTarihi = DateTime.Now });
                await context.SaveChangesAsync();
            }
            if (!context.Islemler.Any())
            {
                var eser = context.SanatEserleri.FirstOrDefault();
                context.Islemler.Add(new Islem { SanatEseri = eser, IslemTarihi = DateTime.Now, Aciklama = "Satış", GirisMi = false });
                await context.SaveChangesAsync();
            }
        }
    }
} 
 