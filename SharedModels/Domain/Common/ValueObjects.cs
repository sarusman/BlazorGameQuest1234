namespace SharedModels.Domain.Common
{
    /// <summary>Points d’un joueur.</summary>
    public record Points(int Valeur);

    /// <summary>Niveau de difficulté.</summary>
    public enum Difficulte { Facile = 1, Normal = 2, Difficile = 3, Mortel = 4 }

    /// <summary>Type de salle.</summary>
    public enum TypeSalle { Combat, Coffre, Piege, Rencontre, Enigme, Repos }

    /// <summary>Type de choix possible.</summary>
    public enum TypeChoix { Combattre, Fouiller, Fuir, Ouvrir, Ignorer, UtiliserObjet, Parler, Resolver }

    /// <summary>Type d’effet appliqué.</summary>
    public enum TypeEffet { GainPoints, PertePoints, DonnerObjet, RetirerObjet, MortInstantanee, Rien }

    /// <summary>Rareté d’un objet.</summary>
    public enum Rareté { Commun, Rare, Epique, Legendaire }
}