using Riff.Data.Access;
using System.Collections.Generic;

namespace Riff.Data.Access
{
    public interface IGenreAccessor : IGenreReadOnlyAccessor
    {
        Genre Add(Genre genre);

        Genre Update(Genre genre);
    }
}
