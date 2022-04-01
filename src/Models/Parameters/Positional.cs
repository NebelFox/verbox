using System;
using System.Collections.Generic;
using Verbox.Text.Tokens;
using Type = Verbox.Text.Type;

namespace Verbox.Models.Parameters
{
    internal record Positional(string Name, Type Type)
    {
        protected virtual int MinValuesCount => 1;
        protected virtual int MaxValuesCount => 1;

        public object Parse(IReadOnlyList<Token> tokens,
                            ref int current,
                            bool optionsEnabled)
        {
            var values = new List<object>();

            while (current < tokens.Count
                && IsValue(tokens[current], optionsEnabled)
                && values.Count < MaxValuesCount)
            {
                values.Add(ParseToken(tokens[current], current, optionsEnabled));
                ++current;
            }

            current += Convert.ToInt32(current < tokens.Count
                                    && tokens[current].Type == TokenType.ShortDelimiter);

            AssertEnoughValues(values.Count);

            return ComposeArgs(values);
        }

        private static bool IsValue(Token token, bool optionsEnabled)
        {
            return token.IsValue || token.IsOption && optionsEnabled == false;
        }

        private object ParseToken(Token token, int index, bool optionsEnabled)
        {
            string argument = token.GetValue(optionsEnabled);
            if (Type.TryParse(argument, out object value))
                return value;
            throw new ArgumentException(
                $"Type <{Type.Name}> of positional parameter <{Name}> mismatched the {index + 1}-th global argument ({argument})");
        }

        private void AssertEnoughValues(int actual)
        {
            int min = MinValuesCount;
            string postfix = min > 1 ? "s" : string.Empty;
            if (actual < min)
                throw new ArgumentException(
                    $"Parameter <{Name}> expected at least {min} value{postfix}, but actually got {actual}");
        }

        protected virtual object ComposeArgs(IReadOnlyList<object> values)
        {
            return values[0];
        }

        public virtual object Default => throw new ArgumentException($"Mandatory positional parameter {Name} missed arguments");
    }
}
