#nullable disable

// See https://aka.ms/new-console-template for more information
public class HashsetSearcher : ISercher
{
    private HashSet<string> hashset;

    public HashsetSearcher(string[] words)
    {
        hashset = words.ToHashSet();
    }

    public bool ContainsFullWord(string word) => hashset.Contains(word);

    public IEnumerable<string> ContainsSubstring(string suffix)
    {
        return hashset.Where(x => x.Contains(suffix));
    }

    public IEnumerable<string> StartWith(string prefix)
    {
        return hashset.Where(x => x.StartsWith(prefix));
    }
}


// ContainsFullWord
// StartsWith
// EndsWith
// ContaintsSubstring