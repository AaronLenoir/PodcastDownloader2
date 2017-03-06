using System;

namespace PodcastDownloader2.Engine.Model
{
    public class Episode
    {
        public Podcast Podcast { get; }
        public string Id { get; }
        public DateTimeOffset PublishDate { get; }
        public string Title { get; }
        public Uri MediaUri { get; }

        public Episode(Podcast podcast, string id, DateTimeOffset publishDate, string title, Uri mediaUri)
        {
            Podcast = podcast;
            Id = id;
            PublishDate = publishDate;
            Title = title;
            MediaUri = mediaUri;
        }
    }
}
