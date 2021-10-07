using System;
using System.Collections.Generic;
using System.Linq;
using EasyCLI.Models;
using EasyCLI.Models.Executables;
using EasyCLI.Models.Styles;
using EasyCLI.Definitions.Executables;
using EasyCLI.Definitions.Options;
using EasyCLI.Properties;

// ReSharper disable once CheckNamespace
namespace EasyCLI
{
    public class Command : ExecutableDefinition
    {
        private Action<Context> _action;
        private readonly LinkedList<KwargDefinition> _kwargs;
        private readonly LinkedList<SwitchDefinition> _switches;
        private readonly LinkedList<ArgDefinition> _args;
        private readonly LinkedList<(string, string)> _examples;

        public Command(string name, string brief) : base(name, brief)
        {
            _kwargs = new LinkedList<KwargDefinition>();
            _switches = new LinkedList<SwitchDefinition>();
            _args = new LinkedList<ArgDefinition>();
            _examples = new LinkedList<(string, string)>();
        }

        public Command WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public Command Kwarg(string name, string description, string defaultValue)
        {
            _kwargs.AddLast(new KwargDefinition(name, description, defaultValue));

            return this;
        }

        public Command Switch(string name, string description, char shortForm)
        {
            _switches.AddLast(new SwitchDefinition(name, description, shortForm));

            return this;
        }

        public Command Arg(string name, string description, ArgTags tags = ArgTags.None)
        {
            _args.AddLast(new ArgDefinition(name, description, tags));

            return this;
        }

        public Command Example(string input, string explanation)
        {
            _examples.AddLast((input, explanation));

            return this;
        }

        public Command Action(Action<Context> action)
        {
            _action = action;
            return this;
        }

        internal override Executable Build(Style style)
        {
            return new Models.Executables.Command(BuildHelp(style), _action);
        }

        internal override string BuildHelp(Style style)
        {
            return string.Empty;
        }
    }
}
