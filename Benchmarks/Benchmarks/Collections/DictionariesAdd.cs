// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;

#pragma warning disable CA1822 // Mark members as static

namespace Benchmarks.Collections
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class DictionariesAdd
    {
        [Benchmark]
        public object? Add()
        {
            var result = new Dictionary<int, int>();

            for (var i = 0; i < 1000; i++)
            {
                result.Add(i, i);
            }

            return result;
        }

        [Benchmark]
        public object? Set()
        {
            var result = new Dictionary<int, int>();

            for (var i = 0; i < 1000; i++)
            {
                result[i] = i;
            }

            return result;
        }
    }
}
