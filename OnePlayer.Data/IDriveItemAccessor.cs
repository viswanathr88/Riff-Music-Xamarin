namespace OnePlayer.Data
{
    public interface IDriveItemAccessor
    {
        DriveItem Get(string id);

        DriveItem Add(DriveItem item);

        DriveItem Update(DriveItem item);

        void Delete(string id);
    }
}
