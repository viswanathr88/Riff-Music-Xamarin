using System.Collections.Generic;

namespace OnePlayer.Data
{
    public interface IAlbumAccessor
    {
        Album Get(int id);

        IList<Album> GetAll();

        IList<Album> GetAllByArtist(int artistId);

        Album FindByArtist(int artistId, string albumName);

        Album Add(Album album);

        Album Update(Album album);
    }
}
