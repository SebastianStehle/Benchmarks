// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;
using System.Globalization;

namespace Benchmarks.Numbers;

[SimpleJob]
[MemoryDiagnoser]
public class ReadChar
{
    [Benchmark]
    public object? Parse1()
    {
        int result = 0;
        int i = 0;

        while (true)
        {
            var c = Read1(i);

            if (c == -1)
            {
                break;
            }

            result += (char)c;
            i++;
        }

        return result;
    }

    [Benchmark]
    public object? Parse2()
    {
        int result = 0;
        int i = 0;

        while (true)
        {
            var (c, end) = Read2(i);

            if (end)
            {
                break;
            }

            result += c;
            i++;
        }

        return result;
    }

    private int Read1(int value)
    {
        if (value == 100)
        {
            return -1;
        }

        return value;
    }

    private (char, bool) Read2(int value)
    {
        if (value == 100)
        {
            return ('\0', false);
        }

        return ((char)value, true);
    }
}
