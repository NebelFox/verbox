using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Text.Tokens;

namespace Verbox.Models.Executables
{
    internal abstract class Executable
    {
        private const string HelpSwitch = "help";
        private readonly string _help;

        protected Executable(string help)
        {
            _help = help;
        }

        public abstract void Execute(Box box, Token[] tokens);

        public void Help()
        {
            Console.WriteLine(_help);
        }

        protected static bool ContainsHelpSwitch(IEnumerable<Token> tokens)
        {
            return tokens.Any(t => t.IsOption && t.Value == HelpSwitch);
        }
    }
}
