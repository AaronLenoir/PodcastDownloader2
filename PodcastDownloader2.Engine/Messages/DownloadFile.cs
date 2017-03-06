using System;

namespace PodcastDownloader2.Engine.Messages
{
    public class DownloadFile
    {
        public Guid Guid { get; }
        public Uri Uri { get; }

        public DownloadFile(Guid guid, Uri uri)
        {
            Guid = guid;
            Uri = uri;
        }

        public DownloadFile(Guid guid, string uriString) : this(guid, new Uri(uriString)) { }
    }
}