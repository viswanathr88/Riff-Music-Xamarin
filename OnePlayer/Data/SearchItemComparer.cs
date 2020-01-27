using System.Collections.Generic;

namespace OnePlayer.Data
{
    sealed class SearchItemComparer : IComparer<SearchItem>
    {
        public int Compare(SearchItem x, SearchItem y)
        {
            int retVal = x.Rank - y.Rank;
            if (retVal == 0)
            {
                retVal = x.Type - y.Type;
                if (retVal == 0)
                {
                    retVal = string.Compare(x.Name, y.Name);
                }
            }
            return retVal;
        }
    }
}
