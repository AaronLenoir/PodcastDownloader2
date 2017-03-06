namespace PodcastDownloader2.Engine.Model
{
    public class Podcast
    {
        public string Url { get; }
        public string Name { get; }

        public Podcast(string url, string name)
        {
            Url = url;
            Name = name;
        }
    }
}
