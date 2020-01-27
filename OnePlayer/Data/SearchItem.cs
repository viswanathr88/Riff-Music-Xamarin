namespace OnePlayer.Data
{
    public enum SearchItemType
    {
        Artist,
        Album,
        Genre,
        Track,
        TrackArtist
    };

    public class SearchItem
    {
        public SearchItemType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Rank { get; set; }
    }
}
