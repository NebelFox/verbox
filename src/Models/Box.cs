using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Text;
using Type = Verbox.Text.Type;

namespace Verbox
{
    using Typeset = IReadOnlyDictionary<string, Type>;

    /// <summary>
    /// Is able to execute commands, prompt for input
    /// and conduct dialogues through the console.
    /// </summary>
    public sealed class Box
    {
        private readonly Models.Executables.Namespace _root;
        private readonly Splitter _splitter;
        private bool _isRunning;

        internal Box(Models.Executables.Namespace root,
                     Style style)
        {
            Style = style;
            _root = root;
            _splitter = new Splitter(Style["input.separator"][0], Style["input.quotes"]);
        }

        /// <summary>
        /// Current style of the box
        /// </summary>
        public Style Style { get; }

        /// <summary>
        /// Prints the dialogue greeting,
        /// Continues to prompt and execute input until terminated
        /// and finally prints the dialogue farewell.
        /// </summary>
        public Box StartDialogue(bool showHelp = true)
        {
            _isRunning = true;
            Greet();
            Separate();
            if (showHelp)
            {
                Help();
                Separate();
            }
            while (_isRunning)
                Prompt();

            Farewell();

            return this;
        }

        /// <summary>
        /// Writes the dialogue prompt indicator,
        /// waits for an input from the console
        /// and then tries to execute the gain string command via <see cref="Execute"/>.
        /// </summary>
        public Box Prompt()
        {
            Console.Write(Style["dialogue.prompt-indicator"]);

            var inputs = new LinkedList<string>();
            do
                inputs.AddLast(Console.ReadLine());
            while (inputs.Last?.Value.EndsWith(Style["input.new-line-escape"]) ?? false);

            var input = string.Join(Style["input.separator"],
                                    inputs.Select(i => i.TrimEnd(Style["input.new-line-escape"][0])));

            try
            {
                Execute(input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Separate();
            }

            return this;
        }

        /// <summary>
        /// Executes an input and adds it to the history
        /// </summary>
        /// <param name="command">A string command to execute</param>
        public Box Execute(string command)
        {
            string[] tokens = _splitter.Split(command).ToArray();
            _root.Execute(this, tokens);
            return this;
        }

        /// <summary>
        /// Stops the dialogue, if it's going on.
        /// </summary>
        public Box Terminate()
        {
            _isRunning = false;
            return this;
        }

        /// <summary>
        /// Prints the root help message.
        /// </summary>
        public Box Help()
        {
            _root.Help();
            return this;
        }

        private void Greet()
        {
            Console.Write(Style["dialogue.greeting"]);
        }

        private void Separate()
        {
            Console.Write(Style["dialogue.semantic-separator"]);
        }

        private void Farewell()
        {
            Console.Write(Style["dialogue.farewell"]);
        }
    }
}
