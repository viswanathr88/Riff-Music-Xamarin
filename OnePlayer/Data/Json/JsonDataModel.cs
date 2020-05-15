using Newtonsoft.Json;
using System;

namespace OnePlayer.Data.Json
{
    internal class DeltaQueryResponse
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string context { get; set; }
        [JsonProperty(PropertyName = "@odata.deltaLink")]
        public string deltaLink { get; set; }

        [JsonProperty(PropertyName = "@odata.nextLink")]
        public string nextLink { get; set; }
        public DriveItem[] value { get; set; }
    }

    internal class DriveItem
    {
        [JsonProperty(PropertyName = "@odata.type")]
        public string odatatype { get; set; }
        public DateTime createdDateTime { get; set; }
        public string cTag { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public long size { get; set; }
        public string webUrl { get; set; }
        public Createdby createdBy { get; set; }
        public LastModifiedBy lastModifiedBy { get; set; }
        public ParentReference parentReference { get; set; }
        public FilesystemInfo fileSystemInfo { get; set; }
        public Folder folder { get; set; }
        public SpecialFolder specialFolder { get; set; }
        [JsonProperty(PropertyName = "@microsoft.graph.downloadUrl")]
        public string DownloadUrl { get; set; }
        public string description { get; set; }
        public Audio audio { get; set; }
        public File file { get; set; }
        public Deleted deleted { get; set; }
        public Thumbnails[] Thumbnails { get; set; }
    }

    internal class Createdby
    {
        public Device device { get; set; }
        public User user { get; set; }
        public Application application { get; set; }
        public OnedriveSync oneDriveSync { get; set; }
    }

    internal class Device
    {
        public string id { get; set; }
    }

    internal class User
    {
        public string displayName { get; set; }
        public string id { get; set; }
    }

    internal class Application
    {
        public string id { get; set; }
    }

    internal class OnedriveSync
    {
        [JsonProperty(PropertyName = "@odata.type")]
        public string odatatype { get; set; }
        public string id { get; set; }
    }

    internal class LastModifiedBy
    {
        public Application application { get; set; }
        public User user { get; set; }
        public Device device { get; set; }
        public OnedriveSync oneDriveSync { get; set; }
    }

    internal class ParentReference
    {
        public string driveId { get; set; }
        public string driveType { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
    }

    internal class FilesystemInfo
    {
        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
    }

    internal class Folder
    {
        public int childCount { get; set; }
        public View view { get; set; }
    }

    internal class View
    {
        public string viewType { get; set; }
        public string sortBy { get; set; }
        public string sortOrder { get; set; }
    }

    internal class SpecialFolder
    {
        public string name { get; set; }
    }

    internal class Audio
    {
        public string album { get; set; }
        public string albumArtist { get; set; }
        public string artist { get; set; }
        public int bitrate { get; set; }
        public string composers { get; set; }
        public int duration { get; set; }
        public string genre { get; set; }
        public bool hasDrm { get; set; }
        public bool isVariableBitrate { get; set; }
        public string title { get; set; }
        public int track { get; set; }
        public int year { get; set; }
        public string copyright { get; set; }
        public int disc { get; set; }
        public int discCount { get; set; }
    }

    internal class File
    {
        public string mimeType { get; set; }
        public Hashes hashes { get; set; }
    }

    internal class Hashes
    {
        public string quickXorHash { get; set; }
        public string sha1Hash { get; set; }
        public string crc32Hash { get; set; }
    }

    internal class Deleted
    {
        public string State { get; set; }
    }


    internal class ThumbnailResponse
    {
        [JsonProperty("@odata.context")]
        public string DataContext { get; set; }
        public Thumbnails[] Value { get; set; }
    }

    internal class Thumbnails
    {
        public string Id { get; set; }
        public Thumbnail Large { get; set; }
        public Thumbnail Medium { get; set; }
        public Thumbnail Small { get; set; }
    }

    internal class Thumbnail
    {
        public int Height { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }

}
