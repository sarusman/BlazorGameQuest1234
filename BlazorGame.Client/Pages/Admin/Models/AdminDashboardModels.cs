namespace BlazorGame.Client.Pages.Admin.Models
{
    public class LeaderboardEntry
    {
        public string Pseudo { get; set; } = "";
        public int Score { get; set; }
    }

    public class PartieAdminView
    {
        public Guid Id { get; set; }
        public Guid JoueurId { get; set; }
        public Guid DonjonId { get; set; }
        public int ScoreFinal { get; set; }
        public bool EstTerminee { get; set; }
    }
}
