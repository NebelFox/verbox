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

        public override void Execute(Context context)
        {
            if (context[HelpSwitch] && context.ArgsCount == 0)
            {
                if (context.OptionsCount > 1)
                    throw new ArgumentException($"{HelpSwitch} switch was used with other options");

                Help();
            }
            else
            {
                if (context.ArgsCount == 0)
                    throw new ArgumentException("No command name provided");

                (string name, Context updatedContext) = context.ExtractFirstArg();
                Execute(name, updatedContext);
            }
        }

        private void Execute(string name, Context context)
        {
            if (!_members.TryGetValue(name, out Executable executable))
                throw new ArgumentException($"Unknown command: '{name}'");
            executable.Execute(context);
        }
    }
}
