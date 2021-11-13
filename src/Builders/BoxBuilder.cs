using System;
using System.Collections.Generic;
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

        private readonly Namespace _root;
        private readonly Dictionary<string, Type> _types;
        private Style _style;

        /// <summary>
        /// Creates a new blank builder 
        /// </summary>
        public BoxBuilder()
        {
            _root = new Namespace(null, null);
            _types = new Dictionary<string, Type>();
            Type("string", token => token);
            _style = Verbox.Style.Default;
        }

        /// <summary>
        /// Adds a new command/namespace to the box root namespace
        /// </summary>
        public BoxBuilder Command(ExecutableDefinition definition)
        {
            _root.Member(definition);
            return this;
        }


        /// <summary>
        /// Adds a new recognizable value type to  box
        /// </summary>
        /// <param name="name">type name</param>
        /// <param name="parse">function for parsing to the type from string</param
        public BoxBuilder Type(string name, ParseFunction parse)
        {
            _types.Add(name, new Type(name, parse));
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
        /// Sets the style the box is built with
        /// </summary>
        public BoxBuilder Style(Style style)
        {
            _style = style;
            return this;
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
                                       _types),
                           _style);
        }
        
        private string BuildHelp()
        {
            return string.Join($"{_style.DialogueSemanticSeparator}\n",
                               _style.HelpLobbyTitle,
                               _style.HelpLobbyHeader,
                               _root.BuildHelp(_style),
                               _style.HelpLobbyFooter);
        }

    }
}
