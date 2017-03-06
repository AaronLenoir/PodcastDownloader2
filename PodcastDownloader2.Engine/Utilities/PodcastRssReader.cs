using PodcastDownloader2.Engine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PodcastDownloader2.Engine.Utilities
{
    public class PodcastRssReader : IDisposable
    {
        private XmlReader _reader;
        private readonly Podcast _podcast;
        private Episode _currentEpisode;
        private Exception _currentError;

        public PodcastRssReader(string rssXml, Podcast podcast)
        {
            _reader = XmlReader.Create(new StringReader(rssXml));
            _podcast = podcast;
        }

        public bool Read()
        {
            while(_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element && _reader.Name == "item")
                {
                    try
                    {
                        _currentEpisode = ParseEpisode(_reader);
                        _currentError = null;
                    } catch (Exception ex)
                    {
                        _currentEpisode = null;
                        _currentError = ex;
                    }
                    return true;
                }
            }

            return false;
        }

        public Episode Episode
        {
            get
            {
                return _currentEpisode;
            }
        }

        public Exception Error
        {
            get
            {
                return _currentError;
            }
        }

        private Episode ParseEpisode(XmlReader reader)
        {
            string title = string.Empty;
            string id = string.Empty;
            string link = string.Empty;
            DateTimeOffset publishDate = DateTimeOffset.MinValue;
            Uri mediaUri = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "title")
                {
                    title = reader.ReadElementContentAsString().Trim();
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "guid")
                {
                    id = reader.ReadElementContentAsString();
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "link")
                {
                    link = reader.ReadElementContentAsString();
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "pubDate")
                {
                    DateTimeOffset tempDate;
                    if (!TryParseCrazyRssDate(reader.ReadElementContentAsString(), out tempDate))
                    {
                        throw new Exception("Could not find episode publish date.");
                    }

                    publishDate = tempDate;
                }
                if (reader.NodeType == XmlNodeType.Element && (reader.Name == "enclosure" || reader.Name == "media:content"))
                {
                    Uri tempUri;
                    if (TryParseMediaUriFromEnclosure(reader, out tempUri))
                    {
                        mediaUri = tempUri;
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "item")
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(link))
            {
                throw new Exception("Could not find episode ID.");
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new Exception("Could not find episode Title.");
            }

            if (mediaUri == null)
            {
                throw new Exception("Could not find episode media URI.");
            }

            if (string.IsNullOrEmpty(id)) { id = link; }

            return new Episode(_podcast, id, publishDate, title, mediaUri);
        }

        private bool TryParseMediaUriFromEnclosure(XmlReader reader, out Uri mediaUri)
        {
            var type = reader.GetAttribute("type");
            if (type == null || (!type.StartsWith("audio/") && !type.StartsWith("video/")))
            {
                mediaUri = null;
                return false;
            }

            var url = reader.GetAttribute("url");

            if (string.IsNullOrEmpty(url))
            {
                mediaUri = null;
                return false;
            }

            mediaUri = new Uri(url);
            return true;
        }

        private bool TryParseCrazyRssDate(string rssDate, out DateTimeOffset date)
        {
            if (DateTimeOffset.TryParse(rssDate, out date))
            {
                return true;
            }
            if (DateTimeOffset.TryParse(rssDate.Substring(0, rssDate.Length - 4), out date))
            {
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            _reader.Dispose();

        }
    }
}
