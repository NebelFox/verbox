using System.Collections.Generic;
using System.Linq;
using Type = Verbox.Text.Type;

namespace Verbox.Models.Parameters
{
    internal class Collective : Positional
    {
        public Collective(string name, Type type) : base(name, type)
        { }

        protected override int MaxValuesCount => int.MaxValue;

        protected override object ComposeArgs(IReadOnlyList<object> values)
        {
            return values.ToArray();
        }
    }
}
