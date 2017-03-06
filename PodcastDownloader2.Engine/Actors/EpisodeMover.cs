using Akka.Actor;
using PodcastDownloader2.Engine.Messages;
using PodcastDownloader2.Engine.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PodcastDownloader2.Engine.Actors
{
    public class EpisodeMover : ReceiveActor
    {
        private IActorRef _bus;
        private string _targetPath;

        public EpisodeMover(IActorRef bus, string targetPath)
        {
            _bus = bus;
            _targetPath = GetCurrentDayFolder(targetPath);

            Become(Ready);
        }

        public void Ready()
        {
            Receive<PodcastEpisodeDownloaded>(message => HandlePodcastEpisodeDownloaded(message));
        }

        private void HandlePodcastEpisodeDownloaded(PodcastEpisodeDownloaded message)
        {
            var finalLocation = Path.Combine(_targetPath, GetAudioFileName(message.Episode));

            File.Delete(finalLocation);
            File.Move(message.Path, finalLocation);

            _bus.Tell(new PodcastEpisodeReady(finalLocation, message.Episode));
        }

        private string GetAudioFileName(Episode episode)
        {
            return MakeValidFilename($"{episode.Podcast.Name} - {episode.PublishDate.ToString("yyyyMMddHH")} - {episode.Title}.mp3");
        }

        private string GetCurrentDayFolder(string targetPath)
        {
            var path = Path.Combine(targetPath, DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        static string MakeValidFilename(string filename)
        {
            var sb = new StringBuilder(filename);

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sb.Replace(c, '_');
            }

            if (sb.Length > 0 && sb[0] == '.')
            {
                sb.Remove(0, 1);
                sb.Insert(0, "dot");
            }

            return sb.ToString();
        }
    }
}
