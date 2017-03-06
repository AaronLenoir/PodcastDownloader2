using System;
using System.Threading.Tasks;
using Akka.Actor;
using PodcastDownloader2.Engine.Messages;
using PodcastDownloader2.Engine.Model;

namespace PodcastDownloader2.Engine.Actors
{
    public interface IBridgeReceiver
    {
        void PodcastDone(Podcast podcast);
        void EpisodeDownloadStarted(Episode episode);
        void AllDone();
        void EpisodeDownloadProgressed(Episode episode, double progress);
        void EpisodeDownloaded(Episode episode);
        void TotalEpisodesKnown(Podcast podcast, int total);
        void EpisodeSkipped(Episode episode, EpisodeSkippedReason reason);
        void PodcastFailed(Podcast podcast, Exception exception);
    }

    public class BridgeActor : ReceiveActor
    {
        private IActorRef _bus;
        private IBridgeReceiver _receiver;

        public BridgeActor(IActorRef bus, IBridgeReceiver receiver)
        {
            if (receiver == null) {
                throw new System.ArgumentNullException(nameof(receiver));
            }

            _bus = bus;
            _receiver = receiver;

            Become(Ready);
        }

        public void Ready()
        {
            Receive<PodcastDone>(message => _receiver.PodcastDone(message.Podcast));
            Receive<PodcastEpisodeStarted>(message => _receiver.EpisodeDownloadStarted(message.Episode));
            Receive<PodcastEpisodeProgressed>(message => _receiver.EpisodeDownloadProgressed(message.Episode, message.Progress));
            Receive<PodcastFailed>(message => _receiver.PodcastFailed(message.Podcast, message.Exception));
            Receive<PodcastEpisodeDownloaded>(message => _receiver.EpisodeDownloaded(message.Episode));
            Receive<TotalPodcastEpisodesKnown>(message => _receiver.TotalEpisodesKnown(message.Podcast, message.Total));
            Receive<PodcastEpisodeSkipped>(message => _receiver.EpisodeSkipped(message.Episode, message.Reason));
            Receive<string>(message => _receiver.AllDone());
        }
    }
}
