namespace Riff.Data
{
    public interface ITrackContainer
    {
        string DownloadIdentifier { get; }

        Track Track { get; }
    }
}
