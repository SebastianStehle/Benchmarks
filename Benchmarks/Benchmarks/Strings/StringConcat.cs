// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Strings;

[SimpleJob]
[MemoryDiagnoser]
public class StringConcat
{
    private const string Test = "Hello World";
    private readonly Guid id1 = Guid.NewGuid();
    private readonly Guid id2 = Guid.NewGuid();
    private readonly Guid id3 = Guid.NewGuid();
    private readonly Guid id4 = Guid.NewGuid();
    private readonly Guid id5 = Guid.NewGuid();
    private readonly Guid id6 = Guid.NewGuid();

    [Benchmark]
    public object? Concat()
    {
        return string.Concat("_", id1, id2, id3, id4, id5, id5, id6);
    }

    [Benchmark]
    public object? Interpolate()
    {
        return $"{id1}_{id2}_{id3}_{id4}_{id5}_{id6}";
    }

    [Benchmark]
    public object? Custom()
    {
        return Custom("_", id1, id2, id3, id4, id5, id6);
    }

    public static string Custom<T1, T2, T3, T4, T5, T6>(string separator, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
    {
        var sb = new StringBuilder();

        sb.Append(t1);
        sb.Append(separator);
        sb.Append(t2);
        sb.Append(separator);
        sb.Append(t3);
        sb.Append(separator);
        sb.Append(t4);
        sb.Append(separator);
        sb.Append(t5);
        sb.Append(separator);
        sb.Append(t6);

        return sb.ToString();
    }
}
