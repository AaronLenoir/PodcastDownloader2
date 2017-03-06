using Akka.Actor;

namespace PodcastDownloader2.Engine.Actors.GeneralPurpose.Messages
{
    /// <summary>
    /// Subscribe to the bus as a listener.
    /// </summary>
    public class Subscribe
    {
        public IActorRef Subscriber { get; }

        public Subscribe(IActorRef subscriber)
        {
            Subscriber = subscriber;
        }
    }
}
