using System;
using System.Collections.Generic;
using Verbox.Text.Tokens;

namespace Verbox.Models.Executables
{
    internal sealed class Namespace : Executable
    {
        private readonly IReadOnlyDictionary<string, Executable> _executables;

        public Namespace(string help,
                         IReadOnlyDictionary<string, Executable> executables) : base(help)
        {
            _executables = executables;
        }

        public override void Execute(Box box, Token[] tokens)
        {
            if (tokens[0].Type == TokenType.Word)
            {
                Execute(box, tokens[0].Value, tokens[1..]);
                return;
            }
            
            if (tokens.Length != 1 || ContainsHelpSwitch(tokens) == false)
                throw new InvalidOperationException("Namespace execution attempt");
                
            Help();
        }

        private void Execute(Box box, string name, Token[] tokens)
        {
            if (_executables.TryGetValue(name, out Executable executable) == false)
                throw new ArgumentException($"Unknown command: '{name}'");
            executable.Execute(box, tokens);
        }
    }
}
