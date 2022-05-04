
#nullable disable

public interface ISercher
{
    bool ContainsFullWord(string word);

    IEnumerable<string> StartWith(string prefix);

    IEnumerable<string> ContainsSubstring(string suffix);
}

// ContainsFullWord
// StartsWith
// EndsWith
// ContaintsSubstring