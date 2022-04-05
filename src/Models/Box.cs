using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verbox.Extensions;
using Verbox.Models;
using Verbox.Text;
using Verbox.Text.Tokens;
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
        private readonly History _history;
        private readonly Tokenizer _tokenizer;
        private bool _isRunning;

        internal Box(Models.Executables.Namespace root,
                     Style style)
        {
            Style = style;
            _root = root;
            _splitter = new Splitter(Style["input.separator"][0], Style["input.quotes"]);
            _history = new History();
            _tokenizer = new Tokenizer(style["input.quotes"],
                                       style["option.prefix"],
                                       style["input.delimiter.short"],
                                       style["input.delimiter.long"]);
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
            {
                Prompt();
                Separate();
            }

            Farewell();

            return this;
        }

        /// <summary>
        /// Writes the dialogue prompt indicator,
        /// waits for an input from the console
        /// and then tries to execute the gain string command.
        /// </summary>
        public Box Prompt()
        {
            Console.Write(Style["dialogue.prompt-indicator"]);

            var inputs = new LinkedList<string>();
            do
                inputs.AddLast(Console.ReadLine());
            while (inputs.Last?.Value.EndsWith(Style["input.new-line-escape"]) ?? false);

            var input = string.Join(Style["input.separator"],
                                    inputs.Select(i => i.ChopTail(Style["input.new-line-escape"])));

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
        /// <param name="notice">Whether to add the execution to the history</param>
        public Box Execute(string command, bool notice = true)
        {
            Token[] tokens = _tokenizer.Tokenize(_splitter.Split(command));
            Execute(tokens);
            return this;
        }

        private void Execute(Token[] command, bool notice = true)
        {
            _root.Execute(this, command);
            if (notice)
                _history.Append(command);
        }

        /// <summary>
        /// Sequentially executes all the commands
        /// </summary>
        /// <param name="commands">A sequence of commands</param>
        /// <param name="notice">Whether to add the execution to the history</param>
        public Box Execute(IEnumerable<string> commands, bool notice = true)
        {
            foreach (string command in commands)
                Execute(command, notice);
            return this;
        }

        /// <summary>
        /// Executes a sequence of commands.
        /// Designed for in-code use
        /// </summary>
        /// <param name="commands"></param>
        public Box Execute(params string[] commands)
        {
            return Execute(commands.AsEnumerable());
        }

        /// <summary>
        /// Sequentially executes each line of the file
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="notice">Whether to add the execution to the history</param>
        public Box ExecuteScript(string filepath, bool notice = true)
        {
            return Execute(File.ReadLines(filepath), notice);
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

        ///<summary>
        /// Displays the specific span of input history
        /// </summary>
        /// <param name="start">Starting index of the span</param>
        /// <param name="length">Length of the span</param>
        public Box ShowHistory(int start = 0,
                               int length = -1)
        {
            Console.WriteLine(_history.Render(Style["input.separator"],
                                              start,
                                              length));
            return this;
        }

        /// <summary>
        /// Writes the specific span of the history to a file.
        /// Each noticed command on a new line.
        /// </summary>
        /// <param name="filepath">A file to write to</param>
        /// <param name="start">Start of the span</param>
        /// <param name="length">Length of the span</param>
        /// <param name="appendIfFileExists">Whether to append or overwrite the existing file</param>
        public Box SaveHistory(string filepath,
                               int start = 0,
                               int length = -1,
                               bool appendIfFileExists = true)
        {
            _history.Save(filepath,
                          Style["input.separator"],
                          start,
                          length,
                          appendIfFileExists);
            return this;
        }

        /// <summary>
        /// Repeats the specific span of the input history 1 or more times
        /// </summary>
        /// <param name="start">Starting index of the span</param>
        /// <param name="length">Length of the span</param>
        /// <param name="times">How many times to repeat the span</param>
        /// <param name="notice">Whether to add the execution to the history</param>
        public Box Repeat(int start,
                          int length = 1,
                          int times = 1,
                          bool notice = true)
        {
            for (var i = 0; i < times; ++i)
            {
                foreach (Token[] command in _history.GetRange(start, length))
                    Execute(command, notice);
            }
            return this;
        }

        /// <summary>
        /// Clears the current history
        /// </summary>
        public Box ClearHistory()
        {
            _history.Clear();
            return this;
        }

        private void Greet()
        {
            Console.WriteLine(Style["dialogue.greeting"]);
        }

        /// <summary>
        /// Writes the dialogue.semantic-separator to the console
        /// </summary>
        public Box Separate()
        {
            Console.Write(Style["dialogue.semantic-separator"]);
            return this;
        }

        private void Farewell()
        {
            Console.WriteLine(Style["dialogue.farewell"]);
        }
    }
}
