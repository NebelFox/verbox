using System;

namespace EasyCLI.Models.Executables
{
    internal sealed class Command : Executable
    {
        private readonly Action<Context> _action;
        
        public Command(string help, Action<Context> action) : base(help)
        {
            _action = action;
        }

        public override void Execute(Context context)
        {
            if (context[HelpSwitch])
            {
                if (context.OptionsCount > 1)
                {
                    Console.WriteLine(
                        $"Command aborted as there was {HelpSwitch} switch with other options");
                }
                else
                {
                    Help();
                }
            }
            else
            {
                _action.Invoke(context);
            }
        }
    }
}
