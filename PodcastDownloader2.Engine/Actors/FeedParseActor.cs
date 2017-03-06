using Akka.Actor;
using PodcastDownloader2.Engine.Messages;
using PodcastDownloader2.Engine.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace PodcastDownloader2.Engine.Actors
{
    public class FeedParseActor : ReceiveActor
    {
        private readonly IActorRef _bus;

        public FeedParseActor(IActorRef bus)
        {
            _bus = bus;

            Become(Ready);
        }

        public void Ready()
        {
            Receive<PodcastFeedReady>(message => HandlePodcastFeedReady(message));
        }

        private void HandlePodcastFeedReady(PodcastFeedReady message)
        {
            // Parse feed items
            // Send out messages per feed item
            // TODO: Put SyndicationFeed in own library?
            // TODO: SyndicationFeed fails to parse dotNetRocks XML Feed?
            //       Use our own ugly parser?
            try
            {
                var episodesFound = new List<PodcastEpisodeFound>();
                using (var reader = new PodcastRssReader(message.FeedXml, message.Podcast))
                {
                    while(reader.Read())
                    {
                        if (reader.Error == null)
                        {
                            episodesFound.Insert(0, new PodcastEpisodeFound(reader.Episode));
                        } else
                        {
                            // Log somewhere we could not get required info for an episode? Not sure.
                        }
                    }
                }

                _bus.Tell(new TotalPodcastEpisodesKnown(message.Podcast, episodesFound.Count));
                episodesFound.ForEach(_bus.Tell);
            } catch (XmlException ex)
            {
                _bus.Tell(new PodcastFailed(message.Podcast, ex));
            }
        }

        private SyndicationFeed GetSyndicationFeed(string xml)
        {
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                return SyndicationFeed.Load(reader);
            }
        }
    }
}
