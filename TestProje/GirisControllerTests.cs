using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Yildiz.Controllers;
using Yildiz.Models;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace TestProje
{
    public class GirisControllerTests
    {
        private UygulamaDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<UygulamaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new UygulamaDbContext(options);
            var ulke = new Ulke { Isim = "Türkiye" };
            context.Ulkeler.Add(ulke);
            var sanatci = new Sanatci { Isim = "Sanatçı 1", DogumTarihi = new DateTime(1980, 1, 1), Uyruk = ulke };
            context.Sanatcilar.Add(sanatci);
            var eser = new SanatEseri { Baslik = "Eser 1", Yil = 2020, Aciklama = "Açıklama", Resim = "resim.jpg", Sanatci = sanatci };
            context.SanatEserleri.Add(eser);
            var giris = new Giris { SanatEseri = eser, GirisTarihi = DateTime.Now };
            context.Girisler.Add(giris);
            context.SaveChanges();
            return context;
        }

        private Mock<ILogger<GirisController>> GetLoggerMock() => new Mock<ILogger<GirisController>>();

        [Fact]
        public async Task Listele_ReturnsViewWithGirisler()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);

            var result = await controller.Listele() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Giris>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Detay_ReturnsViewWithGiris_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            var giris = context.Girisler.Include(g => g.SanatEseri).First();

            var result = await controller.Detay(giris.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Giris>(result.Model);
            Assert.Equal(giris.Id, model.Id);
        }

        [Fact]
        public async Task Detay_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);

            var result = await controller.Detay(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Ekle_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);

            var result = controller.Ekle() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Ekle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            var eser = context.SanatEserleri.First();
            var model = new Giris { SanatEseri = eser, GirisTarihi = DateTime.Now };

            var result = await controller.Ekle(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Equal(2, context.Girisler.Count());
        }

        [Fact]
        public async Task Ekle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            controller.ModelState.AddModelError("GirisTarihi", "Zorunlu alan");
            var eser = context.SanatEserleri.First();
            var model = new Giris { SanatEseri = eser };

            var result = await controller.Ekle(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsViewWithGiris_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            var giris = context.Girisler.First();

            var result = await controller.Duzenle(giris.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Giris>(result.Model);
            Assert.Equal(giris.Id, model.Id);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);

            var result = await controller.Duzenle(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Duzenle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            var giris = context.Girisler.Include(g => g.SanatEseri).First();
            giris.GirisTarihi = DateTime.Now.AddDays(1);

            var result = await controller.Duzenle(giris.Id, giris) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
        }

        [Fact]
        public async Task Duzenle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            var giris = context.Girisler.Include(g => g.SanatEseri).First();
            controller.ModelState.AddModelError("GirisTarihi", "Zorunlu alan");

            var result = await controller.Duzenle(giris.Id, giris) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(giris, result.Model);
        }

        [Fact]
        public async Task Duzenle_Post_IdMismatch_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            var giris = context.Girisler.First();

            var result = await controller.Duzenle(9999, giris);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Sil_Get_ReturnsViewWithGiris_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            var giris = context.Girisler.First();

            var result = await controller.Sil(giris.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Giris>(result.Model);
            Assert.Equal(giris.Id, model.Id);
        }

        [Fact]
        public async Task Sil_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);

            var result = await controller.Sil(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SilOnay_Post_DeletesAndRedirects()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);
            var giris = context.Girisler.First();

            var result = await controller.SilOnay(giris.Id) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Empty(context.Girisler.ToList());
        }

        [Fact]
        public async Task SilOnay_Post_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GirisController(context, logger.Object);

            var result = await controller.SilOnay(9999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
} 