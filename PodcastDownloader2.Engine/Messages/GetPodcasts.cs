using PodcastDownloader2.Engine.Model;
using System.Collections.Generic;

namespace PodcastDownloader2.Engine.Messages
{
    public class GetPodcasts
    {
        public List<Podcast> Podcasts { get; }
        public GetPodcasts(List<Podcast> podcasts)
        {
            Podcasts = podcasts;
        }
    }
}
