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
    public class Dictionaries
    {
        private Dictionary<string, string> dictionaryNormal;
        private ListDictionary<string, string> dictionaryList;
        private string index1;
        private string index2;

        [Params(5, 10, 20, 100)]
        public int N { get; set; }

        [IterationSetup]
        public void Setup()
        {
            dictionaryNormal = new Dictionary<string, string>();
            dictionaryList = new ListDictionary<string, string>();

            for (var i = 0; i < N; i++)
            {
                var key = $"{i}";

                dictionaryNormal.Add(key, key);
                dictionaryList.Add(key, key);
            }

            index1 = $"{(int)(N * 0.5)}";
            index2 = $"{(int)(N * 0.8)}";
        }

        [Benchmark]
        public void Remove_Dictionary()
        {
            dictionaryNormal.Remove(index1);
            dictionaryNormal.Remove(index2);
        }

        [Benchmark]
        public void Remove_ListDictionary()
        {
            dictionaryList.Remove(index1);
            dictionaryList.Remove(index2);
        }

        [Benchmark]
        public void Add_Dictionary()
        {
            for (var i = 0; i < 10; i++)
            {
                var key = $"{N + i}";

                dictionaryNormal.Add(key, key);
            }
        }

        [Benchmark]
        public void Add_ListDictionary()
        {
            for (var i = 0; i < 10; i++)
            {
                var key = $"{N + i}";

                dictionaryList.Add(key, key);
            }
        }

        [Benchmark]
        public void Replace_Dictionary()
        {
            var halfN = N / 2;

            for (var i = 0; i < halfN; i++)
            {
                var key = $"{halfN + i}";

                dictionaryNormal[key] = key;
            }
        }

        [Benchmark]
        public void Replace_ListDictionary()
        {
            var halfN = N / 2;

            for (var i = 0; i < halfN; i++)
            {
                var key = $"{halfN + i}";

                dictionaryList[key] = key;
            }
        }
    }
}
