using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyCLI.Parsers
{
        public class Splitter
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
            _quotes = new HashSet<char>(quotes.AsEnumerable());
        }
        
        public Splitter(char separator = ' ',
                        char quote = '"') : this(separator, quote.ToString()){}

        public IEnumerable<string> Split(string text)
        {
            var tokens = new LinkedList<string>();
            var token = new LinkedList<char>();
            State state = State.InSeparator;
            var quote = '\0';

            foreach (char c in text)
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (state)
                {
                case State.InQuotes:
                    if (c == quote)
                    {
                        tokens.AddLast(new string(token.ToArray()));
                        token.Clear();
                        state = State.InSeparator;
                    }
                    else
                    {
                        token.AddLast(c);
                    }
                    break;
                case State.InSeparator:
                    if (IsQuote(c))
                    {
                        quote = c;
                        state = State.InQuotes;
                    }
                    else if (c != _separator)
                    {
                        token.AddLast(c);
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
                        tokens.AddLast(new string(token.ToArray()));
                        token.Clear();
                        state = State.InSeparator;
                    }
                    else
                    {
                        token.AddLast(c);
                    }
                    break;
                }
            }

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (state)
            {
            case State.InQuotes:
                int position = text.Length - token.Count;
                throw new FormatException($"No closing quote for quote at {position}");
            case State.InWord:
                tokens.AddLast(new string(token.ToArray()));
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
