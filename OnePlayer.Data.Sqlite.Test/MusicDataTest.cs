using System;
using System.Linq;
using Xunit;

namespace OnePlayer.Data.Sqlite.Test
{
    public sealed class MusicDataTest
    {
        private readonly string dbPath = ":memory:";
        private readonly MusicMetadata musicData;

        public MusicDataTest()
        {
            musicData = new MusicMetadata(dbPath);
        }

        [Fact]
        public void Constructor_TestAccessorsNotNull()
        {
            Assert.NotNull(musicData.Albums);
            Assert.NotNull(musicData.Artists);
            Assert.NotNull(musicData.Genres);
            Assert.NotNull(musicData.DriveItems);
            Assert.NotNull(musicData.Tracks);
            Assert.NotNull(musicData.Thumbnails);
            Assert.NotNull(musicData.Index);
        }

        [Fact]
        public void Constructor_CheckVersion()
        {
            var expectedVersion = Enum.GetValues(typeof(Version)).Cast<Version>().Last();
            Assert.Equal(expectedVersion, musicData.GetVersion());
        }
    }
}
