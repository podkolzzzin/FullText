using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextGames.SimpleFullTextIndex
{
    // {
    //   "monday": [3456,3456,9876,346]
    // }
    public class FullTextIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index = new Dictionary<string, HashSet<int>>();
        private readonly List<string> _content = new List<string>();
        private readonly Lexer _lexer = new Lexer();
        private readonly SimpleSearcher _searcher = new SimpleSearcher();

        public void AddStringToIndex(string text)
        {
            int documentId = _content.Count;
            foreach (var token in _lexer.GetTokens(text))
            {
                if (_index.TryGetValue(token, out var set))
                    set.Add(documentId);
                else
                    _index.Add(token, new HashSet<int>() { documentId });
            }
            _content.Add(text);
        }

        public ISet<int> Search(string word)
        {
            word = word.ToLowerInvariant();
            if (_index.TryGetValue(word, out var set))
                return set;
            return new HashSet<int>();// O(1) -> Except O(n)
            // SortedSet O(ln(N)) -> Except O(Nln(N));
        }

        public IEnumerable<int> Search(string[] words)
        {
            var sets = words.Select(x => Search(x)).ToArray();
            var result = sets[0].Intersect(sets[1]);
            for (int i = 2; i < sets.Length; i++)
                result = result.Intersect(sets[i]);
            return result;
        }


        public IEnumerable<string> SearchTest(string word)
        {
            var documentList = Search(word);
            foreach (var docId in documentList)
            {
                foreach (var match in _searcher.Search(word, _content[docId]))
                    yield return match;
            }
        }
    }

    class Lexer
    {
        public IEnumerable<string> GetTokens(string text)
        {
            int start = -1;
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLetterOrDigit(text[i]))
                {
                    if (start == -1)
                        start = i;
                }
                else
                {
                    if (start >= 0)
                    {
                        yield return GetToken(text, i, start);
                        start = -1;
                    }
                }

            }
        }

        private string GetToken(string text, int i, int start)
        {
            return text.Substring(start, i - start).Normalize().ToLowerInvariant();
        }
    }
}
