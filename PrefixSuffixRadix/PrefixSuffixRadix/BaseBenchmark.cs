#nullable disable

// See https://aka.ms/new-console-template for more information
using PreparationToStream.FullText.Database;

public abstract class BaseBenchmark
{
    protected readonly SimpleSearcher searcher;
    protected readonly HashsetSearcher hash;
    protected readonly SortedSearcher sorted;
    protected readonly TreeSearcher tree;
    protected readonly SuffixTreeSearcher suffixTree;


    protected string[] _wordsToSeach = new[]
    {
        "adolescence",
        "sublimely",
        "mushy",
        "muckraker",
        "guardsmen",
        "vey",
        "tugboats",
        "cohn",
        "sichuan",
        "princeville",
        "blabla",
        "ololo",
        "alala",
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
    };

    // pg_dump -C -c -U postgres -f ./data.sql FullTextGames
    public BaseBenchmark()
    {
        var ctx = new FullTextGamesContext();
        var arr = ctx.Words.Select(x => x.Word1).ToArray();
        searcher = new(arr);
        hash = new(arr);
        tree = new(arr);
        sorted = new(arr);
        suffixTree = new(arr);
    }

    protected abstract void BenchmarkAction(string word, ISercher searcher);

    protected void DoAction(ISercher searcher)
    {
        foreach (var item in _wordsToSeach)
            BenchmarkAction(item, searcher);
    }
}

// ContainsFullWord
// StartsWith
// EndsWith
// ContaintsSubstring