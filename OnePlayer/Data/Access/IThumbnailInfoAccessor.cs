namespace OnePlayer.Data.Access
{
    public interface IThumbnailInfoAccessor : IThumbnailInfoReadOnlyAccessor
    {
        ThumbnailInfo Add(ThumbnailInfo info);

        ThumbnailInfo Update(ThumbnailInfo info);
    }
}
