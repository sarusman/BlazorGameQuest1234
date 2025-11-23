using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorGame.GameService.Controllers;
using BlazorGame.GameService.Services;
using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.Tests.ControllersTests
{
    public class PartiesControllerTests
    {
        [Fact]
        public async Task DemarrerAndChoisirEndpoints_Work()
        {
            // Summary: Test basique des endpoints Demarrer et Choisir via le controller.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);

            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S1", Description = "d" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "C1" };
            choix.Effets.Add(new Effet { Id = Guid.NewGuid(), Type = SharedModels.Domain.Common.Enums.TypeEffet.GainPoints, Valeur = 1, Description = "+1" });
            salle.ChoixProposes.Add(choix);

            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var ctrl = new PartiesController(svc);

            var start = new PartiesController.StartPartieRequest { JoueurId = Guid.NewGuid(), DonjonId = donjon.Id };

            // Act - Demarrer
            var res = await ctrl.Demarrer(start, CancellationToken.None);
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var partie = Assert.IsType<Partie>(ok.Value);

            // Act - Choisir
            var chooseReq = new PartiesController.ChoisirRequest { SalleId = salle.Id, ChoixId = choix.Id };
            var chooseRes = await ctrl.Choisir(partie.Id, chooseReq, CancellationToken.None);

            // Assert choose
            var okChoose = Assert.IsType<OkObjectResult>(chooseRes.Result);
            Assert.NotNull(okChoose.Value);
        }

        [Fact]
        public async Task Choisir_WithUnknownPartie_ReturnsDefaultResponse()
        {
            // Summary: Vérifie le endpoint Choisir renvoie la réponse par défaut si la partie est inconnue.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var svc = new PartieService(db);
            var ctrl = new PartiesController(svc);

            // Act
            var req = new PartiesController.ChoisirRequest { SalleId = Guid.NewGuid(), ChoixId = Guid.NewGuid() };
            var res = await ctrl.Choisir(Guid.NewGuid(), req, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var payload = Assert.IsType<PartiesController.ChoisirResponse>(ok.Value);
            Assert.Equal(0, payload.Score);
            Assert.True(payload.Fini);
        }

        [Fact]
        public async Task Demarrer_CreatesPartieWithCorrectProperties()
        {
            // Summary: Vérifie que Demarrer crée une partie avec les propriétés correctes.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S1", Description = "d" };
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var ctrl = new PartiesController(svc);
            var joueurId = Guid.NewGuid();
            var start = new PartiesController.StartPartieRequest { JoueurId = joueurId, DonjonId = donjon.Id };

            // Act
            var res = await ctrl.Demarrer(start, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var partie = Assert.IsType<Partie>(ok.Value);
            Assert.Equal(joueurId, partie.JoueurId);
            Assert.Equal(donjon.Id, partie.DonjonId);
            Assert.Equal(10, partie.ScoreFinal);
            Assert.False(partie.EstTerminee);
        }

        [Fact]
        public async Task Demarrer_ReturnsConflict_WhenPartieAlreadyExists()
        {
            // Summary: Vérifie que Demarrer retourne Conflict quand une partie existe déjà pour ce joueur/donjon.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S1" };
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var ctrl = new PartiesController(svc);
            var joueurId = Guid.NewGuid();
            var start = new PartiesController.StartPartieRequest { JoueurId = joueurId, DonjonId = donjon.Id };

            // Act - première création
            await ctrl.Demarrer(start, CancellationToken.None);

            // Act - deuxième création (devrait échouer)
            var res = await ctrl.Demarrer(start, CancellationToken.None);

            // Assert
            Assert.IsType<ConflictObjectResult>(res.Result);
        }

        [Fact]
        public async Task Choisir_ReturnsCorrectResponse_WithGainPoints()
        {
            // Summary: Vérifie que Choisir retourne une réponse correcte avec gain de points.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var effet = new Effet { Id = Guid.NewGuid(), Type = SharedModels.Domain.Common.Enums.TypeEffet.GainPoints, Valeur = 15, Description = "+15" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "Gagner" };
            choix.Effets.Add(effet);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S1" };
            salle.ChoixProposes.Add(choix);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var ctrl = new PartiesController(svc);
            var joueurId = Guid.NewGuid();
            var start = new PartiesController.StartPartieRequest { JoueurId = joueurId, DonjonId = donjon.Id };
            var startRes = await ctrl.Demarrer(start, CancellationToken.None);
            var partie = Assert.IsType<Partie>(((OkObjectResult)startRes.Result!).Value);

            var chooseReq = new PartiesController.ChoisirRequest { SalleId = salle.Id, ChoixId = choix.Id };

            // Act
            var chooseRes = await ctrl.Choisir(partie.Id, chooseReq, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(chooseRes.Result);
            var response = Assert.IsType<PartiesController.ChoisirResponse>(ok.Value);
            Assert.Equal(25, response.Score); // 10 initial + 15
            Assert.False(response.Mort);
        }

        [Fact]
        public async Task Choisir_ReturnsCorrectResponse_WithMort()
        {
            // Summary: Vérifie que Choisir retourne une réponse correcte avec mort instantanée.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var effet = new Effet { Id = Guid.NewGuid(), Type = SharedModels.Domain.Common.Enums.TypeEffet.MortInstantanee, Description = "Mort" };
            var choix = new Choix { Id = Guid.NewGuid(), Libelle = "Mort" };
            choix.Effets.Add(effet);
            var salle = new Salle { Id = Guid.NewGuid(), Titre = "S1" };
            salle.ChoixProposes.Add(choix);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var ctrl = new PartiesController(svc);
            var joueurId = Guid.NewGuid();
            var start = new PartiesController.StartPartieRequest { JoueurId = joueurId, DonjonId = donjon.Id };
            var startRes = await ctrl.Demarrer(start, CancellationToken.None);
            var partie = Assert.IsType<Partie>(((OkObjectResult)startRes.Result!).Value);

            var chooseReq = new PartiesController.ChoisirRequest { SalleId = salle.Id, ChoixId = choix.Id };

            // Act
            var chooseRes = await ctrl.Choisir(partie.Id, chooseReq, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(chooseRes.Result);
            var response = Assert.IsType<PartiesController.ChoisirResponse>(ok.Value);
            Assert.True(response.Mort);
            Assert.True(response.Fini);
        }

        [Fact]
        public async Task Choisir_ReturnsNextSalleId_WhenNotFinished()
        {
            // Summary: Vérifie que Choisir retourne le nextSalleId quand la partie n'est pas terminée.

            // Arrange
            var opts = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var db = new GameDbContext(opts);
            var salle1 = new Salle { Id = Guid.NewGuid(), Titre = "S1" };
            var salle2 = new Salle { Id = Guid.NewGuid(), Titre = "S2" };
            var choix1 = new Choix { Id = Guid.NewGuid(), Libelle = "C1" };
            salle1.ChoixProposes.Add(choix1);
            var donjon = new Donjon { Id = Guid.NewGuid(), Nom = "D", Salles = new System.Collections.Generic.List<Salle> { salle1, salle2 } };
            await db.Donjons.AddAsync(donjon, CancellationToken.None);
            await db.SaveChangesAsync(CancellationToken.None);

            var svc = new PartieService(db);
            var ctrl = new PartiesController(svc);
            var joueurId = Guid.NewGuid();
            var start = new PartiesController.StartPartieRequest { JoueurId = joueurId, DonjonId = donjon.Id };
            var startRes = await ctrl.Demarrer(start, CancellationToken.None);
            var partie = Assert.IsType<Partie>(((OkObjectResult)startRes.Result!).Value);

            var chooseReq = new PartiesController.ChoisirRequest { SalleId = salle1.Id, ChoixId = choix1.Id };

            // Act
            var chooseRes = await ctrl.Choisir(partie.Id, chooseReq, CancellationToken.None);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(chooseRes.Result);
            var response = Assert.IsType<PartiesController.ChoisirResponse>(ok.Value);
            Assert.NotNull(response.NextSalleId);
            Assert.Equal(salle2.Id, response.NextSalleId);
        }
    }
}



