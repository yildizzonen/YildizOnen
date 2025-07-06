using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Yildiz.Controllers;
using Yildiz.Models;

namespace TestProje
{
    public class CikisControllerTests
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
            var cikis = new Cikis { SanatEseri = eser, CikisTarihi = DateTime.Now };
            context.Cikislar.Add(cikis);
            context.SaveChanges();
            return context;
        }

        private Mock<ILogger<CikisController>> GetLoggerMock() => new Mock<ILogger<CikisController>>();

        [Fact]
        public async Task Listele_ReturnsViewWithCikislar()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);

            var result = await controller.Listele() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Cikis>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Detay_ReturnsViewWithCikis_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            var cikis = context.Cikislar.Include(c => c.SanatEseri).First();

            var result = await controller.Detay(cikis.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Cikis>(result.Model);
            Assert.Equal(cikis.Id, model.Id);
        }

        [Fact]
        public async Task Detay_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);

            var result = await controller.Detay(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Ekle_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);

            var result = controller.Ekle() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Ekle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            var eser = context.SanatEserleri.First();
            var model = new Cikis { SanatEseri = eser, CikisTarihi = DateTime.Now };

            var result = await controller.Ekle(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Equal(2, context.Cikislar.Count());
        }

        [Fact]
        public async Task Ekle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            controller.ModelState.AddModelError("CikisTarihi", "Zorunlu alan");
            var eser = context.SanatEserleri.First();
            var model = new Cikis { SanatEseri = eser };

            var result = await controller.Ekle(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsViewWithCikis_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            var cikis = context.Cikislar.First();

            var result = await controller.Duzenle(cikis.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Cikis>(result.Model);
            Assert.Equal(cikis.Id, model.Id);
        }

        [Fact]
        public async Task Duzenle_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);

            var result = await controller.Duzenle(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Duzenle_Post_ValidModel_RedirectsToListele()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            var cikis = context.Cikislar.Include(c => c.SanatEseri).First();
            cikis.CikisTarihi = DateTime.Now.AddDays(1);

            var result = await controller.Duzenle(cikis.Id, cikis) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
        }

        [Fact]
        public async Task Duzenle_Post_InvalidModel_ReturnsViewWithModel()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            var cikis = context.Cikislar.Include(c => c.SanatEseri).First();
            controller.ModelState.AddModelError("CikisTarihi", "Zorunlu alan");

            var result = await controller.Duzenle(cikis.Id, cikis) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(cikis, result.Model);
        }

        [Fact]
        public async Task Duzenle_Post_IdMismatch_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            var cikis = context.Cikislar.First();

            var result = await controller.Duzenle(9999, cikis);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Sil_Get_ReturnsViewWithCikis_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            var cikis = context.Cikislar.First();

            var result = await controller.Sil(cikis.Id) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Cikis>(result.Model);
            Assert.Equal(cikis.Id, model.Id);
        }

        [Fact]
        public async Task Sil_Get_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);

            var result = await controller.Sil(9999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SilOnay_Post_DeletesAndRedirects()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);
            var cikis = context.Cikislar.First();

            var result = await controller.SilOnay(cikis.Id) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Listele", result.ActionName);
            Assert.Empty(context.Cikislar.ToList());
        }

        [Fact]
        public async Task SilOnay_Post_ReturnsNotFound_WhenNotExists()
        {
            var context = GetInMemoryDbContext();
            var logger = GetLoggerMock();
            var controller = new CikisController(context, logger.Object);

            var result = await controller.SilOnay(9999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
} 