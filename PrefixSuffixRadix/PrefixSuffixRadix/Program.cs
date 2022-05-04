#nullable disable

// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using PreparationToStream.FullText.Database;

BenchmarkRunner.Run<ContainsSubstringBenchmark>();

//var ctx = new FullTextGamesContext();
//var arr = ctx.Words.Select(x => x.Word1).ToArray();
//var prefix = new PrefixTree(arr);
//var result = prefix.StartsWith("like").ToArray();
//Console.WriteLine(result.Length);
//foreach (var item in result)
//    Console.WriteLine(item);

////var suffixTree = new SuffixTree(new[]
////{
////    "hello",
////    "hell",
////    "ololo"
////});

////Console.WriteLine();

//var s = new ContainsSubstringBenchmark();
//Console.WriteLine("Init finished");
//Console.ReadLine();
//Console.WriteLine(s.ToString());