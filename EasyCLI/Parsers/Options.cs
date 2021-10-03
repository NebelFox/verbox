using System.Collections.Generic;

namespace EasyCLI.Parsers
{
    internal record Options(Dictionary<string, string> Kwargs,
                          HashSet<string> Switches,
                          LinkedList<string> Args)
    {
        public void AddKwarg(string key, string value)
        {
            Kwargs.Add(key, value);
        }

        public void AddSwitches(IEnumerable<string> keys)
        {
            Switches.UnionWith(keys);
        }

        public void AddArg(string value)
        {
            Args.AddLast(value);
        }
    }
}
