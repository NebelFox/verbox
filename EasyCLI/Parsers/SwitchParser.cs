using System;
using System.Linq;

namespace EasyCLI.Parsers
{
    public class SwitchParser
    {
        private readonly string _shortKeyPrefix;
        private readonly string _longKeyPrefix;
        private readonly int _minLongKeyLength;
        private int MinLongTokenLength => _longKeyPrefix.Length + _minLongKeyLength;
        
        public SwitchParser(string shortKeyPrefix, string longKeyPrefix, int minLongKeyLength=2)
        {
            _shortKeyPrefix = shortKeyPrefix;
            _longKeyPrefix = longKeyPrefix;
            _minLongKeyLength = minLongKeyLength;
        }
        
        public bool TryParse(string token, out string[] keys)
        {
            keys = Array.Empty<string>();
            if (token.Length >= MinLongTokenLength)
                return TryGetLongKeys(token, ref keys) || TryGetShortKeys(token, ref keys);
            return false;
        }

        private bool TryGetLongKeys(string token, ref string[] keys)
        {
            if (!token.StartsWith(_longKeyPrefix)) 
                return false;
            keys = new []{TrimLongKeyPrefix(token)};
            return true;
        }

        private bool TryGetShortKeys(string token, ref string[] keys)
        {
            if (!token.StartsWith(_shortKeyPrefix)) 
                return false;
            keys = StringToArray(TrimShortKeyPrefix(token));
            return true;
        }

        private string TrimShortKeyPrefix(string token)
        {
            return token[_shortKeyPrefix.Length..];
        }

        private string TrimLongKeyPrefix(string token)
        {
            return token[_longKeyPrefix.Length..];
        }

        private static string[] StringToArray(string s)
        {
            return s.Select(c => c.ToString()).ToArray();
        }
    }
}
