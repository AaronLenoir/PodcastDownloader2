using System;

namespace PodcastDownloader2.Engine.Messages
{
    public class DownloadProgressed
    {
        public Guid Guid { get; }
        public Uri Uri { get; }
        public double Progress { get; }

        public DownloadProgressed(Guid guid, Uri uri, double progress)
        {
            Guid = guid;
            Uri = uri;
            Progress = progress;
        }

        public override string ToString()
        {
            return $"Download '{Uri}' progressed: {Progress}%";
        }
    }
}
