namespace NMH.Shared.DTOs
{
    public class CastDto
    {
        public string Name { get; set; } = string.Empty;

        public string? ProfilePath { get; set; }

        public string ProfileFullPath =>
            string.IsNullOrEmpty(ProfilePath)
                ? ""
                : $"https://image.tmdb.org/t/p/w500{ProfilePath}";
    }
}