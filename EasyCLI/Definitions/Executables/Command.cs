using System;
using System.Collections.Generic;
using EasyCLI.Models;
using EasyCLI.Models.Executables;
using EasyCLI.Models.Styles;
using EasyCLI.Definitions.Executables;

// ReSharper disable once CheckNamespace
namespace EasyCLI
{
    public class Command : ExecutableDefinition
    {
        private Action<Context> _action;
        private readonly LinkedList<(string, string)> _kwargs;
        private readonly LinkedList<(string, string)> _switches;
        private readonly LinkedList<(string, string)> _args;
        private readonly LinkedList<string> _examples;

        public Command(string name) : base(name)
        {
            _kwargs = new LinkedList<(string, string)>();
            _switches = new LinkedList<(string, string)>();
            _args = new LinkedList<(string, string)>();
            _examples = new LinkedList<string>();
        }

        public Command WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public Command Arg(string name, string description)
        {
            _args.AddLast((name, description));
            return this;
        }

        public Command Kwarg(string name, string description)
        {
            _kwargs.AddLast((name, description));
            return this;
        }

        public Command Switch(string name, string description)
        {
            _switches.AddLast((name, description));
            return this;
        }

        public Command Example(string input)
        {
            _examples.AddLast(input);
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
