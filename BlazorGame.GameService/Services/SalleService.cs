using SharedModels.Domain.Common.Enums;
using SharedModels.Domain.Common.Records;
using SharedModels.Domain.Gameplay;

namespace BlazorGame.GameService.Services
{
    /// <summary>Génère une salle avec des choix cohérents.</summary>
    public class SalleService
    {
        /// <summary>Génère une salle.</summary>
        /// <param name="difficulte">Difficulté.</param>
        /// <param name="type">Type forcé ou null pour aléatoire.</param>
        /// <param name="seed">Graine RNG.</param>
        /// <returns>Salle générée.</returns>
        public Salle GenererSalle(Difficulte difficulte, TypeSalle? type, int? seed)
        {
            var r = seed.HasValue ? new Random(seed.Value) : new Random();

            TypeSalle t;
            if (type.HasValue) t = type.Value;
            else t = (TypeSalle)r.Next(Enum.GetValues(typeof(TypeSalle)).Length);

            string titre;
            string desc;

            if (t == TypeSalle.Combat)
            {
                titre = "Un gobelin apparaît. Que faites-vous ?";
                desc = "Combattre, fuir ou fouiller la zone.";
            }
            else if (t == TypeSalle.Coffre)
            {
                titre = "Un coffre mystérieux";
                desc = "L'ouvrir ou l'ignorer ?";
            }
            else if (t == TypeSalle.Piege)
            {
                titre = "Dalles instables";
                desc = "Mieux vaut avancer prudemment.";
            }
            else if (t == TypeSalle.Rencontre)
            {
                titre = "Un inconnu dans l'ombre";
                desc = "Parler, utiliser un objet, ou passer.";
            }
            else if (t == TypeSalle.Enigme)
            {
                titre = "Porte à énigme";
                desc = "Résoudre ou éviter.";
            }
            else
            {
                titre = "Coin de repos";
                desc = "Se reposer ou continuer.";
            }

            int scale;
            if (difficulte == Difficulte.Facile) scale = 6;
            else if (difficulte == Difficulte.Normal) scale = 12;
            else scale = 18;

            var choix = new List<Choix>();

            if (t == TypeSalle.Combat)
            {
                choix.Add(BuildChoix(TypeChoix.Combattre, "Combattre",
                    EffetGain(r.Next(4, scale + 1), "Succès au combat"),
                    EffetPerte(-r.Next(2, scale / 2 + 2), "Blessure au combat")
                ));

                choix.Add(BuildChoix(TypeChoix.Fuir, "Fuir",
                    EffetPerte(-r.Next(1, 4), "Vous perdez du temps en fuyant")
                ));

                if (r.NextDouble() < 0.5)
                    choix.Add(BuildChoix(TypeChoix.Fouiller, "Fouiller",
                        EffetGain(r.Next(4, scale + 1), "Vous trouvez quelque chose d'utile")
                    ));
                else
                    choix.Add(BuildChoix(TypeChoix.Fouiller, "Fouiller",
                        EffetPerte(-r.Next(2, scale / 2 + 2), "Un petit piège vous blesse")
                    ));
            }
            else if (t == TypeSalle.Coffre)
            {
                // Ouvrir: trésor (gain), piège (perte), mort 10%
                double p = r.NextDouble();
                if (p < 0.10)
                    choix.Add(BuildChoix(TypeChoix.Ouvrir, "Ouvrir",
                        EffetMort("Piège mortel dans le coffre")
                    ));
                else if (p < 0.55)
                    choix.Add(BuildChoix(TypeChoix.Ouvrir, "Ouvrir",
                        EffetGain(r.Next(scale / 2, scale + 3), "Trésor !")
                    ));
                else
                    choix.Add(BuildChoix(TypeChoix.Ouvrir, "Ouvrir",
                        EffetPerte(-r.Next(scale / 2, scale + 1), "Un piège vous blesse")
                    ));

                choix.Add(BuildChoix(TypeChoix.Ignorer, "Ignorer",
                    EffetRien("Vous passez votre chemin")
                ));
            }
            else if (t == TypeSalle.Enigme)
            {
                // Résoudre: 50/50
                if (r.NextDouble() < 0.5)
                    choix.Add(BuildChoix(TypeChoix.Resolver, "Résoudre",
                        EffetGain(r.Next(scale / 2, scale + 1), "Bonne réponse")
                    ));
                else
                    choix.Add(BuildChoix(TypeChoix.Resolver, "Résoudre",
                        EffetPerte(-r.Next(2, scale / 2 + 2), "Mauvaise réponse")
                    ));

                // Fuir: petit malus −1
                choix.Add(BuildChoix(TypeChoix.Fuir, "Fuir",
                    EffetPerte(-1, "Vous perdez un peu de temps")
                ));
            }
            else if (t == TypeSalle.Repos)
            {
                choix.Add(BuildChoix(TypeChoix.Ignorer, "Continuer",
                    EffetRien("Vous poursuivez votre route")
                ));


                choix.Add(BuildChoix(TypeChoix.UtiliserObjet, "Se reposer",
                    EffetGain(r.Next(2, 6), "Vous reprenez des forces")
                ));
            }
            else
            {
                choix.Add(BuildChoix(TypeChoix.Ignorer, "Avancer",
                    EffetRien("Rien à signaler")
                ));
            }

            var s = new Salle();
            s.Id = Guid.NewGuid();
            s.Titre = titre;
            s.Description = desc;
            s.Type = t;
            s.Difficulte = difficulte;
            s.ChoixProposes = choix;
            s.ButinPotentiel = null;

            return s;
        }

        private static Choix BuildChoix(TypeChoix type, string libelle, params Effet[] effets)
        {
            var c = new Choix();
            c.Id = Guid.NewGuid();
            c.Type = type;
            c.Libelle = libelle;
            c.Effets = effets.ToList();
            return c;
        }

        private static Effet EffetGain(int val, string txt)
        {
            var e = new Effet();
            e.Id = Guid.NewGuid();
            e.Type = TypeEffet.GainPoints;
            e.Valeur = val;
            e.Description = txt;
            return e;
        }

        private static Effet EffetPerte(int valNegatif, string txt)
        {
            var e = new Effet();
            e.Id = Guid.NewGuid();
            e.Type = TypeEffet.PertePoints;
            e.Valeur = valNegatif;
            e.Description = txt;
            return e;
        }

        private static Effet EffetMort(string txt)
        {
            var e = new Effet();
            e.Id = Guid.NewGuid();
            e.Type = TypeEffet.MortInstantanee;
            e.Valeur = 0;
            e.Description = txt;
            return e;
        }

        private static Effet EffetRien(string txt)
        {
            var e = new Effet();
            e.Id = Guid.NewGuid();
            e.Type = TypeEffet.Rien;
            e.Valeur = 0;
            e.Description = txt;
            return e;
        }
    }
}
