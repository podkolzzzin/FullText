#nullable disable

// See https://aka.ms/new-console-template for more information
//BenchmarkRunner.Run<ContainsFullWordBenchmark>();

// ContainsFullWord
// StartsWith
// EndsWith
// ContaintsSubstring

using System.Text;

class SuffixTree
{
    private class TreeNode
    {
        public char C { get; set; }
        public Dictionary<char, TreeNode> Children { get; set; }
        public HashSet<string> Contains { get; set; }
        public bool IsWord { get; set; }
    }

    private readonly TreeNode root;

    public SuffixTree(string[] words)
    {
        root = new TreeNode();
        foreach (var word in words)
        {
            for (int i = 0; i < word.Length; i++)
            {
                AddWord(i, word);
            }
        }
    }

    public IEnumerable<string> ContainsString(string substr)
    {
        var s = GetNode(substr);
        if (s == null)
            return Enumerable.Empty<string>();

        return GetContains(s).ToHashSet();
    }

    private IEnumerable<string> GetContains(TreeNode node)
    {
        if (node.Contains != null)
            foreach (var item in node.Contains)
                yield return item;

        if (node.Children == null)
            yield break;

        foreach (var child in node.Children)
        {
            foreach (var word in GetContains(child.Value))
                yield return word;
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
            yield break;

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

    private void AddWord(int idx, string word)
    {
        var current = root;
        var suffix = word.AsSpan(idx);
        for (int i = 0; i < suffix.Length; i++)
        {
            if (current.Children != null && current.Children.TryGetValue(suffix[i], out var node))
            {
                current = node;
            }
            else
            {
                if (current.Children == null)
                    current.Children = new Dictionary<char, TreeNode>();
                current.Children.Add(suffix[i], current = new TreeNode() { C = suffix[i] });
            }
        }
        if (idx == 0)
            current.IsWord = true;
        if (current.Contains == null)
            current.Contains = new HashSet<string>();
        current.Contains.Add(word);
    }
}