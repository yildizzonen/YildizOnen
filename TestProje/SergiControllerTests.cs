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
    public class SergiControllerTests
    {
        private UygulamaDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<UygulamaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new UygulamaDbContext(options);
            var konum = new GaleriKonum { Isim = "Test Galeri", Adres = "Test Adres" };
            context.GaleriKonumlari.Add(konum);
            var sergi = new Sergi { Baslik = "Test Sergi", BaslangicTarihi = DateTime.Now.AddDays(-10), BitisTarihi = DateTime.Now.AddDays(10), Konum = konum };
            context.Sergiler.Add(sergi);
            context.SaveChanges();
            return context;
        }

        private Mock<ILogger<SergiController>> GetLoggerMock() => new Mock<ILogger<SergiController>>();

        [Fact]
        public async Task Listele_ReturnsViewWithSergiler()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);

            var result = await controller.Listele() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Sergi>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Detay_ReturnsViewWithSergi_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            var sergi = context.Sergiler.Include(s => s.Konum).First();

            var result = await controller.Detay(sergi.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Sergi>(result.Model);
            Assert.Equal(sergi.Id, model.Id);
        }

        [Fact]
        public async Task Detay_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);

            var result = await controller.Detay(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Ekle_Get_ReturnsViewWithKonumlar()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);

            var result = await controller.Ekle() as ViewResult;
            Assert.NotNull(result);
            Assert.NotNull(controller.ViewBag.Konumlar);
            var konumlar = Assert.IsAssignableFrom<List<GaleriKonum>>(controller.ViewBag.Konumlar);
            Assert.Single(konumlar);
        }

        [Fact]
        public async Task Ekle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            var konum = context.GaleriKonumlari.First();
            var model = new Sergi { Baslik = "Yeni Sergi", BaslangicTarihi = DateTime.Now, BitisTarihi = DateTime.Now.AddDays(30), Konum = konum };

            var result = await controller.Ekle(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Equal(2, context.Sergiler.Count());
        }

        [Fact]
        public async Task Ekle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            controller.ModelState.AddModelError("Baslik", "Zorunlu alan");
            var konum = context.GaleriKonumlari.First();
            var model = new Sergi { BaslangicTarihi = DateTime.Now, BitisTarihi = DateTime.Now.AddDays(30), Konum = konum };

            var result = await controller.Ekle(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsViewWithSergiAndKonumlar_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            var sergi = context.Sergiler.First();

            var result = await controller.Duzenle(sergi.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Sergi>(result.Model);
            Assert.Equal(sergi.Id, model.Id);
            Assert.NotNull(controller.ViewBag.Konumlar);
            var konumlar = Assert.IsAssignableFrom<List<GaleriKonum>>(controller.ViewBag.Konumlar);
            Assert.Single(konumlar);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);

            var result = await controller.Duzenle(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Duzenle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            var sergi = context.Sergiler.Include(s => s.Konum).First();
            sergi.Baslik = "Güncellenmiş Sergi";

            var result = await controller.Duzenle(sergi.Id, sergi) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
        }

        [Fact]
        public async Task Duzenle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            var sergi = context.Sergiler.Include(s => s.Konum).First();
            controller.ModelState.AddModelError("Baslik", "Zorunlu alan");

            var result = await controller.Duzenle(sergi.Id, sergi) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(sergi, result.Model);
        }

        [Fact]
        public async Task Duzenle_Post_IdMismatch_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            var sergi = context.Sergiler.First();

            var result = await controller.Duzenle(9999, sergi);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Sil_Get_ReturnsViewWithSergi_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            var sergi = context.Sergiler.First();

            var result = await controller.Sil(sergi.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Sergi>(result.Model);
            Assert.Equal(sergi.Id, model.Id);
        }

        [Fact]
        public async Task Sil_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);

            var result = await controller.Sil(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SilOnay_Post_DeletesAndRedirects()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);
            var sergi = context.Sergiler.First();

            var result = await controller.SilOnay(sergi.Id) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Empty(context.Sergiler.ToList());
        }

        [Fact]
        public async Task SilOnay_Post_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SergiController(context, logger.Object);

            var result = await controller.SilOnay(9999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
} 