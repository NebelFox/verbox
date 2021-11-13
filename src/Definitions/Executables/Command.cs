using System;
using System.Collections.Generic;
using Verbox.Definitions;
using Verbox.Definitions.Executables;
using Verbox.Models;
using Verbox.Models.Executables;
using Verbox.Text.Tokens;
using Type = Verbox.Text.Type;

// ReSharper disable once CheckNamespace
namespace Verbox
{
    using Typeset = IReadOnlyDictionary<string, Type>;
    
    public class Command : ExecutableDefinition
    {
        private Action<Context> _action;
        private readonly SignatureDefinition _signature;
        private readonly List<string> _examples;

        public Command(string name, string brief) : base(name, brief)
        {
            _signature = new SignatureDefinition();
            _examples = new List<string>();
        }

        public Command WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public Command Parameters(params string[] definitions)
        {
            foreach (string definition in definitions)
                _signature.Parameter(definition);
            return this;
        }

        public Command Examples(params string[] examples)
        {
            _examples.AddRange(examples);
            return this;
        }

        public Command Action(Action<Context> action)
        {
            _action = action;
            return this;
        }

        internal override Executable Build(Style style, Typeset typeset)
        {
            return new Models.Executables.Command(BuildHelp(style), 
                                                  _action, 
                                                  _signature.Build(typeset,
                                                                   new Tokenizer("'\"", style.OptionPrefix)));
        }

        internal override string BuildHelp(Style style)
        {
            var parts = new List<string>();
            if (Description != null)
                parts.Add($"{Description}\n");
            parts.Add(_signature.BuildHelp());
            if (_examples.Count != 0)
            {
                parts.Add("\nExamples:");
                parts.AddRange(_examples);
            }
            return string.Join('\n', parts);
        }
    }
}
