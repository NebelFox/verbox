using System;
using System.Collections.Generic;
using Verbox.Definitions.Executables;
using Verbox.Extensions;
using Verbox.Text;
using Type = Verbox.Text.Type;

// ReSharper disable once CheckNamespace
namespace Verbox
{
    public class BoxBuilder
    {
        public delegate bool TryParse<TValue>(string name, out TValue value);

        private readonly Namespace _root;
        private readonly Dictionary<string, Type> _types;
        private Style _style;

        public BoxBuilder()
        {
            _root = new Namespace(null, null);
            _types = new Dictionary<string, Type>();
            Type("string", token => token);
            _style = Verbox.Style.Default;
        }

        public BoxBuilder Header(string header)
        {
            _header = header.Split(Delimiter);
            return this;
        }

        public BoxBuilder Command(ExecutableDefinition definition)
        {
            _root.Member(definition);
            return this;
        }

        public BoxBuilder Farewell(string farewell)
        {
            _farewell = farewell.Split(Delimiter);
            return this;
        }

        public BoxBuilder Type(string name, Type.ParseFunction parse)
        {
            _types.Add(name, new Type(name, parse));
            return this;
        }

        public BoxBuilder Type<TEnum>() where TEnum : struct, Enum
        {
            return Type<TEnum>(typeof(TEnum).Name.PascalToDash());
        }

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

        public BoxBuilder Type<TValue>(string name, TryParse<TValue> tryParse)
        {
            return Type(name,
                        token => tryParse(token, out TValue value) ? value : null);
        }

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
