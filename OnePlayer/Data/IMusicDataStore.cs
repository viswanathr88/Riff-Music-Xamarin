namespace OnePlayer.Data
{
    public interface IMusicDataStore
    {
        IMusicDataAccessor Create();

        IThumbnailCache Thumbnails { get; }
    }
}
