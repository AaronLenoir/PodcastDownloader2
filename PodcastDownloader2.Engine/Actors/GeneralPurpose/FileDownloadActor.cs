using Akka.Actor;
using System;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace PodcastDownloader2.Engine.Actors.GeneralPurpose
{
    public class FileDownloadActor : ReceiveActor, IWithUnboundedStash
    {
        #region State

        private Guid _currentGuid;
        private Uri _currentUri;
        private string _currentTargetPath;
        private IActorRef _currentDownloadRequestor;
        private IActorRef _self;
        public IStash Stash { get; set; }
        private readonly WebClient _client = new WebClient();

        #endregion

        #region Initialization

        public FileDownloadActor()
        {
            _self = Self;
            _client.DownloadProgressChanged += HandleWebClientDownloadProgressChanged;
            _client.DownloadFileCompleted += HandleWebClientDownloadCompleted;
            Become(Ready);
        }

        #endregion

        #region Messages

        public class RequestDownload
        {
            public Guid Guid { get; }
            public Uri Uri { get; private set; }
            public RequestDownload(Guid guid, Uri uri)
            {
                Guid = guid;
                Uri = uri;
            }
        }

        public class DownloadStarted
        {
            public Guid Guid { get; }
            public Uri Uri { get; private set; }
            public DownloadStarted(Guid guid, Uri uri)
            {
                Guid = guid;
                Uri = uri;
            }
        }

        public class DownloadCompleted
        {
            public Guid Guid { get; }
            public Uri Uri { get; private set; }
            public string Path { get; private set; }
            public DownloadCompleted(Guid guid, Uri uri, string path)
            {
                Guid = guid;
                Uri = uri;
                Path = path;
            }
        }

        public class DownloadFailed
        {
            public Guid Guid { get; }
            public Uri Uri { get; private set; }
            public Exception Error { get; private set; }
            public DownloadFailed(Guid guid, Uri uri, Exception error)
            {
                Guid = guid;
                Uri = uri;
                Error = error;
            }
        }

        public class DownloadProgressed
        {
            public Guid Guid { get; }
            public Uri Uri { get; private set; }
            public double Progress { get; private set; }
            public DownloadProgressed(Guid guid, Uri uri, double progress)
            {
                Guid = guid;
                Uri = uri;
                Progress = progress;
            }
        }

        #endregion

        #region States

        public void Ready()
        {
            Receive<RequestDownload>(message =>
            {
                HandleDownloadRequest(message);
                Become(Downloading);
            });
        }

        public void Downloading()
        {
            Receive<RequestDownload>(message =>
            {
                Stash.Stash();
            });
            Receive<DownloadCompleted>(message =>
            {
                Become(Ready);
                Stash.UnstashAll();
            });
            Receive<DownloadFailed>(message =>
            {
                Become(Ready);
                Stash.UnstashAll();
            });
        }

        #endregion

        #region Handlers

        private void HandleDownloadRequest(RequestDownload message)
        {
            _currentGuid = message.Guid;
            _currentUri = message.Uri;
            _currentTargetPath = Path.GetTempFileName();
            _currentDownloadRequestor = Sender;

            _currentDownloadRequestor.Tell(new DownloadStarted(_currentGuid, _currentUri));

            StartDownload();
        }

        #endregion

        #region Helper Functions

        private void StartDownload()
        {
            _client.DownloadFileAsync(_currentUri, _currentTargetPath);
            Become(Downloading);
        }

        private void HandleWebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var progress = 0.0;
            if (e.TotalBytesToReceive > 0)
            {
                progress = Math.Round(((double)e.BytesReceived / (double)e.TotalBytesToReceive) * 100, 2);
            }

            _currentDownloadRequestor.Tell(new DownloadProgressed(_currentGuid, _currentUri, progress));
        }

        private void HandleWebClientDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            object resultMessage = GetResultMessageFromEventArgs(e);

            _currentDownloadRequestor.Tell(resultMessage);
            _self.Tell(resultMessage);
        }

        private object GetResultMessageFromEventArgs(AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                return new DownloadCompleted(_currentGuid, _currentUri, _currentTargetPath);
            }

            return new DownloadFailed(_currentGuid, _currentUri, e.Error);
        }

        #endregion
    }
}
