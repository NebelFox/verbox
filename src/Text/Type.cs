namespace Verbox.Text
{
    /// <summary>
    /// Parses a string to an <see cref="object"/>
    /// </summary>
    /// <returns>resulting object on success or null on failure</returns>
    public delegate object ParseFunction(string token);
    
    internal class Type
    {
        private readonly ParseFunction _parse;

        public Type(string name, ParseFunction parse)
        {
            _parse = parse;
            Name = name;
        }
        
        public string Name { get; }

        public object Parse(string token) => _parse(token);

        public bool TryParse(string token, out object value)
        {
            value = _parse(token);
            return value != null;
        }
    }
}
