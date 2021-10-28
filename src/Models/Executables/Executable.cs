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

        public abstract void Execute(Menu source, string[] tokens);

        public void Help()
        {
            Console.WriteLine(_help);
        }
    }
}
