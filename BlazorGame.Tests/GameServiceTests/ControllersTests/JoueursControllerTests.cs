using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using BlazorGame.GameService.Controllers;
using SharedModels.Domain.Users;
using BlazorGame.GameService.Persistence;

namespace BlazorGame.Tests.GameServiceTests.ControllersTests
{
    public class JoueursControllerTests
    {
        [Fact]
        public async Task Register_ReturnsConflictIfPseudoExists()
        {
            var repo = new Mock<IRepository<Joueur>>();
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Joueur> { new Joueur { Pseudo = "dup" } });
                var ctrl = new JoueursController(repo.Object);
            var req = new RegisterRequest { Pseudo = "dup" };
            var result = await ctrl.Register(req, CancellationToken.None);
            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Contains("existe déjà", conflict.Value.ToString());
        }

        [Fact]
        public async Task Register_ReturnsOkIfNewPseudo()
        {
            var repo = new Mock<IRepository<Joueur>>();
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Joueur>());
            repo.Setup(r => r.AddAsync(It.IsAny<Joueur>(), It.IsAny<CancellationToken>())).ReturnsAsync((Joueur j, CancellationToken ct) => j);
                var ctrl = new JoueursController(repo.Object);
            var req = new RegisterRequest { Pseudo = "newuser" };
            var result = await ctrl.Register(req, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueur = Assert.IsType<Joueur>(ok.Value);
            Assert.Equal("newuser", joueur.Pseudo);
        }

        [Fact]
        public async Task Login_ReturnsNotFoundIfMissing()
        {
            var repo = new Mock<IRepository<Joueur>>();
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Joueur>());
                var ctrl = new JoueursController(repo.Object);
            var req = new LoginRequest { Pseudo = "absent" };
            var result = await ctrl.Login(req, CancellationToken.None);
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("introuvable", notFound.Value.ToString());
        }

        [Fact]
        public async Task Login_ReturnsOkIfActive()
        {
            var repo = new Mock<IRepository<Joueur>>();
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Joueur> { new Joueur { Pseudo = "active", Actif = true, Id = Guid.NewGuid() } });
                var ctrl = new JoueursController(repo.Object);
            var req = new LoginRequest { Pseudo = "active" };
            var result = await ctrl.Login(req, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueur = Assert.IsType<Joueur>(ok.Value);
            Assert.Equal("active", joueur.Pseudo);
        }

        [Fact]
        public async Task Login_ReturnsNotFoundIfInactive()
        {
            var repo = new Mock<IRepository<Joueur>>();
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Joueur> { new Joueur { Pseudo = "inactive", Actif = false } });
                var ctrl = new JoueursController(repo.Object);
            var req = new LoginRequest { Pseudo = "inactive" };
            var result = await ctrl.Login(req, CancellationToken.None);
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("désactivé", notFound.Value.ToString());
        }

        [Fact]
        public void Logout_DeletesCookiesAndReturnsNoContent()
        {
            var repo = new Mock<IRepository<Joueur>>();
                var ctrl = new JoueursController(repo.Object);
            var result = ctrl.Logout();
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFoundIfMissing()
        {
            var repo = new Mock<IRepository<Joueur>>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Joueur)null);
                var ctrl = new JoueursController(repo.Object);
            var result = await ctrl.GetById(Guid.NewGuid(), CancellationToken.None);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_ReturnsOkIfActive()
        {
            var repo = new Mock<IRepository<Joueur>>();
            var joueur = new Joueur { Id = Guid.NewGuid(), Pseudo = "ok", Actif = true };
            repo.Setup(r => r.GetByIdAsync(joueur.Id, It.IsAny<CancellationToken>())).ReturnsAsync(joueur);
                var ctrl = new JoueursController(repo.Object);
            var result = await ctrl.GetById(joueur.Id, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var j = Assert.IsType<Joueur>(ok.Value);
            Assert.Equal("ok", j.Pseudo);
        }

        [Fact]
        public async Task GetById_ReturnsNotFoundIfInactive()
        {
            var repo = new Mock<IRepository<Joueur>>();
            var joueur = new Joueur { Id = Guid.NewGuid(), Pseudo = "no", Actif = false };
            repo.Setup(r => r.GetByIdAsync(joueur.Id, It.IsAny<CancellationToken>())).ReturnsAsync(joueur);
                var ctrl = new JoueursController(repo.Object);
            var result = await ctrl.GetById(joueur.Id, CancellationToken.None);
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("admin", notFound.Value.ToString());
        }

        [Fact]
        public async Task UpdateActif_UpdatesStatus()
        {
            var repo = new Mock<IRepository<Joueur>>();
            var joueur = new Joueur { Id = Guid.NewGuid(), Pseudo = "update", Actif = false };
            repo.Setup(r => r.GetByIdAsync(joueur.Id, It.IsAny<CancellationToken>())).ReturnsAsync(joueur);
            repo.Setup(r => r.UpdateAsync(joueur, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
                var ctrl = new JoueursController(repo.Object);
            var result = await ctrl.UpdateActif(joueur.Id, true, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var j = Assert.IsType<Joueur>(ok.Value);
            Assert.True(j.Actif);
        }

        [Fact]
        public async Task ListAsync_ReturnsAllJoueurs()
        {
            var repo = new Mock<IRepository<Joueur>>();
            var joueurs = new List<Joueur> { new Joueur { Pseudo = "a" }, new Joueur { Pseudo = "b" } };
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(joueurs);
                var ctrl = new JoueursController(repo.Object);
            var result = await ctrl.ListAsync(CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<Joueur>>(ok.Value);
            Assert.Contains(list, j => j.Pseudo == "a");
            Assert.Contains(list, j => j.Pseudo == "b");
        }

        [Fact]
        public async Task Register_SetsCookiesWhenResponseIsPresent()
        {
            var repo = new Mock<IRepository<Joueur>>();
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Joueur>());
            repo.Setup(r => r.AddAsync(It.IsAny<Joueur>(), It.IsAny<CancellationToken>())).ReturnsAsync((Joueur j, CancellationToken ct) => j);
            
            var ctrl = new JoueursController(repo.Object);
            
            // Mock HttpContext avec Response et Cookies
            var mockHttpContext = new DefaultHttpContext();
            ctrl.ControllerContext = new ControllerContext { HttpContext = mockHttpContext };
            
            var req = new RegisterRequest { Pseudo = "newuser" };
            var result = await ctrl.Register(req, CancellationToken.None);
            
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueur = Assert.IsType<Joueur>(ok.Value);
            Assert.Equal("newuser", joueur.Pseudo);
            
            // Vérifie que les cookies ont été définis
            Assert.True(mockHttpContext.Response.Headers.SetCookie.Count > 0);
        }

        [Fact]
        public async Task Login_SetsCookiesWhenResponseIsPresent()
        {
            var repo = new Mock<IRepository<Joueur>>();
            var existingJoueur = new Joueur { Id = Guid.NewGuid(), Pseudo = "existing", Actif = true };
            repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Joueur> { existingJoueur });
            
            var ctrl = new JoueursController(repo.Object);
            
            // Mock HttpContext avec Response et Cookies
            var mockHttpContext = new DefaultHttpContext();
            ctrl.ControllerContext = new ControllerContext { HttpContext = mockHttpContext };
            
            var req = new LoginRequest { Pseudo = "existing" };
            var result = await ctrl.Login(req, CancellationToken.None);
            
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var joueur = Assert.IsType<Joueur>(ok.Value);
            Assert.Equal("existing", joueur.Pseudo);
            
            // Vérifie que les cookies ont été définis
            Assert.True(mockHttpContext.Response.Headers.SetCookie.Count > 0);
        }

        [Fact]
        public void Logout_DeletesCookiesWhenResponseIsPresent()
        {
            var repo = new Mock<IRepository<Joueur>>();
            var ctrl = new JoueursController(repo.Object);
            
            // Mock HttpContext avec Response et Cookies
            var mockHttpContext = new DefaultHttpContext();
            ctrl.ControllerContext = new ControllerContext { HttpContext = mockHttpContext };
            
            var result = ctrl.Logout();
            
            Assert.IsType<NoContentResult>(result);
            // Vérifie que les cookies ont été supprimés
            Assert.True(mockHttpContext.Response.Headers.SetCookie.Count > 0);
        }

        [Fact]
        public async Task UpdateActif_ReturnsNotFoundIfJoueurMissing()
        {
            var repo = new Mock<IRepository<Joueur>>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Joueur?)null);
            
            var ctrl = new JoueursController(repo.Object);
            var result = await ctrl.UpdateActif(Guid.NewGuid(), true, CancellationToken.None);
            
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}