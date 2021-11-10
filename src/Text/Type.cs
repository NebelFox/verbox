namespace Verbox.Text
{
    public class Type
    {
        public delegate object ParseFunction(string token);

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
