using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Riff.Data.Test
{
    [Collection("PlaylistStorageTests")]
    public sealed class PlaylistTest : IDisposable
    {
        private readonly static string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playlists");
        private readonly PlaylistManager manager = new PlaylistManager(path);

        [Fact]
        public void Constructor_EmptyName_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new Playlist(path, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new Playlist(path, null));
        }

        [Fact]
        public void Constructor_ValidName_NotNull()
        {
            Assert.NotNull(new Playlist(path, "Test"));
        }

        [Fact]
        public void Constructor_Success_VerifyNameAndItems()
        {
            var playlist = new Playlist(path, "Test");
            Assert.Equal("Test", playlist.Name);
            Assert.Empty(playlist.Items);
        }

        [Fact]
        public async Task LoadAsync_NonExistentPlaylist_Throw()
        {
            var playlist = new Playlist(path, "Test");
            await Assert.ThrowsAnyAsync<Exception>(() => playlist.LoadAsync());
        }

        [Fact]
        public async Task LoadAsync_EmptyPlaylist_VerifyNoThrowAndCollectionSize()
        {
            string name = "TestPlaylist";
            var playlist = manager.CreatePlaylist(name);

            await playlist.LoadAsync();
            Assert.Empty(playlist.Items);
        }

        [Fact]
        public async Task LoadAsync_ValidJsonContents_VerifyCollectionSize()
        {
            List<DriveItem> items = new List<DriveItem>()
            {
                new DriveItem()
                {
                    Id = "TestId",
                    CTag = "TestCTag",
                    ETag = "TestETag",
                    AddedDate = DateTime.Now,
                    Description = "Test Drive Item",
                    Name = "Test Name",
                    Size = 4038578,
                    Source = DriveItemSource.OneDrive,
                    Track = new Track()
                    {
                        Id = 1,
                        Title = "Test Title",
                        Album = new Album()
                        {
                            Id = 1,
                            Name = "Test Album",
                            Artist = new Artist()
                            {
                                Id = 1,
                                Name = "Test Artist"
                            },
                            ReleaseYear = 2000,
                            Genre = new Genre()
                            {
                                Id = 1,
                                Name = "Test Genre"
                            }
                        },
                        Artist = "Test Track Artist",
                        Genre = new Genre()
                        {
                            Id = 1,
                            Name = "Test Genre"
                        }
                    }
                }
            };

            var playlist = manager.CreatePlaylist("Test");
            playlist.Items.Add(items.First());
            await playlist.SaveAsync();

            playlist.Items.Clear();
            await playlist.LoadAsync();
            Assert.Single(playlist.Items);
            Assert.Equal(items.First(), playlist.Items.First(), new DriveItemTableTest.DriveItemComparer());
        }

        public void Dispose()
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
