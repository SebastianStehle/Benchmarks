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
public class ByteComparison
{
    private readonly byte[] lhs = new byte[500];
    private readonly byte[] rhs = new byte[500];

    public ByteComparison()
    {
        var random = new Random();

        random.NextBytes(lhs);
        random.NextBytes(rhs);
    }

    [Benchmark]
    public object? SequenceEqual()
    {
        return lhs.SequenceEqual(rhs);
    }

    [Benchmark]
    public object? FullEqual()
    {
        var equalCount = 0;

        for (var i = 0; i < lhs.Length; i++)
        {
            equalCount += lhs[i] == rhs[i] ? 1 : 0;
        }

        return equalCount == lhs.Length;
    }
}
