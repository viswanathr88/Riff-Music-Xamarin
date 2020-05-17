using System;
using System.Collections.Generic;
using System.Text;

namespace OnePlayer.Data.Sqlite
{
    public enum Version
    {
        None = 0,
        Initial = 1,
        AddIndexes = 2
    };
}
