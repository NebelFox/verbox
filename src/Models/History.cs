using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Verbox.Models
{
    internal class History
    {
        private readonly List<string[]> _entries;

        public History()
        {
            _entries = new List<string[]>();
        }

        private IReadOnlyList<string[]> Entries => _entries;

        internal void Append(string[] entry)
        {
            _entries.Add(entry);
        }

        public string Render(string separator,
                             int start = 0,
                             int length = -1,
                             bool index = true)
        {
            return string.Join('\n',
                               MakeRange(start, length)
                                  .Select(MakeSelector(separator, index)));
        }

        private Func<int, string> MakeSelector(string separator, bool index)
        {
            return index
                ? i => $"{i}. {Join(i, separator)}"
                : i => Join(i, separator);
        }

        private string Join(int index, string separator)
        {
            return string.Join(separator, Entries[index]);
        }

        public IEnumerable<string[]> GetRange(int start = 0, int length = 1)
        {
            return MakeRange(start, length)
               .Select(i => Entries[i]);
        }

        private IEnumerable<int> MakeRange(int start, int length)
        {
            if (length < 0 || start + length > Entries.Count)
                length = Entries.Count - start;
            return Enumerable.Range(start, length);
        }

        public void Save(string filepath,
                         string separator,
                         int start = 0,
                         int count = -1,
                         bool appendIfFileExists = true)
        {
            bool append = File.Exists(filepath) && appendIfFileExists;
            using var writer = new StreamWriter(filepath, append);
            writer.WriteLine(Render(separator,
                                start,
                                count,
                                false));
        }

        public void Clear()
        {
            _entries.Clear();
        }
    }
}
