using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Text;
using Type = Verbox.Text.Type;

// ReSharper disable once CheckNamespace
namespace Verbox
{
    using Typeset = IReadOnlyDictionary<string, Type>;
    
    public sealed class Box
    {
        private readonly Verbox.Models.Executables.Namespace _root;
        private readonly Style _style;
        private readonly Splitter _splitter;
        private bool _isRunning;

        internal Box(Verbox.Models.Executables.Namespace root,
                     Style style)
        {
            _style = style;
            _root = root;
            _splitter = new Splitter(_style.InputSeparator, _style.InputQuotes);
        }

        public void StartDialogue()
        {
            _isRunning = true;
            Console.WriteLine(_style.DialogueGreeting);
            Console.Write(_style.DialogueSemanticSeparator);
            while (_isRunning)
                Prompt();

            Console.WriteLine(_style.DialogueFarewell);
        }

        public void Prompt()
        {
            Console.Write(_style.DialoguePromptIndicator);

            var inputs = new LinkedList<string>();
            do
                inputs.AddLast(Console.ReadLine());
            while (inputs.Last.Value.EndsWith(_style.InputNewLineEscape));

            var input = string.Join(_style.InputSeparator,
                                    inputs.Select(i => i.TrimEnd(_style.InputNewLineEscape)));

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
                Console.Write(_style.DialogueSemanticSeparator);
            }
        }

        public void Execute(string command)
        {
            string[] tokens = _splitter.Split(command).ToArray();
            _root.Execute(this, tokens);
        }

        public void Terminate()
        {
            _isRunning = false;
        }

        public void Help()
        {
            _root.Help();
        }
    }
}
