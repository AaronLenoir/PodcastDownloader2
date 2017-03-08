# Podcast Downloader

In the first place an excercise in using Akka.NET.

In the second place, a utility that downloads new podcasts from a given set of RSS feeds.

It keeps track of what was downloaded in a sqlite database.

It has soms bugs (sometimes stops working).

## Usage

Put a ```feeds.txt``` file next to the ```PodcastDownloader2.CommandLine.exe``` executable.

Format is:

```
<feedurl> <description>
```

Example:

```
http://feeds.nature.com/nature/podcast/current?format=xml Nature 
http://www.pwop.com/feed.aspx?show=dotnetrocks&filetype=master dotnetrocks
```

Then run ```PodcastDownloader2.CommandLine.exe```

```
PodcastDownloader2.CommandLine.exe <datadir>
```

The <datadir> is where the podcasts are downloaded to, and where the history sqlite database is put, so it must be writable. There will be a directory created for the day you are running the tool.

It will only download podcasts you haven't downloaded yet and that aren't older than some time.
