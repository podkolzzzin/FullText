// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

//var list = StringExtractor.ArticlesSet().Take(10_000).ToArray();

//var searcher = new SimpleSearcher();
//searcher.Search("Monday", list);
//var s = new SearchBenchmark();
//s.SimpleSearch();


//BenchmarkRunner.Run<SearchBenchmark>();
BenchmarkRunner.Run<BuildIndexBenchmark>();

[MemoryDiagnoser]
[WarmupCount(1)]
[IterationCount(5)]
[InvocationCount(5)]
public class BuildIndexBenchmark
{
    private readonly string[] _dataset;

    public BuildIndexBenchmark()
    {
        _dataset = StringExtractor.ArticlesSet().Take(1_000).ToArray();
    }

    [Benchmark(Baseline = true)]
    public void ReadDataset() => StringExtractor.ArticlesSet().Take(1_000).ToArray();

    [Benchmark]
    public void SimpleIndex()
    {
        FullTextGames.SimpleFullTextIndex.FullTextIndex index = new();
        foreach (var text in _dataset)
            index.AddStringToIndex(text);
    }

    [Benchmark]
    public void PositionedIndex()
    {
        FullTextGames.FullTextWithPositions.FullTextIndex index = new();
        foreach (var text in _dataset)
            index.AddStringToIndex(text);
    }

    [Benchmark]
    public void FragmentIndex()
    {
        PreparationToStream.FullText.DocumentBasedShortDocumentsFullTextIndex.FullTextIndex index = new();
        foreach (var text in _dataset)
            index.AddStringToIndex(text);
    }
}

[MemoryDiagnoser]
[WarmupCount(1)]
[IterationCount(5)]
public class SearchBenchmark
{
    private readonly string[] _dataset;
    private readonly FullTextGames.SimpleFullTextIndex.FullTextIndex _index;
    private readonly FullTextGames.FullTextWithPositions.FullTextIndex _withPositions;
    private readonly PreparationToStream.FullText.DocumentBasedShortDocumentsFullTextIndex.FullTextIndex _fragments;

    public SearchBenchmark()
    {
        _dataset = StringExtractor.ArticlesSet().Take(3_000).ToArray();
        _index = new ();
        _withPositions = new ();
        _fragments = new ();
        foreach (var item in _dataset)
        {
            _index.AddStringToIndex(item);
            _withPositions.AddStringToIndex(item);
            _fragments.AddStringToIndex(item);
        }
    }

    [Params("assessment", "monday", "where")]
    public string Query { get; set; }

    [Benchmark(Baseline = true)]
    public void SimpleSearch()
    {
        new SimpleSearcher().Search(Query, _dataset).ToArray();
    }

    [Benchmark]
    public void FragmentsSearch()
    {
        _fragments.SearchTest(Query).ToArray();
    }

    [Benchmark]
    public void FullTextIndexedSearch()
    {
        _index.SearchTest(Query).ToArray();
    }


    [Benchmark]
    public void WithPositionsIndexedSearch()
    {
        _withPositions.SearchTest(Query).ToArray();
    }
}

class SimpleSearcher
{
    public IEnumerable<string> Search(string word, string item, int start = 0, int end = -1)
    {
        int pos = start;
        while (true)
        {
            pos = end == -1 ? item.IndexOf(word, pos)
                : item.IndexOf(word, pos, end - pos);
            if (pos >= 0)
            {
                yield return PrettyMatch(item, pos);
            }
            else
            {
                break;
            }
            pos++;
        }
    }

    public IEnumerable<string> Search(string word, IEnumerable<string> stringsToSearch)
    {
        foreach(var item in stringsToSearch)
        {
            foreach (var match in Search(word, item))
                yield return match;
        }
    }

    public string PrettyMatch(string text, int pos)
    {
        var start = Math.Max(0, pos - 50);
        int end = Math.Min(start + 100, text.Length - 1);
        return (start == 0 ? "" : "...")
            + text.Substring(start, end - start)
            + (end == text.Length - 1 ? "" : "...");
    }
}


class StringExtractor
{
    public static IEnumerable<string> ArticlesSet()
    {
        return ReadArticleSet("articles1.csv")
            .Concat(ReadArticleSet("articles2.csv"))
            .Concat(ReadArticleSet("articles3.csv"));
    }

    private static IEnumerable<string> ReadArticleSet(string fileName)
    {
        using var reader = new CsvHelper.CsvReader(
            File.OpenText(Path.Combine(@"C:\datasets\archive", fileName)),
            System.Globalization.CultureInfo.InvariantCulture);
        reader.Read();
        reader.ReadHeader();
        while (reader.Read())
        {
            var content = reader["content"];
            yield return content;
        }
    }
}