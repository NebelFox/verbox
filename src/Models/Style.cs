// ReSharper disable once CheckNamespace
namespace Verbox
{
    /// <summary>
    /// Defines style of mostly visual aspects of the box it's applied to
    /// </summary>
    /// <param name="DialogueGreeting">Dialogue start message</param>
    /// <param name="DialogueFarewell">Dialogue end message</param>
    /// <param name="DialoguePromptIndicator">The start of the prompt line</param>
    /// <param name="DialogueSemanticSeparator">Separates messages and text blocks</param>
    /// <param name="InputSeparator">Is treated as the tokens separator in input</param>
    /// <param name="InputQuotes">Each character from this string may be used
    /// for quoted escape</param>
    /// <param name="InputNewLineEscape">Placing it at the end of an input
    /// allows to continue the input at the next line </param>
    /// <param name="OptionPrefix">Token starting with this string is treated as an option</param>
    /// <param name="HelpLobbyTitle">Text at the very top of the box root help message.
    /// Is meant to contain the program/box brief</param>
    /// <param name="HelpLobbyHeader">Text above commands list in the box root help message</param>
    /// <param name="HelpLobbyFooter">Text below commands list in the box root help message</param>
    /// <param name="HelpNamespaceMemberFormat">String template with 2 placeholders -
    /// for command/namespace name and brief respectively</param>
    public record Style(string DialogueGreeting,
                        string DialogueFarewell,
                        string DialoguePromptIndicator,
                        string DialogueSemanticSeparator,
                        char InputSeparator,
                        string InputQuotes,
                        char InputNewLineEscape,
                        string OptionPrefix,
                        string HelpLobbyTitle,
                        string HelpLobbyHeader,
                        string HelpLobbyFooter,
                        string HelpNamespaceMemberFormat)
    {
        private static Style s_Default;
        
        /// <summary>
        /// Default style. Is meant to be used as "Style.Default with {...}"
        /// </summary>
        public static Style Default => s_Default ??= CreateDefaultStyle();

        private static Style CreateDefaultStyle()
        {
            return new Style("Welcome",
                             "Exiting the program...",
                             "$ ",
                             "\n\n",
                             ' ',
                             "'`\"",
                             '\\',
                             "--",
                             null,
                             null,
                             "> {0} - {1}",
                             null);
        }
    }
}
