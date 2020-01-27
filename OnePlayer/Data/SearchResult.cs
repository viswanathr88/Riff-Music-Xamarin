using System.Collections.Generic;

namespace OnePlayer.Data
{
    public sealed class SearchResult
    {
        private readonly List<SearchItem> artists = new List<SearchItem>();
        private readonly List<SearchItem> albums = new List<SearchItem>();
        private readonly List<SearchItem> genres = new List<SearchItem>();
        private readonly List<SearchItem> tracks = new List<SearchItem>();

        public IList<SearchItem> Artists => artists;

        public IList<SearchItem> Albums => albums;

        public IList<SearchItem> Genres => genres;

        public IList<SearchItem> Tracks => tracks;
    }
}
