using System;
using System.Collections.Generic;

namespace EasyCLI
{
    using Command = Action<Context<Menu>>;

    public class Menu
    {
        private readonly OptionsParser _optionsParser;
        private readonly Dictionary<string, Command> _commands;
        private bool _isRunning;

        public Menu(OptionsParser optionsParser)
        {
            _optionsParser = optionsParser;
            _commands = new Dictionary<string, Command>();
        }

        private bool Contains(string name)
        {
            return _commands.ContainsKey(name);
        }

        public void Add(string name, Command command)
        {
            if (Contains(name))
                throw new ArgumentException(
                    $"The menu already contains a command with name '{name}'",
                    nameof(name));
            _commands[name] = command;
        }

        public void Execute(string command)
        {
            IEnumerable<string> tokens = command.Split();
            (Dictionary<string, string> kwargs,
             HashSet<string> switches,
             List<string> args) = _optionsParser.Parse(tokens);
            
            if (args.Count == 0)
                throw new ArgumentException(
                    "A command must contain at least 1 positional argument");

            string name = args[0];
            args.RemoveAt(0);
            if (!Contains(name))
                throw new ArgumentException($"Unknown command name: '{name}'");

            var context = new Context<Menu>(this, args, kwargs, switches);
            _commands[name].Invoke(context);
        }

        public void Terminate()
        {
            _isRunning = false;
        }

        public void Run()
        {
            _isRunning = true;
            while (_isRunning) Execute(Console.ReadLine());
        }
    }
}
