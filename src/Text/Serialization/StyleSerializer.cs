using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Verbox.Extensions;

namespace Verbox.Text.Serialization
{
    using Aspect = KeyValuePair<string, string>;

    /// <summary>
    /// Deserializes styles specification to <see cref="Style"/> from JSON
    /// </summary>
    public class StyleSerializer
    {
        private readonly Dictionary<string, Style> _styles;
        private readonly Dictionary<string, List<(string name, Style style)>> _pending;

        /// <summary>
        /// Constructs a new instance with no styles deserialized yet
        /// </summary>
        public StyleSerializer()
        {
            _styles = new Dictionary<string, Style> { ["default"] = Style.Default };
            _pending = new Dictionary<string, List<(string name, Style style)>>();
        }

        /// <summary>
        /// Grants access to the already deserialized styles by name
        /// </summary>
        /// <param name="name">name of the style</param>
        /// <exception cref="InvalidOperationException">If the requested style is already deserialized,
        /// but its base is not yet</exception>
        public Style this[string name] => Get(name);

        private Style Get(string name)
        {
            if (_styles.TryGetValue(name, out Style style))
                return style;
            foreach ((string @base, List<(string name, Style style)> styles) in _pending)
            {
                foreach ((string name, Style style) pair in styles)
                {
                    if (pair.name == name)
                        throw new InvalidOperationException($"Base style \"{@base}\" is not in deserialized list yet");
                }
            }
            return _styles[name];
        }

        /// <summary>
        /// Adds a <see cref="Style"/> instance to the list of
        /// deserialized styles with the specified name
        /// </summary>
        /// <param name="style">a style to add</param>
        /// <param name="name">the added style could be referred with it</param>
        /// <exception cref="ArgumentException">If a style with the specified name
        /// is already in the list of deserialized styles</exception>
        public void Add(Style style, string name)
        {
            if (_styles.ContainsKey(name))
                throw new ArgumentException($"Style duplicate: \"{name}\"");
            _styles[name] = style;
            OnStyleAdded(name);
        }

        /// <summary>
        /// Adds a style
        /// </summary>
        /// <param name="style"></param>
        /// <param name="baseName"></param>
        /// <param name="name"></param>
        public void Add(Style style, string baseName, string name)
        {
            if (_styles.TryGetValue(baseName, out Style @base))
            {
                Add(style.Rebase(@base), name);
            }
            else
            {
                if (_pending.ContainsKey(baseName) == false)
                    _pending[baseName] = new List<(string name, Style style)>();
                _pending[baseName].Add((name, style));
            }
        }

        /// <summary>
        /// Deserializes a sequence of named styles
        /// </summary>
        /// <param name="element">A JSON object of properties like "style-name: {...style-aspects...}</param>
        public void DeserializeMany(JsonElement element)
        {
            foreach (JsonProperty style in element.AssertValueKind("styles", JsonValueKind.Object).EnumerateObject())
                Deserialize(style);
        }

        /// <summary>
        /// Deserializes a JSON property to a style.
        /// Uses the property name as the style name.
        /// The property value must be a JSON object.
        /// </summary>
        /// <param name="property">A JSON property to deserialize</param>
        public void Deserialize(JsonProperty property)
        {
            Deserialize(property.Value, property.Name);
        }

        /// <summary>
        /// Deserializes a JSON object to a style with the specified name
        /// </summary>
        /// <param name="element">A JSON object of style aspects</param>
        /// <param name="name">Name to save the style with</param>
        public void Deserialize(JsonElement element, string name)
        {
            Style style = Deserialize(element, out string baseName);
            if (baseName == null)
                Add(style, name);
            else
                Add(style, baseName, name);
        }

        /// <summary>
        /// Deserializes a JSON object into an anonymous style without saving it.
        /// The style may have a base,
        /// but it must be already present in deserialization list
        /// </summary>
        /// <param name="element">A JSON object to deserialize</param>
        /// <returns>The deserialized style</returns>
        /// <exception cref="KeyNotFoundException">If the specified base style is missing</exception>
        public Style Deserialize(JsonElement element)
        {
            Style style = Deserialize(element, out string baseName);
            if (baseName == null)
                return style;
            if (_styles.TryGetValue(baseName, out Style @base) == false)
                throw new KeyNotFoundException($"Unknown base style: \"{baseName}\"");
            return style.Rebase(@base);
        }

        private static Style Deserialize(JsonElement element, out string baseName)
        {
            var aspects = new Dictionary<string, string>(DeserializeElement(element));
            aspects.Remove("base", out baseName);
            return new Style(aspects);
        }

        private static IEnumerable<Aspect> DeserializeElement(JsonElement element,
                                                              string path = null)
        {
            if (element.ValueKind == JsonValueKind.Object)
                foreach ((string key, string value) in Flatten(element, path))
                    yield return new Aspect(key, value);
            else
                yield return new Aspect(path, element.GetString());
        }

        private static IEnumerable<Aspect> Flatten(JsonElement element, string path)
        {
            return element.EnumerateObject()
                          .SelectMany(property => DeserializeElement(property.Value,
                                                                     AppendPath(path, property.Name)));
        }

        private static string AppendPath(string path, string suffix)
        {
            return path == null ? suffix : $"{path}.{suffix}";
        }

        private void OnStyleAdded(string name)
        {
            if (_pending.ContainsKey(name) == false)
                return;
            Style @base = _styles[name];
            foreach ((string styleName, Style style) in _pending[name])
                Add(style.Rebase(@base), styleName);
            _pending.Remove(name);
        }
    }
}
