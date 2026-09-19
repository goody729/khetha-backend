namespace TertiaryInstitutions.Models;

/// <summary>
/// A video guide (YouTube playlist) that walks users through using the platform.
/// </summary>
public class GuideResponse
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>The YouTube playlist id.</summary>
    public string PlaylistId { get; set; } = string.Empty;

    /// <summary>The id of the video the guide opens on.</summary>
    public string StartVideoId { get; set; } = string.Empty;

    /// <summary>Link to watch the guide on YouTube (opens on the start video, within the playlist).</summary>
    public string WatchUrl { get; set; } = string.Empty;

    /// <summary>Link to the full playlist page on YouTube.</summary>
    public string PlaylistUrl { get; set; } = string.Empty;

    /// <summary>URL suitable for an iframe <c>src</c> to embed the playlist player.</summary>
    public string EmbedUrl { get; set; } = string.Empty;

    /// <summary>Episodes in the series, in watch order.</summary>
    public IReadOnlyList<GuideEpisode> Episodes { get; set; } = Array.Empty<GuideEpisode>();
}

/// <summary>
/// A single episode in the guide playlist.
/// </summary>
public class GuideEpisode
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;

    /// <summary>Running time as m:ss.</summary>
    public string Duration { get; set; } = string.Empty;

    /// <summary>The YouTube video id for this episode.</summary>
    public string VideoId { get; set; } = string.Empty;

    /// <summary>Cover image (thumbnail) for the episode, served by YouTube.</summary>
    public string ThumbnailUrl => $"https://img.youtube.com/vi/{VideoId}/hqdefault.jpg";
}
