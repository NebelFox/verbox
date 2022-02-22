using System;

namespace Verbox.Definitions.Parameters
{
    /// <summary>
    /// Flags for modifying a positional parameter parsing behavior
    /// Could be also applied to an option parameter
    /// </summary>
    [Flags]
    public enum ArgTags
    {
        /// <summary>
        /// Accepts exactly 1 value
        /// </summary>
        None = 0,

        /// <summary>
        /// Accepts 0 or 1 value
        /// </summary>
        Optional = 1,

        /// <summary>
        /// Accepts 1 and more values
        /// </summary>
        Collective = 2
    }
}
