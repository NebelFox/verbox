using System;
using System.Collections.Generic;
using Verbox.Definitions.Executables;
using Verbox.Extensions;
using Verbox.Models.Styles;
using Type = Verbox.Text.Type;

// ReSharper disable once CheckNamespace
namespace Verbox
{
    public class MenuBuilder
    {
        public delegate bool TryParse<TValue>(string name, out TValue value);

        private const char Delimiter = '\n';
        private string[] _greeting;
        private string[] _farewell;
        private string _title;
        private string[] _header;
        private string[] _footer;
        private readonly Namespace _rootNamespace;
        private readonly Dictionary<string, Type> _types;

        public MenuBuilder()
        {
            _rootNamespace = new Namespace();
            _types = new Dictionary<string, Type>();
        }

        public MenuBuilder Greeting(string greeting)
        {
            _greeting = greeting.Split(Delimiter);
            return this;
        }

        public MenuBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public MenuBuilder Header(string header)
        {
            _header = header.Split(Delimiter);
            return this;
        }

        public MenuBuilder Command(ExecutableDefinition definition)
        {
            _rootNamespace.Member(definition);
            return this;
        }

        public MenuBuilder Footer(string footer)
        {
            _footer = footer.Split(Delimiter);
            return this;
        }

        public MenuBuilder Farewell(string farewell)
        {
            _farewell = farewell.Split(Delimiter);
            return this;
        }

        public MenuBuilder Type(string name, Type.ParseFunction parse)
        {
            _types.Add(name, new Type(name, parse));
            return this;
        }

        public MenuBuilder Type<TEnum>() where TEnum : struct, Enum
        {
            return Type<TEnum>(typeof(TEnum).Name.PascalToDash());
        }

        public MenuBuilder Type<TEnum>(string name) where TEnum : struct, Enum
        {
            return Type(name,
                        token =>
                        {
                            if (Enum.TryParse(token.DashToPascal(), out TEnum value))
                                return value;
                            return null;
                        });
        }

        public MenuBuilder Type<TValue>(string name, TryParse<TValue> tryParse)
        {
            return Type(name,
                        token => tryParse(token, out TValue value) ? value : null);
        }

        public Menu Build()
        {
            Style style = BuildStyle();
            string help = BuildHelp(style);
            return new Menu(help,
                            _rootNamespace,
                            style,
                            _types);
        }

        private Style BuildStyle()
        {
            return new Style(
                new DialogueStyle(string.Join('\n', _greeting),
                                  string.Join('\n', _farewell),
                                  "$ ",
                                  "\n"),
                new InputStyle(' ',
                               "'\"",
                               '\\'),
                new OptionStyle("--"),
                new HelpStyle("> {0} - {1}"));
        }

        private string BuildHelp(Style style)
        {
            return string.Join($"{style.Dialogue.SemanticSeparator}\n",
                               _title,
                               string.Join('\n', _header),
                               _rootNamespace.BuildHelp(style),
                               string.Join('\n', _footer));
        }
    }
}
