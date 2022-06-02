// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;
using Benchmarks.Collections.Internal;

namespace Benchmarks.Collections
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class DictionarySize
    {
        private List<string> values;

        [Params(5, 10, 20, 100)]
        public int N { get; set; }

        [IterationSetup]
        public void Setup()
        {
            values = new List<string>(N);

            var random = new Random();

            for (var i = 0; i < N; i++)
            {
                values.Add($"{random.Next(1000)}");
            }
        }

        [Benchmark]
        public Dictionary<string, string> Dictionary()
        {
            var result = new Dictionary<string, string>();

            foreach (var value in values)
            {
                result[value] = value;
            }

            return result;
        }

        [Benchmark]
        public Dictionary<string, string> DictionaryPreSize()
        {
            var result = new Dictionary<string, string>(N);

            foreach (var value in values)
            {
                result[value] = value;
            }

            return result;
        }

        [Benchmark]
        public List<KeyValuePair<string, string>> List()
        {
            var result = new List<KeyValuePair<string, string>>();

            foreach (var value in values)
            {
                result.Add(new KeyValuePair<string, string>(value, value));
            }

            return result;
        }

        [Benchmark]
        public List<KeyValuePair<string, string>> PreSize()
        {
            var result = new List<KeyValuePair<string, string>>(N);

            foreach (var value in values)
            {
                result.Add(new KeyValuePair<string, string>(value, value));
            }

            return result;
        }

        [Benchmark]
        public ListDictionary<string, string> ListDictionary()
        {
            var result = new ListDictionary<string, string>();

            foreach (var value in values)
            {
                result[value] = value;
            }

            return result;
        }
    }
}
