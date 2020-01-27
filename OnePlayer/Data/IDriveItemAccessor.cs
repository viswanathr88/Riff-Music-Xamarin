namespace OnePlayer.Data
{
    public interface IDriveItemAccessor
    {
        void EnsureCreated();

        DriveItem Get(string id);

        DriveItem Add(DriveItem item);

        DriveItem Update(DriveItem item);
        void Delete(string id);
    }
}
