using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Text;
using Type = Verbox.Text.Type;

// ReSharper disable once CheckNamespace
namespace Verbox
{
    using Typeset = IReadOnlyDictionary<string, Type>;
    
    /// <summary>
    /// Is able to execute commands, prompt for input
    /// and conduct dialogues through the console.
    /// </summary>
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

        /// <summary>
        /// Prints the dialogue greeting,
        /// Continues to prompt and execute input until terminated
        /// and finally prints the dialogue farewell.
        /// </summary>
        public void StartDialogue()
        {
            _isRunning = true;
            Console.WriteLine(_style.DialogueGreeting);
            Console.Write(_style.DialogueSemanticSeparator);
            while (_isRunning)
                Prompt();

            Console.WriteLine(_style.DialogueFarewell);
        }

        /// <summary>
        /// Writes the dialogue prompt indicator,
        /// waits for an input from the console
        /// and then tries to execute the gain string command via <see cref="Execute"/>.
        /// </summary>
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

        /// <summary>
        /// Executes string command.
        /// </summary>
        /// <param name="command">concrete input</param>
        public void Execute(string command)
        {
            string[] tokens = _splitter.Split(command).ToArray();
            _root.Execute(this, tokens);
        }

        /// <summary>
        /// Stops the dialogue, if it's going on.
        /// </summary>
        public void Terminate()
        {
            _isRunning = false;
        }

        /// <summary>
        /// Prints the root help message.
        /// </summary>
        public void Help()
        {
            _root.Help();
        }
    }
}
