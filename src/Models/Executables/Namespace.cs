using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Text.Tokens;

namespace Verbox.Models.Executables
{
    internal sealed class Namespace : Executable
    {
        private readonly IReadOnlyDictionary<string, Executable> _executables;

        public Namespace(string help,
                         IEnumerable<(string, Executable)> executables) : base(help)
        {
            _executables = new Dictionary<string, Executable>(
                executables.Select(
                    pair => new KeyValuePair<string, Executable>(pair.Item1, pair.Item2)));
        }

        public override void Execute(Box box, Token[] tokens)
        {
            if (tokens[0].Type != TokenType.Word)
            {
                if (tokens.Length != 1 || ContainsHelpSwitch(tokens) == false)
                    throw new InvalidOperationException("Namespace execution attempt");
                
                Help();
                return;
            }
            
            Execute(box, tokens[0].Value, tokens[1..]);
        }

        private void Execute(Box box, string name, Token[] tokens)
        {
            if (!_executables.TryGetValue(name, out Executable executable))
                throw new ArgumentException($"Unknown command: '{name}'");
            executable.Execute(box, tokens);
        }
    }
}
