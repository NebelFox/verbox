using System;
using System.Collections.Generic;

namespace Verbox
{
    /// <summary>
    /// Defines style of some visual aspects of the box it's applied to
    /// </summary>
    public class Style
    {
        /// <summary>
        /// Default style to be used if none specified.
        /// Also is a default base style for custom styles without one.
        /// </summary>
        public static readonly Style Default = new(new Dictionary<string, string>
                                                   {
                                                       ["dialogue.greeting"] = null,
                                                       ["dialogue.prompt-indicator"] = "$ ",
                                                       ["dialogue.semantic-separator"] = "\n",
                                                       ["dialogue.farewell"] = null,
                                                       ["input.separator"] = " ",
                                                       ["input.quotes"] = "'\"`",
                                                       ["input.new-line-escape"] = "\\",
                                                       ["help.lobby.title"] = null,
                                                       ["help.lobby.header"] = null,
                                                       ["help.lobby.footer"] = null,
                                                       ["help.namespace-member-format"] = "> {0} - {1}",
                                                       ["option.prefix"] = "--",
                                                   },
                                                   null);

        private readonly IReadOnlyDictionary<string, string> _aspects;
        private readonly Style _base;

        /// <summary>
        /// Constructs a new style from a dictionary.
        /// <see cref="Default"/> is used as the base style
        /// </summary>
        /// <param name="aspects">a dictionary of style aspects</param>
        public Style(IReadOnlyDictionary<string, string> aspects)
            : this(aspects, Default)
        { }

        /// <summary>
        /// Constructs a new style from a dictionary.
        /// </summary>
        /// <param name="aspects">a dictionary of style aspects</param>
        /// <param name="base">A base style to get omitted aspects from</param>
        /// <exception cref="ArgumentException">If any name of given aspects is not recognized</exception>
        public Style(IReadOnlyDictionary<string, string> aspects,
                     Style @base)
        {
            if (Default != null)
            {
                foreach (string key in aspects.Keys)
                {
                    if (Default._aspects.ContainsKey(key) == false)
                        throw new ArgumentException($"Unknown key: \"{key}\"");
                }
            }
            _aspects = aspects;
            _base = @base;
        }

        /// <summary>
        /// Gets the value of requested aspect.
        /// Tries to get it from the base style if missing
        /// </summary>
        /// <param name="key">Name of aspect</param>
        public string this[string key] => Get(key);

        private string Get(string key)
        {
            return _aspects.TryGetValue(key, out string value)
                ? value
                : _base != null
                    ? _base.Get(key)
                    : throw new ArgumentException($"Unknown aspect key: \"{key}\"");
        }

        /// <summary>
        /// Creates a copy of this instance but with different base style.
        /// So all its own aspects are saved,
        /// but all the rest would be seeked for in the new base style
        /// </summary>
        /// <param name="base">A new base style</param>
        /// <returns>Rebased copy</returns>
        public Style Rebase(Style @base)
        {
            return new Style(_aspects, @base);
        }

        /// <summary>
        /// Creates a new style with the same aspects and base style
        /// </summary>
        /// <returns>An identical to this instance style</returns>
        public Style Copy()
        {
            return new Style(_aspects, _base);
        }
    }
}
