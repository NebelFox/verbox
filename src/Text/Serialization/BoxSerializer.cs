using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Verbox.Definitions.Executables;
using Verbox.Extensions;

namespace Verbox.Text.Serialization
{
    /// <summary>
    /// Deserializes boxes specification to from JSON <see cref="BoxBuilder"/>.
    /// Gets required styles from the provided <see cref="StyleSerializer"/>
    /// </summary>
    public class BoxSerializer
    {
        private delegate void BoxExtender(BoxBuilder builder);

        private record Prefab(BoxExtender Extender, string Base = null);

        private readonly StyleSerializer _styles;
        private readonly Dictionary<string, Prefab> _boxes;

        /// <summary>
        /// Constructs a new instance with given <see cref="StyleSerializer"/>
        /// as the styles main source
        /// </summary>
        /// <param name="styles">Where to get the required styles from</param>
        public BoxSerializer(StyleSerializer styles)
        {
            _boxes = new Dictionary<string, Prefab>();
            _styles = styles;
        }

        /// <summary>
        /// Returns a <see cref="BoxBuilder"/> by name
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="KeyNotFoundException">If such box was not deserialized yet</exception>
        public BoxBuilder this[string name] => Get(name);

        private BoxBuilder Get(string name)
        {
            if (_boxes.TryGetValue(name, out Prefab pair) == false)
                throw new KeyNotFoundException($"Unknown base box: {name}");
            BoxBuilder builder = pair.Base != null ? Get(pair.Base) : new BoxBuilder();
            pair.Extender.Invoke(builder);
            return builder;
        }

        /// <summary>
        /// Deserializes a sequence of named boxes
        /// </summary>
        /// <param name="element">A JSON object of properties like "box-name: {...box-specification...}"</param>
        public void DeserializeMany(JsonElement element)
        {
            foreach (JsonProperty style in element.AssertValueKind("element", JsonValueKind.Object)
                                                  .EnumerateObject())
                Deserialize(style);
        }

        /// <summary>
        /// Deserializes a named box.
        /// The property name is used as the box name.
        /// </summary>
        /// <param name="property">A Json property</param>
        public void Deserialize(JsonProperty property)
        {
            Deserialize(property.Value, property.Name);
        }

        /// <summary>
        /// Deserializes an object to box and appoints the given name to it
        /// </summary>
        /// <param name="element">A JSON object with the box specification</param>
        /// <param name="name">A name the box could be referred with</param>
        /// <exception cref="ArgumentException">If a box with the specified name was already deserialized</exception>
        public void Deserialize(JsonElement element, string name)
        {
            if (_boxes.ContainsKey(name))
                throw new ArgumentException($"Box duplicate: \"{name}\"");
            _boxes[name] = Deserialize(element);
        }

        private Prefab Deserialize(JsonElement element)
        {
            JsonProperty commands = element.AssertValueKind("box", JsonValueKind.Object)
                                           .GetAliasedProperty("box",
                                                               "commands",
                                                               "insert",
                                                               "insert-command-blocks");

            BoxExtender extender = commands.Name switch
            {
                "commands"                          => DeserializeCommands(commands.Value),
                "insert" or "insert-command-blocks" => DeserializeInsert(commands.Value)
            };

            if (!element.TryGetProperty("style", out JsonElement styleElement))
                return new Prefab(extender, GetBoxBase(element));
            switch (styleElement.ValueKind)
            {
            case JsonValueKind.Object:
            {
                Style style = _styles.Deserialize(styleElement);
                extender += builder => builder.Style(style);
                break;
            }
            case JsonValueKind.String:
                extender += builder => builder.Style(_styles[styleElement.GetString()]);
                break;
            default:
                throw new FormatException("Box style element must be either an object or a string");
            }

            return new Prefab(extender, GetBoxBase(element));
        }

        private static BoxExtender DeserializeInsert(JsonElement element)
        {
            return element.AssertValueKind("insert",
                                           JsonValueKind.Array,
                                           JsonValueKind.Object)
                          .ValueKind switch
            {
                JsonValueKind.Object => DeserializeCommandsInsertBlock(element),
                JsonValueKind.Array => element.EnumerateArray()
                                              .Select(DeserializeCommandsInsertBlock)
                                              .Aggregate((a, b) => a + b)
            };
        }

        private static BoxExtender DeserializeCommands(JsonElement element)
        {
            return InsertCommandsAt(DeserializeExecutables(element), "end");
        }

        private static BoxExtender DeserializeCommandsInsertBlock(JsonElement block)
        {
            block.AssertValueKind("commands insert block", JsonValueKind.Object);

            JsonProperty value = block.GetAliasedProperty("commands insert block",
                                                          "before",
                                                          "at",
                                                          "after");
            if (block.TryGetProperty("commands", out JsonElement commands) == false)
                throw new FormatException("commands insert block missed mandatory \"commands\" property");
            string position = value.Value.AssertValueKind("before|at|after", JsonValueKind.String).GetString();
            ExecutableDefinition[] definitions = DeserializeExecutables(commands);
            return value.Name switch
            {
                "before" => InsertCommandsBefore(definitions, position),
                "at"     => InsertCommandsAt(definitions, position),
                "after"  => InsertCommandsAfter(definitions, position)
            };
        }

        private static BoxExtender InsertCommandsBefore(IReadOnlyList<ExecutableDefinition> definitions,
                                                        string position)
        {
            return builder =>
            {
                builder.CommandNear(definitions[0], position, true);
                for (var i = 1; i < definitions.Count; i++)
                    builder.CommandNear(definitions[i], definitions[i - 1].Name);
            };
        }

        private static BoxExtender InsertCommandsAt(IReadOnlyList<ExecutableDefinition> definitions,
                                                    string position)
        {
            return position switch
            {
                "begin" => builder =>
                {
                    for (int i = definitions.Count - 1; i > -1; --i)
                        builder.Command(definitions[i], true);
                },
                "end" => builder =>
                {
                    foreach (ExecutableDefinition definition in definitions)
                        builder.Command(definition);
                },
                _ => throw new ArgumentException("The position must be either \"begin\" or \"end\"")
            };
        }

        private static BoxExtender InsertCommandsAfter(IReadOnlyList<ExecutableDefinition> definitions,
                                                       string position)
        {
            return builder =>
            {
                builder.CommandNear(definitions[0], position);
                for (var i = 1; i < definitions.Count; i++)
                    builder.CommandNear(definitions[i], definitions[i - 1].Name);
            };
        }

        private static BoxExtender AppendCommands(JsonElement commands)
        {
            BoxExtender extender = null;
            foreach (JsonElement command in commands.EnumerateArray())
            {
                ExecutableDefinition definition = DeserializeExecutable(command);
                extender += builder => builder.Command(definition);
            }
            return extender;
        }

        private static ExecutableDefinition[] DeserializeExecutables(JsonElement definitionsArray)
        {
            return definitionsArray.AssertValueKind("array of executable definitions", JsonValueKind.Array)
                                   .EnumerateArray()
                                   .Select(DeserializeExecutable)
                                   .ToArray();
        }

        private static ExecutableDefinition DeserializeExecutable(JsonElement definition)
        {
            if (definition.ValueKind == JsonValueKind.String)
                return new Command(definition.GetString(), null);

            string name = definition.GetPropertyString("name", "definition");
            string brief = definition.GetOptionalPropertyString("brief");
            string description = definition.GetOptionalPropertyString("description");

            if (definition.TryGetProperty("commands", out JsonElement commands))
            {
                var @namespace = new Namespace(name, brief);
                if (description != null)
                    @namespace.WithDescription(description);
                foreach (JsonElement cmd in commands.AssertValueKind("members", JsonValueKind.Array)
                                                    .EnumerateArray())
                    @namespace.Command(DeserializeExecutable(cmd));

                return @namespace;
            }

            var command = new Command(name, brief);
            if (description != null)
                command.WithDescription(description);

            if (definition.TryGetAliasedProperty(out JsonProperty parameters, "parameters", "params"))
            {
                parameters.Value.AssertValueKind($"\"{parameters.Name}\" value",
                                                 JsonValueKind.Array,
                                                 JsonValueKind.String);
                if (parameters.Value.ValueKind == JsonValueKind.Array)
                    command.Parameters(parameters.Value.EnumerateArray().Select(p => p.GetString()).ToArray());
                else
                    command.Parameters(parameters.Value.GetString());
            }

            if (definition.TryGetProperty("examples", out JsonElement examples))
                command.Examples(examples.EnumerateArray().Select(p => p.GetString()).ToArray());

            return command;
        }

        private static string GetBoxBase(JsonElement element)
        {
            return element.TryGetProperty("base", out JsonElement baseElement)
                ? baseElement.AssertValueKind("base", JsonValueKind.String).GetString()
                : null;
        }
    }
}
