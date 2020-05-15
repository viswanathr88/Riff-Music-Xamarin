using OnePlayer.Data.Access;
using System.Collections.Generic;

namespace OnePlayer.Data.Access
{
    public interface IGenreAccessor : IGenreReadOnlyAccessor
    {
        Genre Add(Genre genre);

        Genre Update(Genre genre);
    }
}
