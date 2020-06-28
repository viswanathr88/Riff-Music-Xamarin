namespace Riff.Data.Sqlite
{
    interface ITable
    {
        string Name { get; }

        void HandleUpgrade(Version version);
    }
}
