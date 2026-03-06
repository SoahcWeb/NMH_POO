namespace NMH.Shared.DTOs
{
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class GenresResponseDto
    {
        public List<GenreDto>? Genres { get; set; }
    }
}