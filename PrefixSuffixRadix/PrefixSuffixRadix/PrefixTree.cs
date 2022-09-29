#nullable disable

// See https://aka.ms/new-console-template for more information
//BenchmarkRunner.Run<ContainsFullWordBenchmark>();

// ContainsFullWord
// StartsWith
// EndsWith
// ContaintsSubstring

using System.Text;

class PrefixTree
{
    private class TreeNode
    {
        public char C { get; set; }
        public Dictionary<char, TreeNode> Children { get; set; }
        public bool IsWord { get; set; }
    }

    private readonly TreeNode root;

    public PrefixTree(string[] words)
    {
        root = new TreeNode();
        foreach (var word in words)
        {
            AddWord(word);
        }
    }

    public IEnumerable<string> StartsWith(string prefix)
    {
        var node = GetNode(prefix);
        if (node == null)
            return Enumerable.Empty<string>();

        return GetWords(new StringBuilder().Append(prefix.AsSpan(0, prefix.Length - 1)), node);
    }

    private IEnumerable<string> GetWords(StringBuilder builder, TreeNode node)
    {
        builder.Append(node.C);
        if (node.IsWord)
            yield return builder.ToString();

        if (node.Children == null)
        {
            builder.Remove(builder.Length - 1, 1);
            yield break;
        }

        foreach(var childNode in node.Children.Values)
        {
            foreach (var word in GetWords(builder, childNode))
                yield return word;
        }

        builder.Remove(builder.Length - 1, 1);
    }

    private TreeNode GetNode(string prefix)
    {
        var current = root;
        for (int i = 0; i < prefix.Length; i++)
        {
            if (current.Children != null && current.Children.TryGetValue(prefix[i], out var node))
            {
                current = node;
            }
            else
            {
                return null;
            }
        }
        return current;
    }

    public bool ContainsFullWord(string word)
    {
        return GetNode(word)?.IsWord == true;
    }

    private void AddWord(string word)
    {
        var current = root;
        for (int i = 0; i < word.Length; i++)
        {
            if (current.Children != null && current.Children.TryGetValue(word[i], out var node))
            {
                current = node;
            }
            else
            {
                if (current.Children == null)
                    current.Children = new Dictionary<char, TreeNode>();
                current.Children.Add(word[i], current = new TreeNode() { C = word[i] });
            }
        }
        current.IsWord = true;
    }
}
