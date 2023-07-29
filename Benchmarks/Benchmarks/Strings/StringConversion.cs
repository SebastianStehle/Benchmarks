// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json.Linq;

namespace Benchmarks.Strings;

[SimpleJob]
[MemoryDiagnoser]
public class StringConversion
{
    private string input;

    [Params(5, 10, 20, 100)]
    public int N { get; set; }

    [IterationSetup]
    public void Setup()
    {
        input = new string('a', N);
    }

    [Benchmark]
    public byte[] Normal()
    {
        return Encoding.Unicode.GetBytes(input);
    }

    [Benchmark]
    public int StackAlloc()
    {
        var length = Encoding.Unicode.GetByteCount(input);

        Span<byte> buffer = stackalloc byte[length];

        return Encoding.Unicode.GetBytes(input.AsSpan(), buffer);
    }

    [Benchmark]
    [SkipLocalsInit]
    public int StackAllocNoInit()
    {
        var length = Encoding.Unicode.GetByteCount(input);

        Span<byte> buffer = stackalloc byte[length];

        return Encoding.Unicode.GetBytes(input.AsSpan(), buffer);
    }

    [Benchmark]
    public int ArrayPool()
    {
        var length = Encoding.Unicode.GetByteCount(input);

        var buffer = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            return Encoding.Default.GetBytes(input, buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
