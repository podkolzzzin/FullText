using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullTextGames.FullTextWithPositions
{
    // {
    //   "monday": [3456: [17, 1234, 9879],3456: [837], 9999: [99, 1111]]
    // }

    public class FullTextIndex
    {
        private readonly Dictionary<string, Dictionary<int, List<int>>> _index = new ();
        private readonly List<string> _content = new ();
        private readonly Lexer _lexer = new ();
        private readonly SimpleSearcher _searcher = new ();

        public void AddStringToIndex(string text)
        {
            int documentId = _content.Count;
            foreach (var token in _lexer.GetTokens(text))
            {
                if (_index.TryGetValue(token.Token, out var set))
                {
                    if (set.TryGetValue(documentId, out var positions))
                        positions.Add(token.Position);
                    else
                        set.Add(documentId, new List<int>() { token.Position });
                }
                else
                {
                    _index.Add(token.Token, new Dictionary<int, List<int>>()
                    {
                        [documentId] = new List<int>() { token.Position }
                    });
                }
            }
            _content.Add(text);
        }

        public Dictionary<int, List<int>> Search(string word)
        {
            word = word.ToLowerInvariant();
            if (_index.TryGetValue(word, out var set))
                return set;
            return new ();
        }

        public IEnumerable<string> SearchTest(string word)
        {
            var documentList = Search(word);
            foreach (var documentMatches in documentList)
            {
                foreach(var match in documentMatches.Value)
                    yield return _searcher.PrettyMatch(_content[documentMatches.Key], match);
            }
        }
    }

    class Lexer
    {
        public IEnumerable<(string Token, int Position)> GetTokens(string text)
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
                        yield return (GetToken(text, i, start), i);
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
