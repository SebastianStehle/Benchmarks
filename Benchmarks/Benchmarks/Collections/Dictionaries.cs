// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;
using Benchmarks.Collections.Internal;

namespace Benchmarks.Collections;

[SimpleJob]
[MemoryDiagnoser]
public class Dictionaries
{
    private Dictionary<string, string> dictionaryNormal;
    private ListDictionary<string, string> dictionaryList;
    private string index1;
    private string index2;

    [Params(5, 10, 20, 100)]
    public int N { get; set; }

    [IterationSetup]
    public void Setup()
    {
        dictionaryNormal = new Dictionary<string, string>();
        dictionaryList = new ListDictionary<string, string>();

        for (var i = 0; i < N; i++)
        {
            var key = $"KEY_{i}";

            dictionaryNormal.Add(key, key);
            dictionaryList.Add(key, key);
        }

        index1 = $"KEY_{(int)(N * 0.5)}";
        index2 = $"KEY_{(int)(N * 0.8)}";
    }

    [Benchmark]
    public void Remove_Dictionary()
    {
        dictionaryNormal.Remove(index1);
        dictionaryNormal.Remove(index2);
    }

    [Benchmark]
    public void Remove_ListDictionary()
    {
        dictionaryList.Remove(index1);
        dictionaryList.Remove(index2);
    }

    [Benchmark]
    public void Add_Dictionary()
    {
        for (var i = 0; i < 10; i++)
        {
            var key = $"KEY_{N + i}";

            dictionaryNormal.Add(key, key);
        }
    }

    [Benchmark]
    public void Add_ListDictionary()
    {
        for (var i = 0; i < 10; i++)
        {
            var key = $"KEY_{N + i}";

            dictionaryList.Add(key, key);
        }
    }

    [Benchmark]
    public void Replace_Dictionary()
    {
        var halfN = N / 2;

        for (var i = 0; i < halfN; i++)
        {
            var key = $"KEY_{halfN + i}";

            dictionaryNormal[key] = key;
        }
    }

    [Benchmark]
    public void Replace_ListDictionary()
    {
        var halfN = N / 2;

        for (var i = 0; i < halfN; i++)
        {
            var key = $"KEY_{halfN + i}";

            dictionaryList[key] = key;
        }
    }

    [Benchmark]
    public void TryGetValue_Dictionary()
    {
        for (var i = 0; i < 10; i++)
        {
            var key = $"KEY_{N + i}";

            dictionaryNormal.TryGetValue(key, out _);
        }
    }

    [Benchmark]
    public void TryGetValue_ListDictionary()
    {
        for (var i = 0; i < 10; i++)
        {
            var key = $"KEY_{N + i}";

            dictionaryList.TryGetValue(key, out _);
        }
    }

    [Benchmark]
    public Dictionary<string, int> SizeEmpty_Dictionary()
    {
        return new Dictionary<string, int>(N);
    }

    [Benchmark]
    public ListDictionary<string, int> SizeEmpty_ListDictionary()
    {
        return new ListDictionary<string, int>(N);
    }

    [Benchmark]
    public Dictionary<string, string> Size_Dictionary()
    {
        var result = new Dictionary<string, string>(N);

        foreach (var (key, value) in dictionaryNormal)
        {
            result.Add(key, value);
        }

        return result;
    }

    [Benchmark]
    public object? Size_ListDictionary()
    {
        var result = new ListDictionary<string, string>(N);

        foreach (var (key, value) in dictionaryNormal)
        {
            result.Add(key, value);
        }

        return result;
    }
}
