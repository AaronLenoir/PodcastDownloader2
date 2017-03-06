using PodcastDownloader2.Engine.Model;
using System;

namespace PodcastDownloader2.Engine.Messages
{
    public class PodcastFailed
    {
        public Podcast Podcast { get; }
        public Exception Exception { get; }

        public PodcastFailed(Podcast podcast, Exception exception)
        {
            Podcast = podcast;
            Exception = exception;
        }
    }
}
