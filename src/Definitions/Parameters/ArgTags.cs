using System;

namespace Verbox.Definitions.Parameters
{
    [Flags]
    public enum ArgTags
    {
        None = 0,
        Optional = 1,
        Collective = 2
    }
}
