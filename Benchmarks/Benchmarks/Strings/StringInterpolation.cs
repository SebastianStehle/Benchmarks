// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Strings;

[SimpleJob]
[MemoryDiagnoser]
public class StringInterpolation
{
    private const string Test = "Hello World";
    private static readonly ReadOnlyMemory<char> TestMemory = Test.AsMemory();

    [Benchmark]
    public object? Interpolate()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < 10000; i++)
        {
            sb.Append(CultureInfo.InvariantCulture, $"{i}px");
        }

        return sb.ToString();
    }

    [Benchmark]
    public object? Interpolate_String()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < 10000; i++)
        {
            var value = $"{i}px";

            sb.Append(value);
        }

        return sb.ToString();
    }

    [Benchmark]
    public object? Append()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < 10000; i++)
        {
            sb.Append(i);
            sb.Append("px");
        }

        return sb.ToString();
    }
}
