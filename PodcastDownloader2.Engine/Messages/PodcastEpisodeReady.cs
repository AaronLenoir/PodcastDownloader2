using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Messages
{
    public class PodcastEpisodeReady
    {
        public string Path { get; }
        public Episode Episode { get; }

        public PodcastEpisodeReady(string path, Episode episode)
        {
            Path = path;
            Episode = episode;
        }
    }
}
