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
public class ForLoops
{
    private List<int> source;
    private List<int> result;
    private IReadOnlyList<int> sourceInterface;

    [IterationSetup]
    public void Setup()
    {
        source = new List<int>();

        for (var i = 0; i < 100; i++)
        {
            source.Add(i);
        }

        sourceInterface = source;

        result = new List<int>(source.Count);
    }

    [Benchmark]
    public object? For()
    {
        for (var i = 0; i < source.Count; i++)
        {
            result.Add(source[i]);
        }

        return result;
    }

    [Benchmark]
    public object? For_interface()
    {
        for (var i = 0; i < sourceInterface.Count; i++)
        {
            result.Add(source[i]);
        }

        return result;
    }

    [Benchmark]
    public object? Foreach()
    {
        foreach (var item in source)
        {
            result.Add(item);
        }

        return result;
    }

    [Benchmark]
    public object? Foreach_Interface()
    {
        foreach (var item in sourceInterface)
        {
            result.Add(item);
        }

        return result;
    }
}
