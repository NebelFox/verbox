// ReSharper disable once CheckNamespace
namespace Verbox
{
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
