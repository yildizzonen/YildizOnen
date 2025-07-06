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
    public class GaleriKonumControllerTests
    {
        private UygulamaDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<UygulamaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new UygulamaDbContext(options);
            var konum = new GaleriKonum { Isim = "Test Galeri", Adres = "Test Adres" };
            context.GaleriKonumlari.Add(konum);
            context.SaveChanges();
            return context;
        }

        private Mock<ILogger<GaleriKonumController>> GetLoggerMock() => new Mock<ILogger<GaleriKonumController>>();

        [Fact]
        public async Task Listele_ReturnsViewWithKonumlar()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);

            var result = await controller.Listele() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<GaleriKonum>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Detay_ReturnsViewWithKonum_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);
            var konum = context.GaleriKonumlari.First();

            var result = await controller.Detay(konum.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<GaleriKonum>(result.Model);
            Assert.Equal(konum.Id, model.Id);
        }

        [Fact]
        public async Task Detay_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);

            var result = await controller.Detay(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Ekle_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);

            var result = controller.Ekle() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Ekle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);
            var model = new GaleriKonum { Isim = "Yeni Galeri", Adres = "Yeni Adres" };

            var result = await controller.Ekle(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Equal(2, context.GaleriKonumlari.Count());
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsViewWithKonum_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);
            var konum = context.GaleriKonumlari.First();

            var result = await controller.Duzenle(konum.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<GaleriKonum>(result.Model);
            Assert.Equal(konum.Id, model.Id);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);

            var result = await controller.Duzenle(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Duzenle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);
            var konum = context.GaleriKonumlari.First();
            konum.Adres = "Güncellenmiş Adres";

            var result = await controller.Duzenle(konum.Id, konum) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
        }

        [Fact]
        public async Task Duzenle_Post_IdMismatch_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);
            var konum = context.GaleriKonumlari.First();

            var result = await controller.Duzenle(9999, konum);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Sil_Get_ReturnsViewWithKonum_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);
            var konum = context.GaleriKonumlari.First();

            var result = await controller.Sil(konum.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<GaleriKonum>(result.Model);
            Assert.Equal(konum.Id, model.Id);
        }

        [Fact]
        public async Task Sil_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);

            var result = await controller.Sil(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SilOnay_Post_DeletesAndRedirects()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);
            var konum = context.GaleriKonumlari.First();

            var result = await controller.SilOnay(konum.Id) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Empty(context.GaleriKonumlari.ToList());
        }

        [Fact]
        public async Task SilOnay_Post_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new GaleriKonumController(context, logger.Object);

            var result = await controller.SilOnay(9999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
} 
 