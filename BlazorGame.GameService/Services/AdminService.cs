using BlazorGame.GameService.Persistence;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace BlazorGame.GameService.Services
{
    public class AdminService
    {
        private readonly Repository<Joueur> _joueurRepo;
        private readonly Repository<Partie> _partieRepo;
        private readonly ScoresService _scoresService;

        public AdminService(Repository<Joueur> joueurRepo, Repository<Partie> partieRepo, ScoresService scoresService)
        {
            _joueurRepo = joueurRepo;
            _partieRepo = partieRepo;
            _scoresService = scoresService;
        }

        public async Task<IEnumerable<Joueur>> GetJoueursAsync(CancellationToken ct) => await _joueurRepo.ListAsync(ct);
        public async Task<Joueur?> SetJoueurActifAsync(Guid id, bool actif, CancellationToken ct)
        {
            var joueur = await _joueurRepo.GetByIdAsync(id, ct);
            if (joueur == null) return null;
            joueur.Actif = actif;
            await _joueurRepo.UpdateAsync(joueur, ct);
            return joueur;
        }
        public async Task<IEnumerable<object>> GetScoresAsync(CancellationToken ct) => await _scoresService.GetAllAsync(ct);
        public async Task<IEnumerable<object>> GetLeaderboardAsync(CancellationToken ct) => await _scoresService.GetLeaderboardAsync(ct);
        public async Task<IEnumerable<Partie>> GetPartiesAsync(CancellationToken ct) => await _partieRepo.ListAsync(ct);
        public async Task<string> ExportJoueursCsvAsync(CancellationToken ct)
        {
            var joueurs = await _joueurRepo.ListAsync(ct);
            return string.Join("\n", joueurs.Select(j => $"{j.Id},{j.Pseudo},{j.Actif}"));
        }
    }
}
