using BlazorGame.GameService.Controllers;
using BlazorGame.Tests.Support;
using SharedModels.Domain.Scores;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BlazorGame.Tests.ControllersTests
{
    /// <summary>
    /// Tests unitaires pour ScoresController.
    /// </summary>
    public class ScoresControllerTests
    {
        /// <summary>
        /// Vérifie que POST /api/scores crée un score cohérent.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task Post_Cree_Score()
        {
            var repo = new FakeRepository<Score>();
            var ctrl = new ScoresController(repo);

            var req = new ScoresController.ScoreRequest
            {
                JoueurId = Guid.NewGuid(),
                PartieId = Guid.NewGuid(),
                Valeur = 4200
            };

            var result = await ctrl.Post(req, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var scoreCree = Assert.IsType<Score>(ok.Value);

            Assert.Equal(req.JoueurId, scoreCree.JoueurId);
            Assert.Equal(req.PartieId, scoreCree.PartieId);
            Assert.Equal(4200, scoreCree.Valeur);
            Assert.NotEqual(Guid.Empty, scoreCree.Id);
            Assert.True(scoreCree.EnregistreLe <= DateTime.UtcNow);
        }

        /// <summary>
        /// Vérifie que leaderboard renvoie les scores triés par valeur décroissante
        /// et que le premier élément est bien le score le plus élevé.
        /// </summary>
        /// <returns>Task complétée.</returns>
        [Fact]
        public async Task Leaderboard_Trie_Par_Valeur()
        {
            var repo = new FakeRepository<Score>();
            var ctrl = new ScoresController(repo);

            var scoreBas = new Score
            {
                Id = Guid.NewGuid(),
                JoueurId = Guid.NewGuid(),
                PartieId = Guid.NewGuid(),
                Valeur = 10,
                EnregistreLe = DateTime.UtcNow
            };

            var scoreHaut = new Score
            {
                Id = Guid.NewGuid(),
                JoueurId = Guid.NewGuid(),
                PartieId = Guid.NewGuid(),
                Valeur = 999,
                EnregistreLe = DateTime.UtcNow
            };

            await repo.AddAsync(scoreBas, CancellationToken.None);
            await repo.AddAsync(scoreHaut, CancellationToken.None);

            var result = await ctrl.GetLeaderboard(CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result.Result);

            var enumerable = Assert.IsAssignableFrom<System.Collections.IEnumerable>(ok.Value);

            var enumerator = enumerable.GetEnumerator();
            Assert.True(enumerator.MoveNext());
            var firstItem = enumerator.Current;

            var propValeur = firstItem!.GetType().GetProperty("Valeur");
            Assert.NotNull(propValeur);

            var topScore = (int)propValeur!.GetValue(firstItem)!;
            Assert.Equal(999, topScore);
        }
    }
}
