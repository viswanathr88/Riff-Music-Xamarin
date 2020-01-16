using System;
using System.Collections.Generic;

namespace OnePlayer.Data
{
    internal sealed class MusicLibraryWriter
    {
        private readonly IMusicDataContextFactory dataContextFactory;

        public MusicLibraryWriter(IMusicDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public void Add(Json.DriveItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            ProcessAndSaveItem(item);
        }

        public void AddRange(IEnumerable<Json.DriveItem> items)
        {
            using (var context = dataContextFactory.Create())
            {
                try
                {
                    foreach (var item in items)
                    {
                        if (item.audio != null)
                        {
                            ProcessAndSaveItem(item, context);
                        }
                    }
                }
                catch (Exception)
                {
                    context.Rollback();
                }
            }
        }

        public void AddRange(Json.DriveItem[] items)
        {
            using (var context = dataContextFactory.Create())
            {
                try
                {
                    foreach (var item in items)
                    {
                        if (item.audio != null)
                        {
                            ProcessAndSaveItem(item, context);
                        }
                    }
                }
                catch (Exception)
                {
                    context.Rollback();
                }
            }
        }

        private void ProcessAndSaveItem(Json.DriveItem item)
        {
            using (var context = dataContextFactory.Create())
            {
                ProcessAndSaveItem(item, context);
            }
        }

        private void ProcessAndSaveItem(Json.DriveItem item, IMusicDataContext context)
        {
            DriveItem driveItem = context.DriveItems.Get(item.id);
            Track track = (driveItem != null) ? context.Tracks.Get(driveItem.TrackId) : null;
            Artist artist = context.Artists.Find(item.audio.albumArtist);
            Album album = artist != null ? context.Albums.FindByArtist(artist.Id.Value, item.audio.album) : null;
            Genre genre = (album != null) ? new Genre() { Id = album.GenreId } : context.Genres.Find(item.audio.genre);

            artist = artist ?? new Artist()
            {
                Name = item.audio.albumArtist,
                NameLower = item.audio.albumArtist.ToLower()
            };
            genre = genre ?? new Genre() 
            {
                Name = item.audio.genre,
                NameLower = item.audio.genre.ToLower()
            };
            album = album ?? new Album()
            {
                Name = item.audio.album,
                NameLower = item.audio.album.ToLower(),
                ReleaseYear = item.audio.year
            };
            track = track ?? new Track()
            {
                Title = item.audio.title,
                TitleLower = item.audio.title.ToLower(),
                Number = item.audio.track,
                Artist = item.audio.artist,
                Bitrate = item.audio.bitrate,
                Composers = item.audio.composers,
                Duration = item.audio.duration,
                ReleaseYear = item.audio.year
            };
            driveItem = driveItem ?? new DriveItem()
            {
                CTag = item.cTag,
                ETag = item.eTag,
                AddedDate = item.createdDateTime,
                LastModified = item.lastModifiedDateTime,
                DownloadUrl = item.DownloadUrl,
                Size = item.size
            };

            if (!genre.Id.HasValue)
            {
                context.Genres.Add(genre);
            }

            if (!artist.Id.HasValue)
            {
                context.Artists.Add(artist);
            }

            if (!album.Id.HasValue)
            {
                album.GenreId = genre.Id.Value;
                album.ArtistId = artist.Id.Value;
                context.Albums.Add(album);
            }

            if (!track.Id.HasValue)
            {
                track.AlbumId = album.Id.Value;
                context.Tracks.Add(track);
            }

            if (string.IsNullOrEmpty(driveItem.Id))
            {
                driveItem.Id = item.id;
                driveItem.TrackId = track.Id.Value;
                context.DriveItems.Add(driveItem);
            }
        } 
    }
}
