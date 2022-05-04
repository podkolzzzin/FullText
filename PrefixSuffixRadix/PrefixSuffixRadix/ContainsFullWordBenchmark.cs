#nullable disable

// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using PreparationToStream.FullText.Database;


[IterationCount(8)]
[WarmupCount(1)]
public class StartsWithBenchmark : ContainsFullWordBenchmark
{
    protected override void BenchmarkAction(string word, ISercher searcher)
    {
        foreach (var item in searcher.StartWith(word))
            ;
    }
}


[IterationCount(8)]
[WarmupCount(1)]
public class ContainsSubstringBenchmark : BaseBenchmark
{
    private string[] suffixes = new[]
    {
"orn",
"rse",
"ath",
"yan",
"rge",
"rmi"
    };

    public ContainsSubstringBenchmark()
    {
        _wordsToSeach = suffixes;
    }

    protected override void BenchmarkAction(string word, ISercher searcher)
    {
        foreach (var item in searcher.ContainsSubstring(word))
            ;
    }


    [Benchmark]
    public void Simple() => DoAction(searcher);

    [Benchmark(Baseline = true)]
    public void SuffixTree() => DoAction(suffixTree);
}

[IterationCount(8)]
[WarmupCount(1)]
//[EvaluateOverhead(false)]
public class ContainsFullWordBenchmark : BaseBenchmark
{
    protected override void BenchmarkAction(string word, ISercher searcher)
    {
        searcher.ContainsFullWord(word);
    }

    [Benchmark]
    public void Simple() => DoAction(searcher);

    [Benchmark(Baseline = true)]
    public void Hash() => DoAction(hash);

    [Benchmark]
    public void PrefixTree() => DoAction(tree);

    [Benchmark]
    public void Sorted() => DoAction(sorted);
}

// ContainsFullWord
// StartsWith
// EndsWith
// ContaintsSubstring