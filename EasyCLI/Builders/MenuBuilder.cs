using EasyCLI.Definitions.Executables;
using EasyCLI.Models.Styles;
using EasyCLI.Parsers;

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
            var style = new Style("$ ", " - ", "\n");
            var help = string.Join($"{style.Separator}\n",
                                   _title,
                                   string.Join(style.Separator, _header),
                                   _rootNamespace.BuildHelp(style),
                                   string.Join(style.Separator, _footer));
            var optionsParser = new OptionsParser(new KwargParser("--", "="),
                                                  new SwitchParser("-", "--"));
            return new Menu(optionsParser,
                            help,
                            _rootNamespace,
                            style,
                            string.Join(style.Separator, _greeting),
                            string.Join(style.Separator, _farewell));
        }
    }
}
