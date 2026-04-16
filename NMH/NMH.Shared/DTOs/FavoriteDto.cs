namespace NMH.Shared.DTOs
{
    public class FavoriteDto
    {
        public int Id { get; set; }        // ⭐ AJOUT OBLIGATOIRE
        public int MovieId { get; set; }
        public string? Comment { get; set; }
    }
}