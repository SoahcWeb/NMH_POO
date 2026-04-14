namespace NMH.Data
{
    public class Favorite
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int MovieId { get; set; }

        public string? Comment { get; set; }
    }
}