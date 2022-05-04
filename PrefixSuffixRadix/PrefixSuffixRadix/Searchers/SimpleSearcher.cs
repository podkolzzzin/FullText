
#nullable disable


public class SimpleSearcher : ISercher
{
    private readonly string[] words;

    public SimpleSearcher(string[] words)
    {
        this.words = words;
    }

    public bool ContainsFullWord(string word) => words.Contains(word);

    public IEnumerable<string> ContainsSubstring(string suffix) => words.Where(x => x.Contains(suffix));

    public IEnumerable<string> StartWith(string prefix) => words.Where(x => x.StartsWith(prefix));
}

// ContainsFullWord
// StartsWith
// EndsWith
// ContaintsSubstring