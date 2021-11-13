namespace Verbox.Text.Tokens
{
    internal record Token(string Value, TokenType Type)
    {
        public Token(TokenType type) : this(string.Empty, type)
        { }

        public bool IsValue => Type is TokenType.Word or TokenType.Quoted;

        public bool IsOption => Type == TokenType.Option;
    }
}
