using Moq;
using Riff.Data;
using Riff.Sync;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Xunit;

namespace Riff.UWP.Test.ViewModel
{
    public sealed class MediaListTest
    {
        private readonly Mock<ITrackUrlDownloader> mockTrackUrlDownloader;
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly MusicLibrary library;
        private readonly MediaList mediaList;

        public MediaListTest()
        {
            // Setup mocks
            mockTrackUrlDownloader = new Mock<ITrackUrlDownloader>();
            mockMetadata = new Mock<IMusicMetadata>();
            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, mockMetadata.Object);
            mediaList = new MediaList(library, mockTrackUrlDownloader.Object);
        }

        [Fact]
        public async Task SetItems_Null_Throw()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => mediaList.AddItems(null, 0));
        }

        [Fact]
        public async Task SetItems_StartIndexOutOfBounds_Throw()
        {
            List<DriveItem> items = new List<DriveItem>();
            items.Add(new DriveItem() { Id = "TestId" });
            items.Add(new DriveItem() { Id = "TestId2" });
            items.Add(new DriveItem() { Id = "TestId3" });
            items.Add(new DriveItem() { Id = "TestId4" });

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => mediaList.AddItems(items, 4));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => mediaList.AddItems(items, 20));
        }

        [Fact(Skip = "Not deterministic")]
        public async Task SetItems_EmptyTags_ValidateUnknownFields()
        {
            List<DriveItem> items = new List<DriveItem>();
            items.Add(new DriveItem() { Id = "TestId", Track = new Track() { Id = 1, Album = new Album() { Id = 1, Artist = new Artist() { Id = 10 } }}});
            await mediaList.AddItems(items, 0);
            Assert.Single(mediaList);
            Assert.NotNull(mediaList[0]);
            var item = mediaList[0];
            Assert.Equal("Unknown Title", item.Title);
            Assert.Equal("Unknown Album", item.Album);
            Assert.Equal("Unknown Artist", item.Artist);
        }

        [Fact(Skip = "Not deterministic")]
        public async Task SetItems_MiddleIndex_ValidateFirstItemIsIndex()
        {
            List<DriveItem> items = new List<DriveItem>();
            items.Add(new DriveItem() { Id = "TestId", Track = new Track() { Id = 1, Title = "Title1", Album = new Album() { Id = 1, Artist = new Artist() { Id = 10 } } } });
            items.Add(new DriveItem() { Id = "TestId2", Track = new Track() { Id = 2, Title = "Title2", Album = new Album() { Id = 1, Artist = new Artist() { Id = 10 } } } });
            items.Add(new DriveItem() { Id = "TestId3", Track = new Track() { Id = 3, Title = "Title3", Album = new Album() { Id = 1, Artist = new Artist() { Id = 10 } } } });
            items.Add(new DriveItem() { Id = "TestId4", Track = new Track() { Id = 4, Title = "Title4", Album = new Album() { Id = 1, Artist = new Artist() { Id = 10 } } } });

            await mediaList.AddItems(items, 2);
            Assert.Equal(items[2].Track.Title, mediaList[0].Title);
        }
    }
}
