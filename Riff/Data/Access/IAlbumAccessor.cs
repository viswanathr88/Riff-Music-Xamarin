namespace Riff.Data.Access
{
    public interface IAlbumAccessor : IAlbumReadOnlyAccessor
    {
        Album Add(Album album);

        Album Update(Album album);
    }
}
