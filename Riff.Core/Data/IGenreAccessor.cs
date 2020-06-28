using System.Collections.Generic;

namespace Riff.Data
{
    public interface IGenreAccessor : IGenreReadOnlyAccessor
    {
        Genre Add(Genre genre);

        Genre Update(Genre genre);
    }
}
