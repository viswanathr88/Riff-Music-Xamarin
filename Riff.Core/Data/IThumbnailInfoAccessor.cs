namespace Riff.Data
{
    public interface IThumbnailInfoAccessor : IThumbnailInfoReadOnlyAccessor
    {
        ThumbnailInfo Add(ThumbnailInfo info);

        ThumbnailInfo Update(ThumbnailInfo info);
    }
}
