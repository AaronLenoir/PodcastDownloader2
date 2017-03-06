using System;
using Akka.Actor;
using PodcastDownloader2.Engine.Messages;
using System.Collections.Generic;
using PodcastDownloader2.Engine.Actors.GeneralPurpose;

namespace PodcastDownloader2.Engine.Actors
{
    public class DownloadCoordinator : ReceiveActor
    {
        private readonly Props _downloadActorProps;

        private IActorRef _requestor;
        private Dictionary<string, IActorRef> _downloadActors;

        public DownloadCoordinator(Props downloadActorProps, IActorRef requestor)
        {
            _downloadActorProps = downloadActorProps;
            _downloadActors = new Dictionary<string, IActorRef>();
            _requestor = requestor;
            Become(Ready);
        }

        public void Ready()
        {
            Receive<DownloadFile>(message => HandleDownloadFile(message));
            Receive<FileDownloadActor.DownloadCompleted>(message => HandleDownloadEvent(message));
            Receive<FileDownloadActor.DownloadProgressed>(message => HandleDownloadEvent(message));
            Receive<FileDownloadActor.DownloadFailed>(message => HandleDownloadEvent(message));
        }

        private void HandleDownloadFile(DownloadFile message)
        {
            var downloadActor = GetOrCreateDownloadActor(message.Uri);
            downloadActor.Tell(new FileDownloadActor.RequestDownload(message.Guid, message.Uri));
        }

        private void HandleDownloadEvent(object message)
        {
            var downloadCompletedMessage = message as FileDownloadActor.DownloadCompleted;
            var downloadProgressedMessage = message as FileDownloadActor.DownloadProgressed;
            var downloadFailedMessage = message as FileDownloadActor.DownloadFailed;

            if (downloadCompletedMessage != null)
            {
                _requestor.Tell(new DownloadFileCompleted(downloadCompletedMessage.Guid, 
                                                              downloadCompletedMessage.Uri,
                                                              downloadCompletedMessage.Path));
            }
            if (downloadProgressedMessage != null)
            {
                _requestor.Tell(new DownloadProgressed(downloadProgressedMessage.Guid, 
                                                           downloadProgressedMessage.Uri,
                                                           downloadProgressedMessage.Progress));
            }
            if (downloadFailedMessage != null)
            {
                _requestor.Tell(new DownloadFailed(downloadFailedMessage.Guid, 
                                                       downloadFailedMessage.Uri, 
                                                       downloadFailedMessage.Error));
            }
        }

        private IActorRef GetOrCreateDownloadActor(Uri uri)
        {
            if (!_downloadActors.ContainsKey(uri.Host))
            {
                _downloadActors.Add(uri.Host, 
                                    Context.System.ActorOf(_downloadActorProps));
            }

            return _downloadActors[uri.Host];
        }
    }
}
