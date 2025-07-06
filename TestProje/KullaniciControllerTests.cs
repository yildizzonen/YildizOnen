using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Yildiz.Controllers;
using Yildiz.Models;
using Yildiz.ViewModels;

namespace TestProje
{
    public class KullaniciControllerTests
    {
        private Mock<UserManager<Kullanici>> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<Kullanici>>();
            return new Mock<UserManager<Kullanici>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<SignInManager<Kullanici>> GetSignInManagerMock(UserManager<Kullanici> userManager)
        {
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Kullanici>>();
            return new Mock<SignInManager<Kullanici>>(userManager, contextAccessor.Object, claimsFactory.Object, null, null, null, null);
        }

        private Mock<RoleManager<Rol>> GetRoleManagerMock()
        {
            var store = new Mock<IRoleStore<Rol>>();
            return new Mock<RoleManager<Rol>>(store.Object, null, null, null, null);
        }

        [Fact]
        public void KayitOl_Get_ReturnsView()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);

            var result = controller.KayitOl() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task KayitOl_Post_ValidModel_SuccessfulRegistration_RedirectsToHome()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            userManager.Setup(x => x.CreateAsync(It.IsAny<Kullanici>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            userManager.Setup(x => x.AddToRoleAsync(It.IsAny<Kullanici>(), "Kullanici")).ReturnsAsync(IdentityResult.Success);
            signInManager.Setup(x => x.SignInAsync(It.IsAny<Kullanici>(), false, null)).Returns(Task.CompletedTask);
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);
            var model = new KayitOlViewModel { Eposta = "test@test.com", Ad = "Test", Soyad = "User", Sifre = "Password123!" };

            var result = await controller.KayitOl(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public async Task KayitOl_Post_ValidModel_FailedRegistration_ReturnsViewWithErrors()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Hata" });
            userManager.Setup(x => x.CreateAsync(It.IsAny<Kullanici>(), It.IsAny<string>())).ReturnsAsync(identityResult);
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);
            var model = new KayitOlViewModel { Eposta = "test@test.com", Ad = "Test", Soyad = "User", Sifre = "Password123!" };

            var result = await controller.KayitOl(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task KayitOl_Post_InvalidModel_ReturnsViewWithModel()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);
            controller.ModelState.AddModelError("Eposta", "Zorunlu alan");
            var model = new KayitOlViewModel();

            var result = await controller.KayitOl(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public void GirisYap_Get_ReturnsView()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);

            var result = controller.GirisYap() as ViewResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GirisYap_Post_ValidModel_SuccessfulLogin_RedirectsToHome()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);
            var model = new GirisYapViewModel { Eposta = "test@test.com", Sifre = "Password123!", BeniHatirla = false };

            var result = await controller.GirisYap(model) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public async Task GirisYap_Post_ValidModel_FailedLogin_ReturnsViewWithError()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);
            var model = new GirisYapViewModel { Eposta = "test@test.com", Sifre = "Password123!", BeniHatirla = false };

            var result = await controller.GirisYap(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task GirisYap_Post_InvalidModel_ReturnsViewWithModel()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);
            controller.ModelState.AddModelError("Eposta", "Zorunlu alan");
            var model = new GirisYapViewModel();

            var result = await controller.GirisYap(model) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task CikisYap_ReturnsRedirectToHome()
        {
            var userManager = GetUserManagerMock();
            var signInManager = GetSignInManagerMock(userManager.Object);
            var roleManager = GetRoleManagerMock();
            signInManager.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);
            var controller = new KullaniciController(userManager.Object, signInManager.Object, roleManager.Object);

            var result = await controller.CikisYap() as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }
    }
} 