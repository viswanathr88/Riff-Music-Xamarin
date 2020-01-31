namespace OnePlayer.Data
{
    public interface IMusicDataStore
    {
        IMusicDataAccessor Create();

        IThumbnailCache TrackThumbnails { get; }

        IThumbnailCache AlbumThumbnails { get; }
    }
}
