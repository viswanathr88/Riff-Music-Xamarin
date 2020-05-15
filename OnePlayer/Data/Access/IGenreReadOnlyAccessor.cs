using System;
using System.Collections.Generic;
using System.Text;

namespace OnePlayer.Data.Access
{
    public interface IGenreReadOnlyAccessor
    {
        Genre Get(long id);

        Genre Find(string genreName);

        IList<Genre> GetAll();
    }
}
