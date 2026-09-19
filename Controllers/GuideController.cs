using Microsoft.AspNetCore.Mvc;
using TertiaryInstitutions.Models;

namespace TertiaryInstitutions.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GuideController : ControllerBase
{
    internal const string PlaylistId = "PLNxjvIyFxGMueX_4E5RtoETHnmCXvfRDJ";
    internal const string StartVideoId = "bGiofFfb678";
    internal const string WatchUrl = $"https://www.youtube.com/watch?v={StartVideoId}&list={PlaylistId}";

    private static readonly GuideEpisode[] EpisodeList =
    [
        new() { Number = 1, Title = "Humble Beginnings", Duration = "4:02", VideoId = "bGiofFfb678" },
        new() { Number = 2, Title = "Big Dreams", Duration = "4:04", VideoId = "36ty2syCLNw" },
        new() { Number = 3, Title = "Self-esteem", Duration = "3:39", VideoId = "BrUrKtzWeKM" },
        new() { Number = 4, Title = "Excuses", Duration = "3:50", VideoId = "cmovME3Oj0g" },
        new() { Number = 5, Title = "Things Don't Come Easy", Duration = "4:40", VideoId = "rP_SmV56N7A" },
        new() { Number = 6, Title = "The 4th Industrial Revolution (4IR)", Duration = "5:16", VideoId = "upEQaUHyEJo" },
        new() { Number = 7, Title = "What is Insurance", Duration = "5:41", VideoId = "nytQvrQvO5g" },
        new() { Number = 8, Title = "Developing an App", Duration = "5:06", VideoId = "OQ1k_xlnpo4" },
        new() { Number = 9, Title = "All About Learnerships And Internships", Duration = "5:40", VideoId = "IPyrYSFkZTc" },
        new() { Number = 10, Title = "SMART Goals", Duration = "5:04", VideoId = "F7lXNmU7QSw" },
        new() { Number = 11, Title = "From Call Centre to Corner Office", Duration = "6:12", VideoId = "2ZhAImid8RY" },
        new() { Number = 12, Title = "What! Marketing In Insurance", Duration = "5:26", VideoId = "eDtNxtnQMxg" },
        new() { Number = 13, Title = "Bean counting…is that what Chartered Accountants do?", Duration = "4:21", VideoId = "-kZ_gvZ9KOc" },
        new() { Number = 14, Title = "Financial Planning for a bike, What is Mkhulu up to?", Duration = "4:56", VideoId = "Q-7_cECdmGs" },
        new() { Number = 15, Title = "Actuary and a Vrrrpha", Duration = "5:16", VideoId = "ECthxrTNXeU" },
        new() { Number = 16, Title = "Self-knowledge. It's all about understanding who you are", Duration = "6:16", VideoId = "sTHYi0MSnvY" },
        new() { Number = 17, Title = "Taking care of what is important…our loved ones.", Duration = "5:14", VideoId = "h6n1mo4-ww4" },
        new() { Number = 18, Title = "Volunteering for a better future", Duration = "4:32", VideoId = "6fkGNU7M3dI" },
        new() { Number = 19, Title = "Funding your studies begins with you!", Duration = "4:58", VideoId = "Akrgdx0nUtw" },
        new() { Number = 20, Title = "Preparing for an interview", Duration = "4:32", VideoId = "TeGv3J4uF14" },
        new() { Number = 21, Title = "Attending an Interview", Duration = "5:35", VideoId = "_RXELbH2t7w" },
        new() { Number = 22, Title = "Lifelong learning. Choose a learning platform and start growing", Duration = "5:59", VideoId = "lTdjb0rHHTc" },
        new() { Number = 23, Title = "The importance of reading books", Duration = "4:11", VideoId = "D8cptWW8R6s" },
        new() { Number = 24, Title = "Maths and what it helps with", Duration = "5:03", VideoId = "NNYZ3rO5MuE" },
        new() { Number = 25, Title = "What is a loss adjuster?", Duration = "4:10", VideoId = "abk374BUMeY" },
        new() { Number = 26, Title = "Funeral homes", Duration = "3:49", VideoId = "izBq8VinDVg" },
        new() { Number = 27, Title = "Principles of success", Duration = "5:08", VideoId = "OG-OS6MNv7Q" },
        new() { Number = 28, Title = "Travel broadens your horizon", Duration = "6:05", VideoId = "Ooa-9RaS6Z8" },
    ];

    /// <summary>
    /// Gets the video guide (the "After School" YouTube playlist) that walks users through
    /// planning their study and career path, including its episode list.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GuideResponse), StatusCodes.Status200OK)]
    public ActionResult<GuideResponse> GetGuide()
    {
        return Ok(new GuideResponse
        {
            Title = "After School",
            Description = "A short-video series on planning your study and career path: goals, interviews, funding your studies, learnerships and more.",
            PlaylistId = PlaylistId,
            StartVideoId = StartVideoId,
            WatchUrl = WatchUrl,
            PlaylistUrl = $"https://www.youtube.com/playlist?list={PlaylistId}",
            EmbedUrl = $"https://www.youtube.com/embed/{StartVideoId}?list={PlaylistId}",
            Episodes = EpisodeList
        });
    }
}
