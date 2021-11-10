using System;
using System.Collections.Generic;
using Verbox.Text;
using Type = Verbox.Text.Type;

namespace Verbox.Models.Parameters
{
    public record Positional(string Name,
                             Type Type,
                             int MinValuesCount,
                             int MaxValuesCount)
    {
        public bool IsMandatory => MinValuesCount > 0;
        
        public object Parse(IReadOnlyList<Token> tokens,
                            ref int current)
        {
            var values = new List<object>();

            while (current < tokens.Count
                && tokens[current].IsValue
                && values.Count < MaxValuesCount)
            {
                string argument = tokens[current].Value; 
                if(Type.TryParse(argument, out object value))
                {
                    values.Add(value);
                    ++current;
                }
                else
                {
                    throw new ArgumentException($"Type <{Type.Name}> of positional parameter <{Name}> mismatched the {current+1}th argument ({argument})");
                }
            }

            if (values.Count < MinValuesCount)
                throw new ArgumentException($"Parameter \"{Name}\" expected at least {MinValuesCount} value, but actually got {values.Count}");

            if (MaxValuesCount > 1)
                return values.ToArray();
            return values.Count switch
            {
                0 => null,
                1 => values[0],
                _ => values.ToArray()
            };
        }
    }
}
