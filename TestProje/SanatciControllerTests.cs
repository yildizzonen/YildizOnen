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
    public class SanatciControllerTests
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
            context.SaveChanges();
            return context;
        }

        private Mock<ILogger<SanatciController>> GetLoggerMock() => new Mock<ILogger<SanatciController>>();

        [Fact]
        public async Task Listele_ReturnsViewWithSanatcilar()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);

            var result = await controller.Listele() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Sanatci>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Detay_ReturnsViewWithSanatci_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            var sanatci = context.Sanatcilar.Include(s => s.Uyruk).First();

            var result = await controller.Detay(sanatci.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Sanatci>(result.Model);
            Assert.Equal(sanatci.Id, model.Id);
        }

        [Fact]
        public async Task Detay_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);

            var result = await controller.Detay(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Ekle_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);

            var result = controller.Ekle() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Ekle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            var ulke = context.Ulkeler.First();
            var model = new Sanatci { Isim = "Yeni Sanatçı", DogumTarihi = new DateTime(1990, 5, 15), Uyruk = ulke };

            var result = await controller.Ekle(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Equal(2, context.Sanatcilar.Count());
        }

        [Fact]
        public async Task Ekle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            controller.ModelState.AddModelError("Isim", "Zorunlu alan");
            var ulke = context.Ulkeler.First();
            var model = new Sanatci { DogumTarihi = new DateTime(1990, 5, 15), Uyruk = ulke };

            var result = await controller.Ekle(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsViewWithSanatci_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            var sanatci = context.Sanatcilar.First();

            var result = await controller.Duzenle(sanatci.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Sanatci>(result.Model);
            Assert.Equal(sanatci.Id, model.Id);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);

            var result = await controller.Duzenle(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Duzenle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            var sanatci = context.Sanatcilar.Include(s => s.Uyruk).First();
            sanatci.Isim = "Güncellenmiş Sanatçı";

            var result = await controller.Duzenle(sanatci.Id, sanatci) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
        }

        [Fact]
        public async Task Duzenle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            var sanatci = context.Sanatcilar.Include(s => s.Uyruk).First();
            controller.ModelState.AddModelError("Isim", "Zorunlu alan");

            var result = await controller.Duzenle(sanatci.Id, sanatci) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(sanatci, result.Model);
        }

        [Fact]
        public async Task Duzenle_Post_IdMismatch_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            var sanatci = context.Sanatcilar.First();

            var result = await controller.Duzenle(9999, sanatci);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Sil_Get_ReturnsViewWithSanatci_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            var sanatci = context.Sanatcilar.First();

            var result = await controller.Sil(sanatci.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Sanatci>(result.Model);
            Assert.Equal(sanatci.Id, model.Id);
        }

        [Fact]
        public async Task Sil_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);

            var result = await controller.Sil(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SilOnay_Post_DeletesAndRedirects()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);
            var sanatci = context.Sanatcilar.First();

            var result = await controller.SilOnay(sanatci.Id) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Empty(context.Sanatcilar.ToList());
        }

        [Fact]
        public async Task SilOnay_Post_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new SanatciController(context, logger.Object);

            var result = await controller.SilOnay(9999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
} 