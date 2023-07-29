// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;

namespace Benchmarks.Collections;

[SimpleJob]
[MemoryDiagnoser]
public class DictionaryRemoveLoop
{
    private Dictionary<string, string> source;
    private string index1;
    private string index2;

    [Params(5, 10, 20, 100)]
    public int N { get; set; }

    [IterationSetup]
    public void Setup()
    {
        source = new Dictionary<string, string>();

        for (var i = 0; i < N; i++)
        {
            source.Add($"{i}", $"{i}");
        }

        index1 = $"{(int)(N * 0.5)}";
        index2 = $"{(int)(N * 0.8)}";
    }

    [Benchmark]
    public void Remove_Linq_Keys()
    {
        foreach (var key in source.Keys.Where(x => x == index1 || x == index2).ToList())
        {
            source.Remove(key);
        }
    }

    [Benchmark]
    public void Remove_Linq_Normal()
    {
        foreach (var kvp in source.Where(x => x.Key == index1 || x.Key == index2).ToList())
        {
            source.Remove(kvp.Value);
        }
    }

    [Benchmark]
    public void Remove_Plain()
    {
        while (true)
        {
            var isRemoved = false;

            foreach (var kvp in source)
            {
                if (kvp.Key == index1 || kvp.Key == index2)
                {
                    source.Remove(kvp.Key);
                    isRemoved = true;
                    break;
                }
            }

            if (!isRemoved)
            {
                break;
            }
        }
    }
}
