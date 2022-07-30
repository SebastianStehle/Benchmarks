// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Globalization;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Numbers
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class ParseNumbers
    {
        [Benchmark]
        public object? Parse_Double()
        {
            return double.Parse("1000", NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public object? Parse_Double_Convert()
        {
            return int.Parse("1000", NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        [Benchmark]
        public object? Parse_Int()
        {
            return int.Parse("1000", NumberStyles.Any, CultureInfo.InvariantCulture);
        }
    }
}
