using System.Collections.Generic;

namespace Riff.Data
{
    public interface IIndexReadOnlyAccessor
    {
        IndexedTrack Get(long id);

        IList<IndexedTrackWithOffset> Search(string term);

        IList<AlbumQueryItem> FindMatchingAlbums(string term, int? maxCount);

        IList<ArtistQueryItem> FindMatchingArtists(string term, int? maxCount);

        IList<GenreQueryItem> FindMatchingGenres(string term, int? maxCount);

        IList<TrackQueryItem> FindMatchingTracks(string term, int? maxCount);

        IList<TrackQueryItem> FindMatchingTracksWithArtists(string term, int? maxCount);
    }
}
