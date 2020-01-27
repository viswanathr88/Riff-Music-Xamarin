namespace OnePlayer.Data
{
    public sealed class SearchQuery
    {
        public SearchQuery()
        {
            // Defaults
            MaxArtistCount = 3;
            MaxGenreCount = 2;
            MaxAlbumCount = 5;
            MaxTrackCount = 15;
        }

        public string Term { get; set; }

        public int MaxArtistCount { get; set; }

        public int MaxGenreCount { get; set; }

        public int MaxAlbumCount { get; set; }

        public int MaxTrackCount { get; set; }
    }
}
