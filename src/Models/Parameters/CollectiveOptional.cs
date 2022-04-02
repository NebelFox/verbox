using System;
using Type = Verbox.Text.Type;

namespace Verbox.Models.Parameters
{
    internal class CollectiveOptional : Collective
    {
        public CollectiveOptional(string name, Type type) : base(name, type)
        { }

        protected override int MinValuesCount => 0;

        public override object Default => Array.Empty<object>();
    }
}
