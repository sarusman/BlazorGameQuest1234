using SharedModels.Domain.Common.Enums;
using SharedModels.Domain.Common.Records;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.GameService.Services
{
    /// <summary>
    /// Génère une salle avec des choix simples et cohérents.
    /// </summary>
    public class SalleService
    {
        /// <summary>
        /// Génère une nouvelle salle avec un titre, une description,
        /// un type (optionnellement imposé), une difficulté, et une liste de choix.
        /// </summary>
        /// <param name="difficulte">Niveau de difficulté global.</param>
        /// <param name="type">Type de salle à forcer, ou <c>null</c> pour aléatoire.</param>
        /// <param name="seed">Graine RNG pour un résultat reproductible, ou <c>null</c>.</param>
        /// <returns>
        /// Une instance de <see cref="Salle"/> prête à être ajoutée dans un donjon,
        /// avec des choix adaptés au type de salle.
        /// </returns>
        public Salle GenererSalle(Difficulte difficulte, TypeSalle? type, int? seed)
        {
            var rng = seed.HasValue ? new Random(seed.Value) : new Random();

            var t = type.HasValue
                ? type.Value
                : (TypeSalle)rng.Next(Enum.GetValues(typeof(TypeSalle)).Length);

            string titre;
            string description;

            if (t == TypeSalle.Combat)
            {
                titre = "Un gobelin apparaît. Que faites-vous ?";
                description = "Combattre, fuir ou fouiller la zone.";
            }
            else if (t == TypeSalle.Coffre)
            {
                titre = "Un coffre mystérieux";
                description = "L'ouvrir ou l'ignorer ?";
            }
            else if (t == TypeSalle.Piege)
            {
                titre = "Dalles instables";
                description = "Avancez prudemment.";
            }
            else if (t == TypeSalle.Rencontre)
            {
                titre = "Un inconnu dans l'ombre";
                description = "Parler, utiliser un objet, ou passer.";
            }
            else if (t == TypeSalle.Enigme)
            {
                titre = "Porte à énigme";
                description = "Résoudre ou éviter.";
            }
            else
            {
                titre = "Coin de repos";
                description = "Se reposer ou continuer.";
            }

            int scale;
            if (difficulte == Difficulte.Facile) scale = 6;
            else if (difficulte == Difficulte.Normal) scale = 12;
            else scale = 18;

            var choix = new List<Choix>();

            if (t == TypeSalle.Combat)
            {
                choix.Add(NouveauChoix(
                    TypeChoix.Combattre, "Combattre",
                    EffetGain(rng.Next(4, scale + 1), "Succès"),
                    EffetPerte(-rng.Next(2, scale / 2 + 2), "Blessure")
                ));

                choix.Add(NouveauChoix(
                    TypeChoix.Fuir, "Fuir",
                    EffetPerte(-rng.Next(1, 4), "Temps perdu")
                ));

                if (rng.NextDouble() < 0.5)
                {
                    choix.Add(NouveauChoix(
                        TypeChoix.Fouiller, "Fouiller",
                        EffetGain(rng.Next(4, scale + 1), "Trouvaille")
                    ));
                }
                else
                {
                    choix.Add(NouveauChoix(
                        TypeChoix.Fouiller, "Fouiller",
                        EffetPerte(-rng.Next(2, scale / 2 + 2), "Petit piège")
                    ));
                }
            }
            else if (t == TypeSalle.Coffre)
            {
                var p = rng.NextDouble();
                if (p < 0.10)
                {
                    choix.Add(NouveauChoix(
                        TypeChoix.Ouvrir, "Ouvrir",
                        EffetMort("Piège mortel")
                    ));
                }
                else if (p < 0.55)
                {
                    choix.Add(NouveauChoix(
                        TypeChoix.Ouvrir, "Ouvrir",
                        EffetGain(rng.Next(scale / 2, scale + 3), "Trésor")
                    ));
                }
                else
                {
                    choix.Add(NouveauChoix(
                        TypeChoix.Ouvrir, "Ouvrir",
                        EffetPerte(-rng.Next(scale / 2, scale + 1), "Piège")
                    ));
                }

                choix.Add(NouveauChoix(
                    TypeChoix.Ignorer, "Ignorer",
                    EffetRien("Vous passez votre chemin")
                ));
            }
            else if (t == TypeSalle.Enigme)
            {
                if (rng.NextDouble() < 0.5)
                {
                    choix.Add(NouveauChoix(
                        TypeChoix.Resolver, "Résoudre",
                        EffetGain(rng.Next(scale / 2, scale + 1), "Bonne réponse")
                    ));
                }
                else
                {
                    choix.Add(NouveauChoix(
                        TypeChoix.Resolver, "Résoudre",
                        EffetPerte(-rng.Next(2, scale / 2 + 2), "Mauvaise réponse")
                    ));
                }

                choix.Add(NouveauChoix(
                    TypeChoix.Fuir, "Fuir",
                    EffetPerte(-1, "Vous perdez un peu de temps")
                ));
            }
            else if (t == TypeSalle.Repos)
            {
                choix.Add(NouveauChoix(
                    TypeChoix.Ignorer, "Continuer",
                    EffetRien("Vous poursuivez")
                ));

                choix.Add(NouveauChoix(
                    TypeChoix.UtiliserObjet, "Se reposer",
                    EffetGain(rng.Next(2, 6), "Vous reprenez des forces")
                ));
            }
            else
            {
                choix.Add(NouveauChoix(
                    TypeChoix.Ignorer, "Avancer",
                    EffetRien("Rien à signaler")
                ));
            }

            var salle = new Salle
            {
                Id = Guid.NewGuid(),
                Titre = titre,
                Description = description,
                Type = t,
                Difficulte = difficulte,
                ChoixProposes = choix,
                ButinPotentiel = null
            };

            return salle;
        }

        /// <summary>
        /// Crée un choix avec un type, un libellé et une liste d’effets.
        /// </summary>
        /// <param name="type">Type de choix (Combattre, Fuir, etc.).</param>
        /// <param name="libelle">Texte affiché pour le choix.</param>
        /// <param name="effets">Effets appliqués si ce choix est sélectionné.</param>
        /// <returns>Un <see cref="Choix"/> prêt à être affiché en salle.</returns>
        private static Choix NouveauChoix(TypeChoix type, string libelle, params Effet[] effets)
        {
            var c = new Choix();
            c.Id = Guid.NewGuid();
            c.Type = type;
            c.Libelle = libelle;
            c.Effets = effets.ToList();
            return c;
        }

        /// <summary>
        /// Crée un effet de type gain de points.
        /// </summary>
        /// <param name="valeur">Nombre de points gagnés.</param>
        /// <param name="texte">Description courte de l’effet.</param>
        /// <returns>Un <see cref="Effet"/> de type <see cref="TypeEffet.GainPoints"/>.</returns>
        private static Effet EffetGain(int valeur, string texte)
        {
            var e = new Effet();
            e.Id = Guid.NewGuid();
            e.Type = TypeEffet.GainPoints;
            e.Valeur = valeur;
            e.Description = texte;
            return e;
        }

        /// <summary>
        /// Crée un effet de type perte de points.
        /// </summary>
        /// <param name="valeurNegative">Points perdus (attendu négatif).</param>
        /// <param name="texte">Description courte de l’effet.</param>
        /// <returns>Un <see cref="Effet"/> de type <see cref="TypeEffet.PertePoints"/>.</returns>
        private static Effet EffetPerte(int valeurNegative, string texte)
        {
            var e = new Effet();
            e.Id = Guid.NewGuid();
            e.Type = TypeEffet.PertePoints;
            e.Valeur = valeurNegative;
            e.Description = texte;
            return e;
        }

        /// <summary>
        /// Crée un effet de mort instantanée.
        /// </summary>
        /// <param name="texte">Description courte de l’effet.</param>
        /// <returns>Un <see cref="Effet"/> de type <see cref="TypeEffet.MortInstantanee"/>.</returns>
        private static Effet EffetMort(string texte)
        {
            var e = new Effet();
            e.Id = Guid.NewGuid();
            e.Type = TypeEffet.MortInstantanee;
            e.Valeur = 0;
            e.Description = texte;
            return e;
        }

        /// <summary>
        /// Crée un effet neutre (aucun impact).
        /// </summary>
        /// <param name="texte">Description courte de l’effet.</param>
        /// <returns>Un <see cref="Effet"/> de type <see cref="TypeEffet.Rien"/>.</returns>
        private static Effet EffetRien(string texte)
        {
            var e = new Effet();
            e.Id = Guid.NewGuid();
            e.Type = TypeEffet.Rien;
            e.Valeur = 0;
            e.Description = texte;
            return e;
        }
    }
}



