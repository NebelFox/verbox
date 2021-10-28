using System;
using System.Collections.Generic;
using System.Linq;

namespace Verbox.Models.Executables
{
    using Arguments = IReadOnlyDictionary<string, object>;
    internal sealed class Command : Executable
    {
        private readonly Action<Context> _action;
        private readonly Signature _signature;

        public Command(string help, 
                       Action<Context> action,
                       Signature signature) : base(help)
        {
            _action = action;
            _signature = signature;
        }

        public override void Execute(Menu source, string[] tokens)
        {
            if (tokens.Contains(HelpSwitch))
            {
                Help();
            }
            else
            {
                Arguments arguments = _signature.ParseArguments(tokens);
                var context = new Context(source, arguments);
                _action.Invoke(context);
            }
        }
    }
}
