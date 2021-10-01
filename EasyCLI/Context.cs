using System.Collections.Generic;

namespace EasyCLI
{
    public record Context<TSource>(TSource Source,
                                   IReadOnlyList<string> Args,
                                   IReadOnlyDictionary<string, string> Kwargs,
                                   IReadOnlySet<string> Switches)
    {
        public string this[int index] => Args[index];
        public string this[string key] => Kwargs[key];
        public bool this[char key] => Switches.Contains(key.ToString());

        public bool IsSwitched(string key)
        {
            return Switches.Contains(key);
        }
    }
}
