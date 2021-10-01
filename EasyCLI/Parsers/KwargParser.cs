namespace EasyCLI.Parsers
{
    public class KwargParser
    {
        private const int ExpectedSplitCount = 2;
        private readonly int _minKeyLength;
        private readonly int _minValueLength;
        private readonly string _prefix;
        private readonly string _suffix;

        private int MinTokenLength =>
            _minKeyLength + _minValueLength + _prefix.Length + _suffix.Length;

        public KwargParser(string prefix,
                           string suffix,
                           int minKeyLength = 2,
                           int minValueLength = 1)
        {
            _prefix = prefix;
            _suffix = suffix;
            _minKeyLength = minKeyLength;
            _minValueLength = minValueLength;
        }

        public bool TryParse(string token, out string key, out string value)
        {
            if (IsTokenLengthValid(token) && IsPrefixed(token))
            {
                string[] pair = Split(Trim(token));
                if (pair.Length == ExpectedSplitCount 
                    && IsKeyLengthValid(pair[0])
                    && IsValueLengthValid(pair[1]))
                {
                    key = pair[0];
                    value = pair[1];
                    return true;
                }
            }

            key = string.Empty;
            value = string.Empty;
            return false;
        }

        private bool IsTokenLengthValid(string token)
        {
            return token.Length >= MinTokenLength;
        }

        private bool IsPrefixed(string token)
        {
            return token.StartsWith(_prefix);
        }

        private string[] Split(string token)
        {
            return token.Split(_suffix);
        }

        private string Trim(string token)
        {
            return token[_prefix.Length..];
        }

        private bool IsKeyLengthValid(string key)
        {
            return key.Length >= _minKeyLength;
        }

        private bool IsValueLengthValid(string value)
        {
            return value.Length >= _minValueLength;
        }
    }
}
