using Akka.Actor;
using PodcastDownloader2.Engine.Actors.GeneralPurpose.Messages;
using System.Collections.Generic;

namespace PodcastDownloader2.Engine.Actors.GeneralPurpose
{
    public class BusActor : ReceiveActor
    {
        private List<IActorRef> _subscribers;

        public BusActor()
        {
            _subscribers = new List<IActorRef>();
            Become(Ready);
        }

        public void Ready()
        {
            Receive<Subscribe>(message => HandleSubscribe(message));
            ReceiveAny(message => Publish(message));
        }

        private void HandleSubscribe(Subscribe message)
        {
            _subscribers.Add(message.Subscriber);
        }

        private void Publish(object message)
        {
            _subscribers.ForEach(subscriber => subscriber.Tell(message));
        }
    }
}
