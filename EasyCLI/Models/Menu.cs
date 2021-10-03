using System;
using System.Collections.Generic;
using System.Linq;
using EasyCLI.Models.Styles;
using EasyCLI.Parsers;

// ReSharper disable once CheckNamespace
namespace EasyCLI
{
    using Context = Models.Context;
    
    public sealed class Menu
    {
        private const char Escape = '\\';
        private const char Separator = ' ';
        private readonly OptionsParser _optionsParser;
        private readonly Models.Executables.Namespace _commands;
        private readonly Style _style;
        private readonly string _greeting;
        private readonly string _farewell;
        private bool _isRunning;

        internal Menu(OptionsParser optionsParser,
                      string help,
                      Namespace rootNamespace,
                      Style style,
                      string greeting,
                      string farewell)
        {
            _optionsParser = optionsParser;
            _style = style;
            _commands = rootNamespace.Build(style, help);
            _greeting = greeting;
            _farewell = farewell;
        }

        public void StartDialogue()
        {
            _isRunning = true;
            Console.WriteLine(_greeting);
            Console.Write(_style.Separator);
            while (_isRunning)
            {
                Console.Write(_style.PromptIndicator);
                
                var inputs = new LinkedList<string>();
                do
                {
                    inputs.AddLast(Console.ReadLine());
                }
                while (inputs.Last.Value.EndsWith(Escape));

                var input = string.Join(Separator, inputs.Select(i => i.TrimEnd(Escape)));
                
                try
                {
                    Execute(input);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Console.Write(_style.Separator);
                }
            }

            Console.WriteLine(_farewell);
        }

        public void Execute(string command)
        {
            IEnumerable<string> tokens = command.Split(Separator, 
                                                       StringSplitOptions.RemoveEmptyEntries);
            Options options = _optionsParser.Parse(tokens);
            var context = new Context(this, options);
            _commands.Execute(context);
        }

        public void Terminate()
        {
            _isRunning = false;
        }

        public void Help()
        {
            _commands.Help();
        }
    }
}
