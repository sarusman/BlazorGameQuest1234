using Microsoft.EntityFrameworkCore;
using SharedModels.Domain.Users;
using SharedModels.Domain.Gameplay;
using SharedModels.Domain.Scores;

namespace BlazorGame.GameService.Persistence
{
    /// <summary>Contexte EF Core de l’application de jeu.</summary>
    public class GameDbContext : DbContext
    {
        /// <summary>Construit le contexte EF Core.</summary>
        /// <param name="options">Options du contexte (provider, connexion, etc.).</param>
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        /// <summary>Table des joueurs.</summary>
        public DbSet<Joueur> Joueurs => Set<Joueur>();

        /// <summary>Table des parties.</summary>
        public DbSet<Partie> Parties => Set<Partie>();

        /// <summary>Table des étapes de partie.</summary>
        public DbSet<EtapeDePartie> Etapes => Set<EtapeDePartie>();

        /// <summary>Table des scores.</summary>
        public DbSet<Score> Scores => Set<Score>();

        /// <summary>Table des donjons.</summary>
        public DbSet<Donjon> Donjons => Set<Donjon>();

        /// <summary>Table des salles.</summary>
        public DbSet<Salle> Salles => Set<Salle>();

        /// <summary>Table des choix proposés dans une salle.</summary>
        public DbSet<Choix> Choix => Set<Choix>();

        /// <summary>Table des effets (gain/perte/mort…)</summary>
        public DbSet<Effet> Effets => Set<Effet>();

        /// <summary>Configure les relations et contraintes du modèle.</summary>
        /// <param name="modelBuilder">Builder du modèle EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Joueur>()
                .HasIndex(j => j.Pseudo)
                .IsUnique();

            modelBuilder.Entity<Partie>()
                .HasOne(p => p.Joueur)
                .WithMany(j => j.Parties)
                .HasForeignKey(p => p.JoueurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Partie>()
                .HasMany(p => p.Etapes)
                .WithOne(e => e.Partie!)
                .HasForeignKey(e => e.PartieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Score>()
                .HasOne(s => s.Joueur)
                .WithMany(j => j.Scores)
                .HasForeignKey(s => s.JoueurId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Score>() // to rm v4 !!
                .HasOne<Partie>()
                .WithMany()
                .HasForeignKey(s => s.PartieId) 
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Donjon>()
                .HasMany(d => d.Salles)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Salle>()
                .HasMany(s => s.ChoixProposes)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Choix>()
                .HasMany(c => c.Effets)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Salle>()
                .Ignore(s => s.ButinPotentiel);
        }
    }
}

