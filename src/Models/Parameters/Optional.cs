using System.Collections.Generic;
using System.Linq;
using Type = Verbox.Text.Type;

namespace Verbox.Models.Parameters
{
    internal class Optional : Positional
    {
        public Optional(string name, Type type) : base(name, type)
        { }

        protected override int MinValuesCount => 0;

        protected override object ComposeArgs(IReadOnlyList<object> values)
        {
            return values.Append(Default).First();
        }

        public override object Default => null;
    }
}
