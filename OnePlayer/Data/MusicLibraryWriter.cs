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

        public event EventHandler<DriveItem> ItemAdded;
        public event EventHandler<DriveItem> ItemRemoved;
        public event EventHandler<DriveItem> ItemModified;

        public void AddRange(IEnumerable<Json.DriveItem> items)
        {
            using (var context = dataContextFactory.Create())
            {
                try
                {
                    bool isEmpty = true;
                    foreach (var item in items)
                    {
                        isEmpty = false;
                        if (item.audio != null)
                        {
                            ProcessAndSaveItem(item, context);
                        }
                    }

                    if (!isEmpty)
                    {
                        context.RemoveOrphans();
                    }
                }
                catch (Exception ex)
                {
                    context.Rollback();
                    throw;
                }
            }
        }

        private void ProcessAndSaveItem(Json.DriveItem item, IMusicDataContext context)
        {
            DriveItem driveItem = context.DriveItems.Get(item.id);

            if (item.deleted != null)
            {
                context.DriveItems.Delete(item.id);
                ItemRemoved?.Invoke(this, driveItem);
                return;
            }

            Track track = (driveItem != null) ? context.Tracks.Get(driveItem.TrackId) : null;
            IndexedTrack indexedTrack = (track != null) ? context.Index.Get(track.Id) : null;
            ThumbnailInfo thumbnailInfo = (track != null) ? context.Thumbnails.Get(track.Id) : null;
            Artist artist = context.Artists.Find(item.audio.albumArtist);
            Album album = artist != null ? context.Albums.FindByArtist(artist.Id, item.audio.album) : null;
            Genre genre = context.Genres.Find(item.audio.genre);

            // If we already have a genre, do nothing. If not create a new one
            bool isGenreNew = (genre == null);
            genre = genre ?? new Genre() { Name = item.audio.genre, NameLower = item.audio.genre.ToLower() };
            if (isGenreNew)
            {
                context.Genres.Add(genre);
            }

            // If we already have an artist, do nothing. If not create a new one
            bool isArtistNew = (artist == null);
            artist = artist ?? new Artist() { Name = item.audio.albumArtist, NameLower = item.audio.albumArtist.ToLower() };
            if (isArtistNew)
            {
                context.Artists.Add(artist);
            }

            // If we already have an album, do nothing. If not create a new one
            bool isAlbumNew = (album == null);
            album = album ?? new Album() { 
                Name = item.audio.album,
                NameLower = item.audio.album.ToLower(),
                ArtistId = artist.Id
            };
            bool albumNeedsUpdate = (album.ReleaseYear != item.audio.year) || (album.GenreId != genre.Id);
            album.ReleaseYear = item.audio.year;
            album.GenreId = genre.Id;
            _ = isAlbumNew ? context.Albums.Add(album) : albumNeedsUpdate ? context.Albums.Update(album) : null;

            // If we have a track, update its properties. If not create a new one
            bool isTrackNew = (track == null);
            track = track ?? new Track();
            track.Title = item.audio.title;
            track.TitleLower = item.audio.title.ToLower();
            track.Number = item.audio.track;
            track.Artist = item.audio.artist;
            track.Bitrate = item.audio.bitrate;
            track.Composers = item.audio.composers;
            track.Duration = item.audio.duration;
            track.ReleaseYear = item.audio.year;
            track.GenreId = genre.Id;
            track.AlbumId = album.Id;
            _ = isTrackNew ? context.Tracks.Add(track) : context.Tracks.Update(track);

            // Replicate whatever changes had to be done to the Track to IndexedTrack
            bool isIndexedTrackNew = (indexedTrack == null);
            indexedTrack = indexedTrack ?? new IndexedTrack();
            indexedTrack.Id = track.Id;
            indexedTrack.AlbumName = album.Name;
            indexedTrack.ArtistName = artist.Name;
            indexedTrack.GenreName = genre.Name;
            indexedTrack.TrackArtist = track.Artist;
            indexedTrack.TrackName = track.Title;
            _ = isIndexedTrackNew ? context.Index.Add(indexedTrack) : context.Index.Update(indexedTrack);

            // Add or update thumbnail
            bool isThumbnailInfoNew = (thumbnailInfo == null);
            thumbnailInfo = thumbnailInfo ?? new ThumbnailInfo();
            thumbnailInfo.Id = track.Id;
            thumbnailInfo.DriveItemId = item.id;
            thumbnailInfo.AttemptCount = 0;
            thumbnailInfo.Cached = false;
            thumbnailInfo.SmallUrl = item.Thumbnails?.Small?.Url;
            thumbnailInfo.MediumUrl = item.Thumbnails?.Medium?.Url;
            thumbnailInfo.LargeUrl = item.Thumbnails?.Large?.Url;
            _ = isThumbnailInfoNew ? context.Thumbnails.Add(thumbnailInfo) : context.Thumbnails.Update(thumbnailInfo);

            // If we have a driveItem already, update its properties. If not create a new one
            bool isDriveItemNew = (driveItem == null);
            driveItem = driveItem ?? new DriveItem();
            driveItem.Id = item.id;
            driveItem.CTag = item.cTag;
            driveItem.ETag = item.eTag;
            driveItem.AddedDate = item.createdDateTime;
            driveItem.LastModified = item.lastModifiedDateTime;
            driveItem.DownloadUrl = item.DownloadUrl;
            driveItem.Size = (int)item.size;
            driveItem.TrackId = track.Id;
            driveItem.Source = DriveItemSource.OneDrive;
            if (isDriveItemNew)
            {
                context.DriveItems.Add(driveItem);
                ItemAdded?.Invoke(this, driveItem);
            }
            else
            {
                context.DriveItems.Update(driveItem);
                ItemModified?.Invoke(this, driveItem);
            }
        } 
    }
}
