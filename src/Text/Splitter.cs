using System;
using System.Collections.Generic;
using System.Text;

namespace Verbox.Text
{
    internal class Splitter
    {
        private enum State
        {
            InQuotes,
            InSeparator,
            InWord
        }

        private readonly char _separator;
        private readonly IReadOnlySet<char> _quotes;

        public Splitter(char separator = ' ',
                        string quotes = "\"'")
        {
            _separator = separator;
            _quotes = new HashSet<char>(quotes);
        }

        public Splitter(char separator = ' ',
                        char quote = '"') : this(separator, quote.ToString())
        { }

        public IEnumerable<string> Split(string text)
        {
            var tokens = new List<string>();
            var token = new StringBuilder();
            State state = State.InSeparator;
            var quote = '\0';

            foreach (char c in text)
            {
                switch (state)
                {
                case State.InQuotes:
                    token.Append(c);
                    if (c == quote)
                    {
                        tokens.Add(token.ToString());
                        token.Clear();
                        state = State.InSeparator;
                    }
                    break;
                case State.InSeparator:
                    if (IsQuote(c))
                    {
                        quote = c;
                        token.Append(c);
                        state = State.InQuotes;
                    }
                    else if (c != _separator)
                    {
                        token.Append(c);
                        state = State.InWord;
                    }
                    break;
                case State.InWord:
                    if (IsQuote(c))
                    {
                        throw new FormatException(
                            $"Quote char ({quote}) inside an unquoted word");
                    }
                    else if (c == _separator)
                    {
                        tokens.Add(token.ToString());
                        token.Clear();
                        state = State.InSeparator;
                    }
                    else
                    {
                        token.Append(c);
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Invalid state: {(int)state}");
                }
            }

            switch (state)
            {
            case State.InQuotes:
                int position = text.Length - token.Length;
                throw new FormatException($"No closing quote for quote at {position}");
            case State.InWord:
                tokens.Add(token.ToString());
                break;
            }

            return tokens;
        }

        private bool IsQuote(char c)
        {
            return _quotes.Contains(c);
        }
    }
}
