#nullable disable

// See https://aka.ms/new-console-template for more information
public class TreeSearcher : ISercher
{
    private PrefixTree tree;

    public TreeSearcher(string[] words)
    {
        tree = new(words);
    }

    public bool ContainsFullWord(string word) => tree.ContainsFullWord(word);

    public IEnumerable<string> ContainsSubstring(string suffix)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> StartWith(string prefix)
    {
        return tree.StartsWith(prefix);
    }
}

public class SuffixTreeSearcher : ISercher
{
    private SuffixTree tree;

    public SuffixTreeSearcher(string[] words)
    {
        tree = new SuffixTree(words);
    }

    public bool ContainsFullWord(string word)
    {
        return false;
    }

    public IEnumerable<string> ContainsSubstring(string suffix)
    {
        return tree.ContainsString(suffix);
    }

    public IEnumerable<string> StartWith(string prefix)
    {
        throw new NotImplementedException();
    }
}