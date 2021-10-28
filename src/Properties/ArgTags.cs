using System;

namespace EasyCLI.Properties
{
    [Flags]
    public enum ArgTags
    {
        None = 0,
        Optional = 1,
        Collective = 2
    }
}
