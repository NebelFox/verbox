using System;
using System.Linq;

namespace EasyCLI.Parsers
{
    public class SwitchParser
    {
        private readonly string _shortPrefix;
        private readonly string _longPrefix;
        private readonly int _minLongKeyLength;
        private int MinLongTokenLength => _longPrefix.Length + _minLongKeyLength;
        
        public SwitchParser(string shortPrefix, string longPrefix, int minLongKeyLength=2)
        {
            _shortPrefix = shortPrefix;
            _longPrefix = longPrefix;
            _minLongKeyLength = minLongKeyLength;
        }

        internal bool TryParseToOptions(string token, Options options)
        {
            if (!TryParse(token, out string[] keys))
                return false;
            options.AddSwitches(keys);
            return true;
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
            if (!token.StartsWith(_longPrefix)) 
                return false;
            keys = new []{TrimLongKeyPrefix(token)};
            return true;
        }

        private bool TryGetShortKeys(string token, ref string[] keys)
        {
            if (!token.StartsWith(_shortPrefix)) 
                return false;
            keys = StringToArray(TrimShortKeyPrefix(token));
            return true;
        }

        private string TrimShortKeyPrefix(string token)
        {
            return token[_shortPrefix.Length..];
        }

        private string TrimLongKeyPrefix(string token)
        {
            return token[_longPrefix.Length..];
        }

        private static string[] StringToArray(string s)
        {
            return s.Select(c => c.ToString()).ToArray();
        }
    }
}
