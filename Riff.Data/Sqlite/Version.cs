namespace Riff.Data.Sqlite
{
    internal enum Version
    {
        None = 0,
        Initial = 1,
        AddIndexes = 2,
        AddPlaylists = 3
    };
}
