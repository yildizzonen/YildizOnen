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
    public class SanatEseriControllerTests
    {
        private UygulamaDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<UygulamaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new UygulamaDbContext(options);
            var ulke = new Ulke { Isim = "Türkiye" };
            context.Ulkeler.Add(ulke);
            var sanatci = new Sanatci { Isim = "Test Sanatçı", DogumTarihi = new DateTime(1980, 1, 1), Uyruk = ulke };
            context.Sanatcilar.Add(sanatci);
            var eser = new SanatEseri { Baslik = "Test Eser", Yil = 2020, Aciklama = "Test Açıklama", Resim = "test.jpg", Sanatci = sanatci };
            context.SanatEserleri.Add(eser);
            context.SaveChanges();
            return context;
        }

        private Mock<ILogger<SanatEseriController>> GetLoggerMock() => new Mock<ILogger<SanatEseriController>>();

        [Fact]
        public async Task Listele_ReturnsViewWithEserler()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);

            var result = await controller.Listele() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<SanatEseri>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Detay_ReturnsViewWithEser_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            var eser = context.SanatEserleri.Include(e => e.Sanatci).First();

            var result = await controller.Detay(eser.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<SanatEseri>(result.Model);
            Assert.Equal(eser.Id, model.Id);
        }

        [Fact]
        public async Task Detay_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);

            var result = await controller.Detay(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Ekle_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);

            var result = controller.Ekle() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Ekle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            var sanatci = context.Sanatcilar.First();
            var model = new SanatEseri { Baslik = "Yeni Eser", Yil = 2021, Aciklama = "Yeni Açıklama", Resim = "yeni.jpg", Sanatci = sanatci };

            var result = await controller.Ekle(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Equal(2, context.SanatEserleri.Count());
        }

        [Fact]
        public async Task Ekle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            controller.ModelState.AddModelError("Baslik", "Zorunlu alan");
            var sanatci = context.Sanatcilar.First();
            var model = new SanatEseri { Yil = 2021, Aciklama = "Eksik Eser", Resim = "eksik.jpg", Sanatci = sanatci };

            var result = await controller.Ekle(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsViewWithEser_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            var eser = context.SanatEserleri.First();

            var result = await controller.Duzenle(eser.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<SanatEseri>(result.Model);
            Assert.Equal(eser.Id, model.Id);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);

            var result = await controller.Duzenle(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Duzenle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            var eser = context.SanatEserleri.Include(e => e.Sanatci).First();
            eser.Baslik = "Güncellenmiş Eser";

            var result = await controller.Duzenle(eser.Id, eser) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
        }

        [Fact]
        public async Task Duzenle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            var eser = context.SanatEserleri.Include(e => e.Sanatci).First();
            controller.ModelState.AddModelError("Baslik", "Zorunlu alan");

            var result = await controller.Duzenle(eser.Id, eser) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(eser, result.Model);
        }

        [Fact]
        public async Task Duzenle_Post_IdMismatch_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            var eser = context.SanatEserleri.First();

            var result = await controller.Duzenle(9999, eser);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Sil_Get_ReturnsViewWithEser_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            var eser = context.SanatEserleri.First();

            var result = await controller.Sil(eser.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<SanatEseri>(result.Model);
            Assert.Equal(eser.Id, model.Id);
        }

        [Fact]
        public async Task Sil_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);

            var result = await controller.Sil(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SilOnay_Post_DeletesAndRedirects()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);
            var eser = context.SanatEserleri.First();

            var result = await controller.SilOnay(eser.Id) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Empty(context.SanatEserleri.ToList());
        }

        [Fact]
        public async Task SilOnay_Post_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatEseriController(context, logger.Object);

            var result = await controller.SilOnay(9999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
} 