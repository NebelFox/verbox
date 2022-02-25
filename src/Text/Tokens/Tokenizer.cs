using System.Collections.Generic;
using System.Linq;

namespace Verbox.Text.Tokens
{
    internal class Tokenizer
    {
        private readonly string _longDelimiter;
        private readonly string _optionPrefix;
        private readonly IReadOnlySet<char> _quotes;
        private readonly string _shortDelimiter;

        public Tokenizer(string quotes,
                         string optionPrefix,
                         string shortDelimiter,
                         string longDelimiter)
        {
            _quotes = new HashSet<char>(quotes);
            _optionPrefix = optionPrefix;
            _shortDelimiter = shortDelimiter;
            _longDelimiter = longDelimiter;
        }

        public Token[] Tokenize(IEnumerable<string> values)
        {
            return values.Select(Tokenize).ToArray();
        }

        private Token Tokenize(string value)
        {
            if (value == _longDelimiter)
                return MakeLongDelimToken();
            if (value == _shortDelimiter)
                return MakeShortDelimToken();
            if (value.StartsWith(_optionPrefix))
                return MakeOptionToken(value);
            if (IsQuoted(value))
                return MakeQuotedToken(value);
            return new Token(value, TokenType.Word);
        }

        private Token MakeOptionToken(string s)
        {
            return new Token(TrimHead(s, _optionPrefix), s, TokenType.Option);
        }

        private static Token MakeQuotedToken(string s)
        {
            return new Token(s[1..^1], s, TokenType.Quoted);
        }

        private static string TrimHead(string s, string head)
        {
            return s[head.Length..];
        }

        private bool IsQuoted(string s)
        {
            return _quotes.Any(s.StartsWith) && s[0] == s[^1];
        }

        private static Token MakeShortDelimToken()
        {
            return new Token(TokenType.ShortDelimiter);
        }

        private static Token MakeLongDelimToken()
        {
            return new Token(TokenType.LongDelimiter);
        }
    }
}
