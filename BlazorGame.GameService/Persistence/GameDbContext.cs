using Microsoft.EntityFrameworkCore;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Scores;

namespace BlazorGame.GameService.Persistence
{
    /// <summary>
    /// Contexte EF Core relié à la base de données du jeu.
    /// </summary>
    public class GameDbContext : DbContext
    {
        /// <summary>
        /// Initialise le contexte EF Core.
        /// </summary>
        /// <param name="options">Options du contexte (provider, chaîne de connexion).</param>
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {
        }

        public DbSet<Joueur> Joueurs => Set<Joueur>();
        public DbSet<Partie> Parties => Set<Partie>();
        public DbSet<Score> Scores => Set<Score>();
        public DbSet<Donjon> Donjons => Set<Donjon>();
        public DbSet<Salle> Salles => Set<Salle>();
        public DbSet<Choix> Choix => Set<Choix>();
        public DbSet<Effet> Effets => Set<Effet>();

        /// <summary>
        /// Configure les relations EF Core.
        /// </summary>
        /// <param name="modelBuilder">Builder du modèle EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Joueur>(entity =>
            {
                entity.HasKey(j => j.Id);
                entity.HasIndex(j => j.Pseudo).IsUnique();
            });

            modelBuilder.Entity<Partie>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasOne(p => p.Joueur);
            });

            modelBuilder.Entity<Score>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasOne(s => s.Joueur);
            });
        }
    }
}
