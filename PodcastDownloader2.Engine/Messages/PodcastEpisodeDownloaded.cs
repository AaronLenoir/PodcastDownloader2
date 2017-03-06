using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class PodcastEpisodeDownloaded
    {
        public string Path { get; }
        public Episode Episode { get; }

        public PodcastEpisodeDownloaded(string path, Episode episode)
        {
            Path = path;
            Episode = episode;
        }
    }
}
