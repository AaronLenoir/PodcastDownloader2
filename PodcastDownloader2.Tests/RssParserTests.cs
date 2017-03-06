using Microsoft.VisualStudio.TestTools.UnitTesting;
using PodcastDownloader2.Engine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PodcastDownloader2.Tests
{
    [TestClass]
    public class RssParserTests
    {
        [TestMethod]
        public void ParseRssFeedRiskyBusinessShouldFind20Episodes()
        {
            const int expectedCount = 1323;
            var feedPath = @"Assets\PodcastRss\risky-business.xml";
            var count = CountEpisodes(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void ParseRssFeedDotNetRocksShouldFind1323Episodes()
        {
            const int expectedCount = 1323;
            var feedPath = @"Assets\PodcastRss\dotnetrocks.xml";
            var count = CountEpisodes(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void ParseRssFeedDotNetRocksShouldFind30Aug2002ForFirstEpisode()
        {
            DateTime expectedDate = new DateTime(2002, 8, 30, 0, 0, 0);
            var feedPath = @"Assets\PodcastRss\dotnetrocks.xml";
            var episode = GetEpisodes(feedPath).Last();
            Assert.AreEqual(expectedDate, episode.PublishDate);
        }

        [TestMethod]
        public void ParseRssFeedRiskyBusinessShouldFindIssue417ForFirstEpisodeTitle()
        {
            string expectedTitle = "Risky Business #417 -- PlayPen ruling to let FBI off leash?";
            var feedPath = @"Assets\PodcastRss\risky-business.xml";
            var episode = GetEpisodes(feedPath).Last();
            Assert.AreEqual(expectedTitle, episode.Title);
        }

        [TestMethod]
        public void ParseRssFeedDotNetRocksShouldFindPatHyndsForFirstEpisodeTitle()
        {
            string expectedTitle = "Pat Hynds";
            var feedPath = @"Assets\PodcastRss\dotnetrocks.xml";
            var episode = GetEpisodes(feedPath).Last();
            Assert.AreEqual(expectedTitle, episode.Title);
        }

        [TestMethod]
        public void ParseRssFeedDotNetRocksShouldFindIdForFirstEpisode()
        {
            string expectedId = "http://www.dotnetrocks.com/default.aspx?ShowNum=1";
            var feedPath = @"Assets\PodcastRss\dotnetrocks.xml";
            var episode = GetEpisodes(feedPath).Last();
            Assert.AreEqual(expectedId, episode.Id);
        }

        [TestMethod]
        public void ParseRssFeedDotNetRocksShouldFindUriForFirstEpisode()
        {
            string expectedUri = "http://www.podtrac.com/pts/redirect.mp3/s3.amazonaws.com/dnr/dotnetrocks_0001_pat_hynds.mp3";
            var feedPath = @"Assets\PodcastRss\dotnetrocks.xml";
            var episode = GetEpisodes(feedPath).Last();
            Assert.AreEqual(expectedUri, episode.MediaUri.AbsoluteUri);
        }

        [TestMethod]
        public void ParseRssFeedHanselminutesShouldFind20Episodes()
        {
            const int expectedCount = 20;
            var feedPath = @"Assets\PodcastRss\hanselminutes.xml";
            var count = CountEpisodes(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void ParseRssFeedRadiolabShouldFind143EpisodesAndOneError()
        {
            const int expectedCount = 143;
            const int expectedErrors = 1;
            var feedPath = @"Assets\PodcastRss\radiolab.xml";
            var count = CountEpisodes(feedPath);
            var errors = CountErrors(feedPath);
            Assert.AreEqual(expectedCount, count);
            Assert.AreEqual(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseRssFeedScienceTalkShouldFind25Episodes()
        {
            const int expectedCount = 25;
            var feedPath = @"Assets\PodcastRss\sciencetalk.xml";
            var count = CountEpisodes(feedPath);
            var errors = CountErrors(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void ParseRssFeedSeRadioShouldFind264Episodes()
        {
            const int expectedCount = 264;
            var feedPath = @"Assets\PodcastRss\seradio.xml";
            var count = CountEpisodes(feedPath);
            var errors = CountErrors(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void ParseRssFeedThisDevLifeShouldFind27Episodes()
        {
            const int expectedCount = 27;
            var feedPath = @"Assets\PodcastRss\thisdevlife.xml";
            var count = CountEpisodes(feedPath);
            var errors = CountErrors(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void ParseRssFeedHelloWorldShouldFind72Episodes()
        {
            const int expectedCount = 72;
            var feedPath = @"Assets\PodcastRss\helloworld.xml";
            var count = CountEpisodes(feedPath);
            var errors = CountErrors(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void ParseRssFeedStarTalkShouldFind250Episodes()
        {
            const int expectedCount = 250;
            var feedPath = @"Assets\PodcastRss\startalk.xml";
            var count = CountEpisodes(feedPath);
            var errors = CountErrors(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        [TestMethod]
        public void ParseRssFeedChangeLogShouldFind211Episodes()
        {
            const int expectedCount = 211;
            var feedPath = @"Assets\PodcastRss\changelog.xml";
            var count = CountEpisodes(feedPath);
            var errors = CountErrors(feedPath);
            Assert.AreEqual(expectedCount, count);
        }

        private IEnumerable<Episode> GetEpisodes(string feedPath)
        {
            var xml = File.ReadAllText(feedPath);
            var podcast = new Engine.Model.Podcast(string.Empty, string.Empty);
            var episodes = new List<Episode>();
            using (var reader = new Engine.Utilities.PodcastRssReader(xml, podcast))
            {
                while (reader.Read())
                {
                    if (reader.Error == null && reader.Episode != null)
                    {
                        episodes.Add(reader.Episode);
                    }
                }
            }

            return episodes;
        }

        private int CountEpisodes(string feedPath)
        {
            var counter = 0;
            var xml = File.ReadAllText(feedPath);
            var podcast = new Engine.Model.Podcast(string.Empty, string.Empty);
            using (var reader = new Engine.Utilities.PodcastRssReader(xml, podcast))
            {
                while (reader.Read())
                {
                    if (reader.Error == null && reader.Episode != null) { 
                        counter += 1;
                    } else
                    {
                        ;
                    }
                }
            }

            return counter;
        }

        private int CountErrors(string feedPath)
        {
            var counter = 0;
            var xml = File.ReadAllText(feedPath);
            var podcast = new Engine.Model.Podcast(string.Empty, string.Empty);
            using (var reader = new Engine.Utilities.PodcastRssReader(xml, podcast))
            {
                while (reader.Read())
                {
                    if (reader.Error != null && reader.Episode == null)
                    {
                        counter += 1;
                    }
                }
            }

            return counter;
        }

    }
}
