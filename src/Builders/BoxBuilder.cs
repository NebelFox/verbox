using System;
using System.Collections.Generic;
using System.Linq;
using Verbox.Definitions.Executables;
using Verbox.Extensions;
using Verbox.Text;
using Type = Verbox.Text.Type;

// ReSharper disable once CheckNamespace
namespace Verbox
{
    /// <summary>
    /// Allows building boxes via chaining method calls
    /// </summary>
    public class BoxBuilder
    {
        /// <summary>
        /// Function, that tries to parse a string to TValue
        /// </summary>
        /// <typeparam name="TValue">type the function parses</typeparam>
        /// <param name="token">string to parse</param>
        /// <param name="value">parsed value if success</param>
        /// <returns>parsing success</returns>
        public delegate bool TryParse<TValue>(string token, out TValue value);

        private Namespace _root;
        private readonly List<Type> _types;
        private Style _style;

        /// <summary>
        /// Creates a new blank builder 
        /// </summary>
        public BoxBuilder()
        {
            _root = new Namespace(null, null);
            _types = new List<Type>();
            Type("string", token => token);
            _style = Verbox.Style.Default;
        }

        /// <summary>
        /// Adds an executable near an already added one with target name.
        /// Adds either before or after the specified executable.
        /// </summary>
        /// <param name="definition">An executable to add</param>
        /// <param name="targetExecutableName">Name of an executable to add near</param>
        /// <param name="before">Adds before the specified executable, of true; Otherwise, after it.</param>
        /// <returns></returns>
        public BoxBuilder CommandNear(ExecutableDefinition definition,
                                      string targetExecutableName,
                                      bool before = false)
        {
            _root.InsertCommandNear(definition, targetExecutableName, before);
            return this;
        }

        /// <summary>
        /// Adds a command/namespace to the box root namespace
        /// </summary>
        /// <param name="definition">an executable definition to add</param>
        /// <param name="prepend">if true - inserts at the beginning,
        /// otherwise at the back</param>
        public BoxBuilder Command(ExecutableDefinition definition,
                                  bool prepend = false)
        {
            _root.Command(definition, prepend);
            return this;
        }

        /// <summary>
        /// Adds a new recognizable value type to the box
        /// </summary>
        /// <param name="name">type name</param>
        /// <param name="parse">function for parsing to the type from string</param>
        public BoxBuilder Type(string name, ParseFunction parse)
        {
            _types.Add(new Type(name, parse));
            return this;
        }

        /// <summary>
        /// Invokes <see cref="Type{TEnum}(string)"/> via name as TEnum name in dash-case.
        /// </summary>
        public BoxBuilder Type<TEnum>() where TEnum : struct, Enum
        {
            return Type<TEnum>(typeof(TEnum).Name.PascalToDash());
        }

        /// <summary>
        /// Registers a new recognizable type for the box.
        /// Invokes <see cref="Type"/> via generated parse function
        /// </summary>
        /// <param name="name">type name</param>
        /// <typeparam name="TEnum">enumeration to build the type on</typeparam>
        public BoxBuilder Type<TEnum>(string name) where TEnum : struct, Enum
        {
            return Type(name,
                        token =>
                        {
                            if (Enum.TryParse(token.DashToPascal(), out TEnum value))
                                return value;
                            return null;
                        });
        }

        /// <summary>
        /// Invokes <see cref="Type"/> via generated parse function
        /// </summary>
        /// <param name="name">type name</param>
        /// <param name="tryParse">function parsing a string to TValue</param>
        /// <typeparam name="TValue">tryParse out type</typeparam>
        public BoxBuilder Type<TValue>(string name, TryParse<TValue> tryParse)
        {
            return Type(name,
                        token => tryParse(token, out TValue value) ? value : null);
        }

        /// <summary>
        /// Sets the box style
        /// </summary>
        /// <param name="style">Style to use</param>
        public BoxBuilder Style(Style style)
        {
            _style = style;
            return this;
        }

        /// <summary>
        /// Returns <see cref="ExecutableDefinition"/> by the requested path
        /// </summary>
        /// <param name="path">path to executable</param>
        /// <returns><see cref="ExecutableDefinition"/></returns>
        /// <exception cref="ArgumentException">if such path doesn't exist</exception>
        public ExecutableDefinition GetByPath(string path)
        {
            string[] parts = path.Split(_style["input.separator"]);
            ExecutableDefinition definition = _root.Get(parts);
            return definition
                ?? throw new ArgumentException($"Invalid executable path: {path}",
                                               nameof(path));
        }

        /// <summary>
        /// For each dictionary pair gets it's target command by specified path
        /// and appoints action in pair to it
        /// </summary>
        /// <param name="actions">Dictionary of pairs "path:action"</param>
        /// <exception cref="InvalidOperationException">The executable by a path is not a command</exception>
        /// <exception cref="ArgumentException">A path doesn't exist</exception>>
        public void SetActionsByPaths(IReadOnlyDictionary<string, Action<Context>> actions)
        {
            foreach ((string path, Action<Context> action) in actions)
            {
                if (GetByPath(path) is not Command command)
                    throw new InvalidOperationException($"Executable at \"{path}\" must be a command, not a namespace");
                command.Action(action);
            }
        }

        /// <summary>
        /// For each command that has no action set yet,
        /// sets the given function as action
        /// </summary>
        /// <param name="action">a function to set to all commands without action</param>
        public void SetMissingActions(Action<Context> action)
        {
            _root.SetMissingActions(action);
        }

        /// <summary>
        /// For each command without any action already set,
        /// sets the action to a function that does nothing
        /// </summary>
        public void SetMissingActionsToDummy()
        {
            _root.SetMissingActions(_ => { });
        }

        /// <summary>
        /// Constructs a new box based on current state.
        /// Doesn't clear the state
        /// </summary>
        /// <returns>a new <see cref="Box"/> instance</returns>
        public Box Build()
        {
            return new Box(_root.Build(_style,
                                       BuildHelp(),
                                       BuildTypeset()),
                           _style);
        }

        private string BuildHelp()
        {
            return _style["dialogue.semantic-separator"]
               .JoinMeaningful(_style["help.lobby.title"],
                             _style["help.lobby.header"],
                             _root.BuildHelp(_style),
                             _style["help.lobby.footer"]);
        }

        private IReadOnlyDictionary<string, Type> BuildTypeset()
        {
            return new Dictionary<string, Type>(_types.Select(t => new KeyValuePair<string, Type>(t.Name, t)));
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="BoxBuilder"/> instance
        /// </summary>
        /// <returns>An independent copy of this <see cref="BoxBuilder"/></returns>
        public BoxBuilder Copy()
        {
            var copy = new BoxBuilder
            {
                _root = _root.Copy(),
                _style = _style
            };
            copy._types.AddRange(_types);
            return copy;
        }
    }
}
