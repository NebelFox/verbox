using System;

namespace Verbox.Models.Executables
{
    internal abstract class Executable
    {
        protected const string HelpSwitch = "--help";
        private readonly string _help;

        protected Executable(string help)
        {
            _help = help;
        }

        public abstract void Execute(Box box, string[] tokens);

        public void Help()
        {
            Console.Write(_help);
        }
    }
}
