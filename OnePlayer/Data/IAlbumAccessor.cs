using System.Collections.Generic;

namespace OnePlayer.Data
{
    public enum AlbumSortType
    {
        ReleaseYear,
        Title
    };

    public interface IAlbumAccessor
    {
        void EnsureCreated();

        int GetCount();

        Album Get(int id);

        IList<Album> GetAll();

        IList<Album> GetAll(AlbumSortType type, SortOrder order);

        IList<Album> GetAll(AlbumSortType type, SortOrder order, int startPosition, int count);

        IList<Album> GetAllByArtist(int artistId);

        Album FindByArtist(int artistId, string albumName);

        Album Add(Album album);

        Album Update(Album album);
    }
}
