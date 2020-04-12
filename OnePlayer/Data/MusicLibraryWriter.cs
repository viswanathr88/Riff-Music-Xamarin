﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OnePlayer.Data
{
    internal sealed class MusicLibraryWriter : IDisposable
    {
        private readonly IMusicDataStore store;
        private readonly IMusicDataAccessor accessor;
        private readonly HttpClient webClient;

        public MusicLibraryWriter(IMusicDataStore musicDataStore, HttpClient webClient)
        {
            store = musicDataStore ?? throw new ArgumentNullException(nameof(musicDataStore));
            this.accessor = musicDataStore.Access();
            this.webClient = webClient ?? throw new ArgumentNullException(nameof(webClient));
        }

        public event EventHandler<DriveItem> ItemAdded;
        public event EventHandler<DriveItem> ItemRemoved;
        public event EventHandler<DriveItem> ItemModified;

        public void Add(IEnumerable<Data.Json.DriveItem> items)
        {
            try
            {
                foreach (var item in items)
                {
                    if (item.audio != null)
                    {
                        ProcessAndSaveItem(item, accessor);
                    }
                }

                accessor.RemoveOrphans();
                accessor.Save();
            }
            catch (Exception)
            {
                accessor.Rollback();
                throw;
            }
        }

        public async Task DownloadThumbnailsAsync()
        {
            // Get all thumbnails that have not been cached
            var thumbnails = accessor.Thumbnails.GetUncached();

            // Download and save the thumbnails
            await thumbnails.ForEachAsync(DownloadAndCacheThumbnailAsync);
        }

        public void Dispose()
        {
            accessor?.Dispose();
        }

        private async Task DownloadAndCacheThumbnailAsync(ThumbnailInfo info)
        {
            try
            {
                // Download the small thumbnail
                if (!string.IsNullOrEmpty(info.SmallUrl))
                {
                    using (HttpResponseMessage message = await webClient.GetAsync(info.SmallUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (var stream = await message.Content.ReadAsStreamAsync())
                        {
                            await store.TrackThumbnails.SaveAsync(info.Id, stream, ThumbnailSize.Small);

                            using (var trackThumbnailStream = store.TrackThumbnails.Get(info.Id, ThumbnailSize.Small))
                            {
                                await store.AlbumThumbnails.SaveAsync(info.AlbumId, trackThumbnailStream, ThumbnailSize.Small);
                            }
                        }
                    }
                }

                // Download the medium thumbnail
                if (!string.IsNullOrEmpty(info.MediumUrl))
                {
                    using (HttpResponseMessage message = await webClient.GetAsync(info.MediumUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        using (var stream = await message.Content.ReadAsStreamAsync())
                        {
                            await store.TrackThumbnails.SaveAsync(info.Id, stream, ThumbnailSize.Medium);

                            using (var trackThumbnailStream = store.TrackThumbnails.Get(info.Id, ThumbnailSize.Medium))
                            {
                                await store.AlbumThumbnails.SaveAsync(info.AlbumId, trackThumbnailStream, ThumbnailSize.Medium);
                            }
                        }
                    }
                }

                info.Cached = true;
            }
            catch (Exception)
            {
                info.AttemptCount++;
                info.Cached = false;
            }
            finally
            {
                accessor.Thumbnails.Update(info);
                accessor.Save();
            }
        }

        private void ProcessAndSaveItem(Data.Json.DriveItem item, IMusicDataAccessor context)
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
            indexedTrack.SetAlbumId(album.Id);
            indexedTrack.ArtistName = artist.Name;
            indexedTrack.SetArtistId(artist.Id);
            indexedTrack.GenreName = genre.Name;
            indexedTrack.SetGenreId(genre.Id);
            indexedTrack.TrackArtist = track.Artist;
            indexedTrack.TrackName = track.Title;
            _ = isIndexedTrackNew ? context.Index.Add(indexedTrack) : context.Index.Update(indexedTrack);

            // Add or update thumbnail
            bool isThumbnailInfoNew = (thumbnailInfo == null);
            thumbnailInfo = thumbnailInfo ?? new ThumbnailInfo();
            thumbnailInfo.Id = track.Id;
            thumbnailInfo.DriveItemId = item.id;
            thumbnailInfo.AlbumId = album.Id;
            thumbnailInfo.AttemptCount = 0;
            thumbnailInfo.SmallUrl = item.Thumbnails?.FirstOrDefault()?.Small?.Url;
            thumbnailInfo.MediumUrl = item.Thumbnails?.FirstOrDefault()?.Medium?.Url;
            thumbnailInfo.LargeUrl = item.Thumbnails?.FirstOrDefault()?.Large?.Url;
            thumbnailInfo.Cached = string.IsNullOrEmpty(thumbnailInfo.SmallUrl) && string.IsNullOrEmpty(thumbnailInfo.MediumUrl) ? true : false;
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
