using PodcastDownloader2.Engine.Actors;
using PodcastDownloader2.Engine.Model;
using System;
using System.Collections.Generic;
using PodcastDownloader2.Engine.Messages;
using System.Linq;

namespace PodcastDownloader2.CommandLine
{
    public class EpisodeProgress
    {
        public Episode Episode { get; }
        public double Progress { get; set; }
        public EpisodeProgress(Episode episode, double progress)
        {
            Episode = episode;
            Progress = progress;
        }

        public override bool Equals(object obj)
        {
            var episodeProgress = obj as EpisodeProgress;
            if (episodeProgress == null) { return false; }
            return episodeProgress.Episode.Id == Episode.Id;
        }

        public override int GetHashCode()
        {
            return Episode.Id.GetHashCode();
        }
    }

    public class PodcastStatus
    {
        public Podcast Podcast { get; }
        public int TotalEpisodes { get; set; }
        public int CompletedEpisodes { get; set; }
        public int SkippedEpisodes { get; set; }
        public List<EpisodeProgress> Episodes { get; }
        public string Error { get; set; }

        public PodcastStatus(Podcast podcast)
        {
            Podcast = podcast;
            TotalEpisodes = 0;
            CompletedEpisodes = 0;
            SkippedEpisodes = 0;
            Episodes = new List<EpisodeProgress>();
            Error = string.Empty;
        }
    }

    public class StatusTracker : IBridgeReceiver
    {
        private List<PodcastStatus> _statusses;

        public StatusTracker()
        {
            _statusses = new List<PodcastStatus>();
        }

        public void EpisodeDownloadStarted(Episode episode)
        {
            var status = _statusses.SingleOrDefault(x => x.Podcast.Url == episode.Podcast.Url);

            if (status != null)
            {
                status.Episodes.Add(new EpisodeProgress(episode, 0));
            }

            OutputSummary();
        }

        public void EpisodeDownloadProgressed(Episode episode, double progress)
        {
            var status = _statusses.SingleOrDefault(x => x.Podcast.Url == episode.Podcast.Url);

            if (status != null)
            {
                var episodeProgress = status.Episodes.SingleOrDefault(x => x.Episode.Id == episode.Id);
                if (episodeProgress == null)
                {
                    status.Episodes.Add(new EpisodeProgress(episode, progress));
                } else
                {
                    episodeProgress.Progress = progress;
                }
            }

            OutputSummary();
        }

        public void EpisodeDownloaded(Episode episode)
        {
            var status = _statusses.SingleOrDefault(x => x.Podcast.Url == episode.Podcast.Url);

            if (status != null) {
                status.CompletedEpisodes += 1;
                status.Episodes.Remove(status.Episodes.Single(x => x.Episode.Id == episode.Id));
            }
            OutputSummary();
        }

        public void EpisodeSkipped(Episode episode, EpisodeSkippedReason reason)
        {
            var status = _statusses.SingleOrDefault(x => x.Podcast.Url == episode.Podcast.Url);

            if (status != null) {
                status.SkippedEpisodes += 1;
                status.Episodes.Remove(status.Episodes.SingleOrDefault(x => x.Episode.Id == episode.Id));
            }
            OutputSummary();
        }

        public void PodcastFailed(Podcast podcast, Exception exception)
        {
            var status = _statusses.SingleOrDefault(x => x.Podcast.Url == podcast.Url);
            if (status == null)
            {
                status = new PodcastStatus(podcast);
                status.Error = exception.Message;
                if (exception.InnerException != null) { status.Error += " - " + exception.InnerException.Message; }
                _statusses.Add(status);
            }

            OutputSummary();
        }

        public void TotalEpisodesKnown(Podcast podcast, int total)
        {
            var status = _statusses.SingleOrDefault(x => x.Podcast.Url == podcast.Url);
            if (status == null)
            {
                status = new PodcastStatus(podcast);
                _statusses.Add(status);
            }

            status.TotalEpisodes = total;

            OutputSummary();
        }

        public void PodcastDone(Podcast podcast)
        {
        }

        public void AllDone()
        {
            OutputSummary(true);
            Console.WriteLine("All done ...");
        }

        private DateTime _lastUpdate = DateTime.Now;

        private void OutputSummary()
        {
            OutputSummary(false);
        }

        private void OutputSummary(bool force)
        {
            ClearScreenOnce();
            if (DateTime.Now.Subtract(_lastUpdate).TotalMilliseconds < 500 && !force) { return; }

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            foreach(var status in _statusses)
            {
                WriteLinePadded($"{status.Podcast.Name}: {status.SkippedEpisodes + status.CompletedEpisodes} / {status.TotalEpisodes}");
                if (string.IsNullOrEmpty(status.Error))
                {
                    foreach(var episodeProgress in status.Episodes)
                    {
                        WriteLinePadded($"  Downloading: '{episodeProgress.Episode.Title}' ({episodeProgress.Progress} %)", ConsoleColor.Green);
                    }

                    if (status.Episodes.Count == 0)
                    {
                        if (status.TotalEpisodes == (status.SkippedEpisodes + status.CompletedEpisodes))
                        {
                            WriteLinePadded("  Nothing to do", ConsoleColor.DarkGray);
                        }
                        else
                        {
                            WriteLinePadded("  Pending ...", ConsoleColor.Yellow);
                        }
                    }
                } else
                {
                    WriteLinePadded($"  Error: '{status.Error}'.", ConsoleColor.Red);
                }
            }

            _lastUpdate = DateTime.Now;
        }

        private void WriteLinePadded(string text)
        {
            var padding = string.Empty;
            var padLength = Console.WindowWidth - text.Length - 1;
            if (padLength > 0)
            {
                padding = new string(' ', padLength);
            }
            Console.WriteLine($"{text}{padding}");
        }

        private void WriteLinePadded(string text, ConsoleColor color)
        {
            var originalColour = Console.ForegroundColor;
            Console.ForegroundColor = color;
            WriteLinePadded(text);
            Console.ForegroundColor = originalColour;
        }

        private void ClearConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private bool _screenCleared = false;
        private void ClearScreenOnce()
        {
            if (_screenCleared) { return; }
            Console.Clear();
            _screenCleared = true;
        }

    }
}
