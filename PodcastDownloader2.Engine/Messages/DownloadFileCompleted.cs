using System;

namespace PodcastDownloader2.Engine.Messages
{
    public class DownloadFileCompleted
    {
        public Guid Guid { get; }
        public Uri Uri { get; }
        public string Path { get; }

        public DownloadFileCompleted(Guid guid, Uri uri, string path)
        {
            Guid = guid;
            Uri = uri;
            Path = path;
        }
    }
}
