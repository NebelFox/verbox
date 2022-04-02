using System;
using System.Collections.Generic;
using System.Linq;
using Type = Verbox.Text.Type;

namespace Verbox.Models.Parameters
{
    internal record Collective(string Name, Type Type) : Positional(Name, Type)
    {
        protected override int MaxValuesCount => int.MaxValue;
        
        protected override object ComposeArgs(IReadOnlyList<object> values)
        {
            return values.ToArray();
        }
    }
}
