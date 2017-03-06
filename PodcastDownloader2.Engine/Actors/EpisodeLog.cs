using Akka.Actor;
using PodcastDownloader2.Engine.Data;
using PodcastDownloader2.Engine.Messages;
using System.IO;

namespace PodcastDownloader2.Engine.Actors
{
    public class EpisodeLog : ReceiveActor
    {
        private IActorRef _bus;
        private string _targetPath;
        private PodcastDatabase _db;

        public EpisodeLog(IActorRef bus, string targetPath)
        {
            _bus = bus;
            _targetPath = Path.Combine(targetPath, "db.sqlite");
            _db = PodcastDatabase.CreateOrOpen(_targetPath);

            Become(Ready);
        }

        public void Ready()
        {
            Receive<PodcastEpisodeDownloaded>(message => HandlePodcastEpisodeDownloaded(message));
            Receive<CheckEpisodeDownloaded>(message => HandleCheckEpisodeDownloaded(message));
            Receive<PodcastEpisodeSkipped>(message => HandlePodcastEpisodeSkipped(message));
        }

        private void HandlePodcastEpisodeSkipped(PodcastEpisodeSkipped message)
        {
            if (message.Reason == EpisodeSkippedReason.TooOld)
            {
                _db.RemoveId(message.Episode.Id);
            }
        }

        private void HandleCheckEpisodeDownloaded(CheckEpisodeDownloaded message)
        {
            var id = message.Episode.Id;
            if (!_db.ContainsId(id))
            {
                _bus.Tell(new EpisodeNotDownloaded(message.Episode));
            } else
            {
                _bus.Tell(new EpisodeAlreadyDownloaded(message.Episode));
            }
        }

        private void HandlePodcastEpisodeDownloaded(PodcastEpisodeDownloaded message)
        {
            var id = message.Episode.Id;
            if (!_db.ContainsId(id))
            {
                _db.AddId(id);
            }
        }
    }
}