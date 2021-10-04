using EasyCLI.Definitions.Executables;
using EasyCLI.Models.Styles;

// ReSharper disable once CheckNamespace
namespace EasyCLI
{
    public class MenuBuilder
    {
        private const char Delimiter = '\n';
        private string[] _greeting;
        private string[] _farewell;
        private string _title;
        private string[] _header;
        private string[] _footer;
        private readonly Namespace _rootNamespace;

        public MenuBuilder()
        {
            _rootNamespace = new Namespace();
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

        public Menu Build()
        {
            Style style = BuildStyle();
            string help = BuildHelp(style);
            return new Menu(help,
                            _rootNamespace,
                            style);
        }

        private Style BuildStyle()
        {
            return new Style(new DialogueStyle(string.Join('\n', _greeting),
                                               string.Join('\n', _farewell),
                                               "$ ",
                                               "\n"),
                             new InputStyle(' ',
                                            "'\"",
                                            '\\'),
                             new OptionsStyle("--",
                                             "=",
                                             "-",
                                             "--"),
                             new HelpStyle(" - "));
        }

        private string BuildHelp(Style style)
        {
            return string.Join($"{style.Dialogue.ExecutionSeparator}\n",
                               _title,
                               string.Join('\n', _header),
                               _rootNamespace.BuildHelp(style),
                               string.Join('\n', _footer));
        }
    }
}
