namespace OnePlayer.Data
{
    public interface IMusicDataStore
    {
        IMusicDataAccessor Access();

        IThumbnailCache TrackThumbnails { get; }

        IThumbnailCache AlbumThumbnails { get; }
    }
}
