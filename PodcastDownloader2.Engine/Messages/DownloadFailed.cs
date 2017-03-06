using System;

namespace PodcastDownloader2.Engine.Messages
{
    public class DownloadFailed
    {
        public Guid Guid { get; }
        public Uri Uri { get; }
        public Exception Exception { get; }

        public DownloadFailed(Guid guid, Uri uri, Exception exception)
        {
            Guid = guid;
            Uri = uri;
            Exception = exception;
        }
    }
}
