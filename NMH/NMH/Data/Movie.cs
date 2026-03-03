namespace NMH.Shared
{
    public class Movie
    {
        public int Id { get; set; }             // Auto-incrémenté
        public string Title { get; set; } = ""; // Obligatoire
        public DateTime ReleaseDate { get; set; }
    }
}