using Akka.Actor;
using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PodcastDownloader2.Engine.Actors.GeneralPurpose;
using PodcastDownloader2.Engine.Actors.GeneralPurpose.Messages;

namespace PodcastDownloader2.Tests
{
    [TestClass]
    public class BusTests : TestKit
    {
        [TestMethod]
        public void BusSubscriberShouldReceiveMessage()
        {
            var bus = Sys.ActorOf(Props.Create(() => new BusActor()));
            bus.Tell(new Subscribe(TestActor));
            bus.Tell("Test Message");
            ExpectMsg("Test Message");
        }
    }
}
