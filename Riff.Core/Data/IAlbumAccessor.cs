namespace Riff.Data
{
    public interface IAlbumAccessor : IAlbumReadOnlyAccessor
    {
        Album Add(Album album);

        Album Update(Album album);
    }
}
