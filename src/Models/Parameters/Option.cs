using System.Collections.Generic;
using Verbox.Text;

namespace Verbox.Models.Parameters
{
    internal record Option(string Name,
                         Positional Parameter,
                         object Default)
    {
        public object Parse(IReadOnlyList<Token> tokens,
                            ref int current)
        {
            return Parameter.Parse(tokens, ref current) ?? Default;
        }
    }
}
