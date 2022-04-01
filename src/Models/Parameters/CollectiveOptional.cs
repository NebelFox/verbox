using System;
using Type = Verbox.Text.Type;

namespace Verbox.Models.Parameters
{
    internal record CollectiveOptional(string Name, Type Type) : Collective(Name, Type)
    {
        protected override int MinValuesCount => 0;

        public override object Default => Array.Empty<object>();
    }
}
