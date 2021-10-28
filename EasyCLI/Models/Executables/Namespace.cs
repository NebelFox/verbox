using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyCLI.Models.Executables
{
    internal sealed class Namespace : Executable
    {
        private readonly IReadOnlyDictionary<string, Executable> _members;

        public Namespace(string help,
                         IEnumerable<(string, Executable)> commands) : base(help)
        {
            _members = new Dictionary<string, Executable>(
                commands.Select(
                    pair => new KeyValuePair<string, Executable>(pair.Item1, pair.Item2)));
        }

        public override void Execute(Menu source, string[] tokens)
        {
            if (tokens.Contains(HelpSwitch) && tokens.Length == 1)
            {
                Help();
            }
            else
            {
                if (tokens.Length == 0)
                    throw new ArgumentException("No command name provided");
                
                Execute(source, tokens[0], tokens[1..]);
            }
        }

        private void Execute(Menu source, string name, string[] tokens)
        {
            if (!_members.TryGetValue(name, out Executable executable))
                throw new ArgumentException($"Unknown command: '{name}'");
            executable.Execute(source, tokens);
        }
    }
}
