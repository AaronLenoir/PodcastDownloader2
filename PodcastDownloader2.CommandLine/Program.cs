using PodcastDownloader2.Engine;
using System;
using PodcastDownloader2.Engine.Model;
using System.Collections.Generic;
using System.IO;

// TODO:
//  * [Done] Specialized RSS parser
//  * [Done] Gracious error handling

namespace PodcastDownloader2.CommandLine
{
    public class Program
    {
        static void Main(string[] args)
        {
            var targetDir = @"C:\temp\podcast";
            if (args.Length > 0)
            {
                targetDir = args[0];
            }

            var downloader = new PodcastDownloader(targetDir, new StatusTracker());

            var podcasts = GetPodcastsFromFile("feeds.txt");

            downloader.GetPodcasts(podcasts);

            Console.ReadLine();
        }

        static List<Podcast> GetPodcastsFromFile(string path)
        {
            var result = new List<Podcast>();

            var lines = File.ReadAllLines(path);

            foreach(var line in lines)
            {
                var firstSpace = line.IndexOf(' ');
                if (line.StartsWith("#") || firstSpace == -1) { continue; }

                var url = line.Substring(0, firstSpace);
                var name = line.Substring(firstSpace + 1, line.Length - firstSpace - 1);

                result.Add(new Podcast(url, name));
            }

            return result;
        }
    }
}
