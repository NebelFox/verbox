using System.Collections.Generic;
using System.Linq;
using Type = Verbox.Text.Type;

namespace Verbox.Models.Parameters
{
    internal record Optional(string Name, Type Type) : Positional(Name, Type)
    {
        protected override int MinValuesCount => 0;
        
        protected override object ComposeArgs(IReadOnlyList<object> values)
        {
            return values.Append(null).First();
        }

        public override object Default => null;
    }
}
