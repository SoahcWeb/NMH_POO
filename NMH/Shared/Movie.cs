using System;
using System.ComponentModel.DataAnnotations;

namespace NMH.Shared;

/// <summary>
/// Represents a movie entity.
/// </summary>
public class Movie
{
    /// <summary>
    /// Gets or sets the unique identifier of the movie.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the movie.
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release date of the movie.
    /// </summary>
    public DateTime ReleaseDate { get; set; }
}

