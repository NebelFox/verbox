using System;

namespace EasyCLI.Models.Executables
{
    internal abstract class Executable
    {
        protected const string HelpSwitch = "help";
        private readonly string _help;
        
        protected Executable(string help)
        {
            _help = help;
        }

        public abstract void Execute(Context context);

        public void Help()
        {
            Console.WriteLine(_help);
        }
    }
}
