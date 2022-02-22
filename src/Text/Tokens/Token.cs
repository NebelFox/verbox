namespace Verbox.Text.Tokens
{
    internal record Token(string Value, string OriginalValue, TokenType Type)
    {
        public Token(TokenType type) : this(string.Empty, type)
        { }

        public Token(string value, TokenType type) : this(value, value, type)
        { }

        public bool IsValue => Type is TokenType.Word or TokenType.Quoted;

        public bool IsOption => Type == TokenType.Option;

        public string GetValue(bool optionsEnabled = true)
        {
            return IsOption && optionsEnabled == false
                ? OriginalValue
                : Value;
        }
    }
}
