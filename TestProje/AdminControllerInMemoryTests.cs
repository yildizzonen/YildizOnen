using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Yildiz.Controllers;
using Yildiz.Models;

namespace TestProje
{
    public class AdminControllerInMemoryTests
    {
        private UygulamaDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<UygulamaDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var context = new UygulamaDbContext(options);

            var kullanici1 = new Kullanici
            {
                UserName = "test1@test.com",
                Email = "test1@test.com",
                Ad = "Test",
                Soyad = "Kullan?c?1",
                EmailConfirmed = true
            };
            var kullanici2 = new Kullanici
            {
                UserName = "test2@test.com",
                Email = "test2@test.com",
                Ad = "Test",
                Soyad = "Kullan?c?2",
                EmailConfirmed = true
            };
            context.Users.AddRange(kullanici1, kullanici2);

            var ulke = new Ulke { Isim = "Türkiye" };
            context.Ulkeler.Add(ulke);

            var sanatci = new Sanatci
            {
                Isim = "Test Sanatç?",
                DogumTarihi = new DateTime(1980, 1, 1),
                Uyruk = ulke
            };
            context.Sanatcilar.Add(sanatci);

            var eser1 = new SanatEseri
            {
                Baslik = "Test Eser 1",
                Yil = 2020,
                Aciklama = "Test aç?klama 1",
                Resim = "test1.jpg",
                Sanatci = sanatci
            };
            var eser2 = new SanatEseri
            {
                Baslik = "Test Eser 2",
                Yil = 2021,
                Aciklama = "Test aç?klama 2",
                Resim = "test2.jpg",
                Sanatci = sanatci
            };
            var eser3 = new SanatEseri
            {
                Baslik = "Test Eser 3",
                Yil = 2022,
                Aciklama = "Test aç?klama 3",
                Resim = "test3.jpg",
                Sanatci = sanatci
            };
            context.SanatEserleri.AddRange(eser1, eser2, eser3);

            var galeriKonum = new GaleriKonum
            {
                Isim = "Test Galeri",
                Adres = "Test Adres"
            };
            context.GaleriKonumlari.Add(galeriKonum);

            var sergi = new Sergi
            {
                Baslik = "Test Sergi",
                BaslangicTarihi = DateTime.Now.AddDays(-10),
                BitisTarihi = DateTime.Now.AddDays(10),
                Konum = galeriKonum
            };
            context.Sergiler.Add(sergi);

            var islem = new Islem
            {
                IslemTarihi = DateTime.Now,
                Aciklama = "Test i?lem",
                GirisMi = true,
                SanatEseri = eser1
            };
            context.Islemler.Add(islem);

            var giris = new Giris
            {
                SanatEseri = eser1,
                GirisTarihi = DateTime.Now.AddDays(-5)
            };
            context.Girisler.Add(giris);

            var cikis = new Cikis
            {
                SanatEseri = eser2,
                CikisTarihi = DateTime.Now
            };
            context.Cikislar.Add(cikis);

            context.SaveChanges();
            return context;
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            var context = GetInMemoryDbContext();
            var controller = new AdminController(context);

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Dashboard_SetsViewBagCountsAndReturnsView()
        {
            var context = GetInMemoryDbContext();
            var controller = new AdminController(context);

            var result = controller.Dashboard() as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(2, controller.ViewBag.KullaniciSayisi);
            Assert.Equal(1, controller.ViewBag.SanatciSayisi);
            Assert.Equal(3, controller.ViewBag.EserSayisi);
            Assert.Equal(1, controller.ViewBag.SergiSayisi);
            Assert.Equal(1, controller.ViewBag.IslemSayisi);
            Assert.Equal(1, controller.ViewBag.GirisSayisi);
            Assert.Equal(1, controller.ViewBag.CikisSayisi);
            Assert.Equal(1, controller.ViewBag.UlkeSayisi);
            Assert.Equal(1, controller.ViewBag.GaleriKonumSayisi);
        }
    }
}