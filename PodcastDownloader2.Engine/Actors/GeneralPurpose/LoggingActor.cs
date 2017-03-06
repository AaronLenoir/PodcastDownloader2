using Akka.Actor;
using Akka.Event;
using System;

namespace PodcastDownloader2.Engine.Actors.GeneralPurpose
{
    public class LoggingActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public LoggingActor()
        {
            Become(Ready);
        }

        public void Ready()
        {
            Receive<object>(message => HandleMessage(message));
        }

        public void HandleMessage(object message)
        {
            _log.Debug($"Message: '{message}'.");
            Console.WriteLine($"Message: '{message}'.");
        }
    }
}
