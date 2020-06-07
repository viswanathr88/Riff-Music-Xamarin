namespace Riff.Data
{
    interface IEditSessionHandler
    {
        void HandleSessionSaved();

        void HandleSessionDisposed();
    }
}
