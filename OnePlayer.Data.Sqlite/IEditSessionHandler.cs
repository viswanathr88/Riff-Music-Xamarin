namespace OnePlayer.Data
{
    interface IEditSessionHandler
    {
        void HandleSessionSaved();

        void HandleSessionDisposed();
    }
}
