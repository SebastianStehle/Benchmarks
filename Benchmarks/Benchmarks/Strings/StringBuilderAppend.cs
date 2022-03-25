// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Strings
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class StringBuilderAppend
    {
        private const string Test = "Hello World";
        private static readonly ReadOnlyMemory<char> TestMemory = Test.AsMemory();

        [Benchmark]
        public string Append_String()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < 10000; i++)
            {
                sb.Append(Test);
            }

            return sb.ToString();
        }

        [Benchmark]
        public string Append_Memory()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < 10000; i++)
            {
                sb.Append(TestMemory);
            }

            return sb.ToString();
        }

        [Benchmark]
        public string Append_Span()
        {
            var span = Test.AsSpan();

            var sb = new StringBuilder();

            for (int i = 0; i < 10000; i++)
            {
                sb.Append(span);
            }

            return sb.ToString();
        }
    }
}
