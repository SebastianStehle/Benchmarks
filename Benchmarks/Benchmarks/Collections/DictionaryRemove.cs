// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;

namespace Benchmarks.Collections
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class DictionaryRemove
    {
        private Dictionary<string, string> source;

        [IterationSetup]
        public void Setuo()
        {
            source = new Dictionary<string, string>();

            for (var i = 0; i < 20; i++)
            {
                source.Add($"{i}", $"{i}");
            }
        }

        [Benchmark]
        public void Remove_Linq_Keys()
        {
            foreach (var key in source.Keys.Where(x => x == "15" || x == "20").ToList())
            {
                source.Remove(key);
            }
        }

        [Benchmark]
        public void Remove_Linq_Normal()
        {
            foreach (var kvp in source.Where(x => x.Key == "15" || x.Key == "20").ToList())
            {
                source.Remove(kvp.Value);
            }
        }

        [Benchmark]
        public void Remove_Plain()
        {
            while (true)
            {
                var isRemoved = false;

                foreach (var kvp in source)
                {
                    if (kvp.Key == "15" || kvp.Key == "20")
                    {
                        source.Remove(kvp.Key);
                        isRemoved = true;
                        break;
                    }
                }

                if (!isRemoved)
                {
                    break;
                }
            }
        }
    }
}
