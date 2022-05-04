namespace PreparationToStream.FullText.DocumentBasedShortDocumentsFullTextIndex
{
    public class FullTextIndex
    {
        class Fragment
        {
            public int Start { get; set; }
            public int End { get; set; }
            public int DocumentId { get; set; }
        }

        private record IndexItem(int DocumentId, int Pos);

        private readonly FullTextGames.FullTextWithPositions.Lexer lexer = new();

        private readonly SimpleSearcher simpleSearch = new ();

        private readonly Dictionary<string, HashSet<int>> index = new Dictionary<string, HashSet<int>>();
        private readonly List<string> content = new List<string>();
        private readonly List<Fragment> fragments = new List<Fragment>();

        public const int ChunkSize = 64;

        public void AddStringToIndex(string item)
        {
            foreach (var tokenChunk in lexer.GetTokens(item).Chunk(ChunkSize))
            {
                var fragment = new Fragment() { DocumentId = content.Count, Start = tokenChunk[0].Position, End = tokenChunk[^1].Position };
                foreach (var token in tokenChunk)
                {
                    if (index.TryGetValue(token.Token, out var set))
                        set.Add(content.Count);
                    else
                        index[token.Token] = new HashSet<int>() { fragments.Count };
                }
                fragments.Add(fragment);
            }
            content.Add(item);
        }

        public IEnumerable<string> SearchWord(string word)
        {
            if (!index.TryGetValue(word, out var set))
                yield break;


            foreach (var doc in set)
            {
                var fragment = fragments[doc];
                var text = content[fragment.DocumentId];
                foreach (var match in simpleSearch.Search(word, text, fragment.Start, fragment.End))
                    yield return match;
            }
        }

        public IEnumerable<string> SearchTest(string word)
            => simpleSearch.Search(word, SearchWord(word));
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