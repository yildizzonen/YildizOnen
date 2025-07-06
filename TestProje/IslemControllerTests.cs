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
    public class IslemControllerTests
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
            var islem = new Islem { SanatEseri = eser, IslemTarihi = DateTime.Now, Aciklama = "Test İşlem", GirisMi = true };
            context.Islemler.Add(islem);
            context.SaveChanges();
            return context;
        }

        private Mock<ILogger<IslemController>> GetLoggerMock() => new Mock<ILogger<IslemController>>();

        [Fact]
        public async Task Listele_ReturnsViewWithIslemler()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);

            var result = await controller.Listele() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Islem>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Detay_ReturnsViewWithIslem_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            var islem = context.Islemler.Include(i => i.SanatEseri).First();

            var result = await controller.Detay(islem.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Islem>(result.Model);
            Assert.Equal(islem.Id, model.Id);
        }

        [Fact]
        public async Task Detay_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);

            var result = await controller.Detay(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Ekle_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);

            var result = controller.Ekle() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Ekle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            var eser = context.SanatEserleri.First();
            var model = new Islem { SanatEseri = eser, IslemTarihi = DateTime.Now, Aciklama = "Yeni İşlem", GirisMi = false };

            var result = await controller.Ekle(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Equal(2, context.Islemler.Count());
        }

        [Fact]
        public async Task Ekle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            controller.ModelState.AddModelError("IslemTarihi", "Zorunlu alan");
            var eser = context.SanatEserleri.First();
            var model = new Islem { SanatEseri = eser, Aciklama = "Eksik İşlem", GirisMi = true };

            var result = await controller.Ekle(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsViewWithIslem_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            var islem = context.Islemler.First();

            var result = await controller.Duzenle(islem.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Islem>(result.Model);
            Assert.Equal(islem.Id, model.Id);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);

            var result = await controller.Duzenle(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Duzenle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            var islem = context.Islemler.Include(i => i.SanatEseri).First();
            islem.Aciklama = "Güncellenmiş İşlem";

            var result = await controller.Duzenle(islem.Id, islem) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
        }

        [Fact]
        public async Task Duzenle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            var islem = context.Islemler.Include(i => i.SanatEseri).First();
            controller.ModelState.AddModelError("IslemTarihi", "Zorunlu alan");

            var result = await controller.Duzenle(islem.Id, islem) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(islem, result.Model);
        }

        [Fact]
        public async Task Duzenle_Post_IdMismatch_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            var islem = context.Islemler.First();

            var result = await controller.Duzenle(9999, islem);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Sil_Get_ReturnsViewWithIslem_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            var islem = context.Islemler.First();

            var result = await controller.Sil(islem.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Islem>(result.Model);
            Assert.Equal(islem.Id, model.Id);
        }

        [Fact]
        public async Task Sil_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);

            var result = await controller.Sil(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SilOnay_Post_DeletesAndRedirects()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);
            var islem = context.Islemler.First();

            var result = await controller.SilOnay(islem.Id) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Empty(context.Islemler.ToList());
        }

        [Fact]
        public async Task SilOnay_Post_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new IslemController(context, logger.Object);

            var result = await controller.SilOnay(9999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
} 