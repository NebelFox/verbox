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
        private readonly OptionsParser _optionsParser;
        private readonly Models.Executables.Namespace _commands;
        private readonly Style _style;
        private readonly Splitter _splitter;
        private bool _isRunning;

        internal Menu(string help,
                      Namespace rootNamespace,
                      Style style)
        {
            _style = style;
            _optionsParser = new OptionsParser(_style.Optionses);
            _commands = rootNamespace.Build(style, help);
            _splitter = new Splitter(_style.Input.Separator, _style.Input.Quotes);
        }

        public void StartDialogue()
        {
            _isRunning = true;
            Console.WriteLine(_style.Dialogue.Greeting);
            Console.Write(_style.Dialogue.ExecutionSeparator);
            while (_isRunning)
            {
                Prompt();
            }

            Console.WriteLine(_style.Dialogue.Farewell);
        }

        public void Prompt()
        {
            Console.Write(_style.Dialogue.PromptIndicator);

            var inputs = new LinkedList<string>();
            do
            {
                inputs.AddLast(Console.ReadLine());
            }
            // ReSharper disable once PossibleNullReferenceException
            while (inputs.Last.Value.EndsWith(_style.Input.NewLineEscape));

            var input = string.Join(_style.Input.Separator,
                                    inputs.Select(i => i.TrimEnd(_style.Input.NewLineEscape)));

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
                Console.Write(_style.Dialogue.ExecutionSeparator);
            }
        }

        public void Execute(string command)
        {
            IEnumerable<string> tokens = _splitter.Split(command);
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
