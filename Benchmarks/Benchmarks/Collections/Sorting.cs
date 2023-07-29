// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Collections;

[SimpleJob]
[MemoryDiagnoser]
public class Sorting
{
    public class Entity
    {
        public Guid Id { get; set; }
    }

    private readonly List<Entity> entities = new List<Entity>();
    private readonly List<Guid> entityIds = new List<Guid>();

    [Params(1, 2, 3, 4, 5, 10, 20, 50)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        for (var i = 0; i < N; i++)
        {
            var id = Guid.NewGuid();

            entities.Add(new Entity { Id = id });
            entityIds.Add(id);
        }

        entityIds.Reverse();
    }

    [Benchmark]
    public List<Entity> SortLookup()
    {
        return Lookup(entities, x => x.Id, entityIds);
    }

    [Benchmark]
    public List<Entity> SortDictionary()
    {
        return Dictionary(entities, x => x.Id, entityIds);
    }

    [Benchmark]
    public List<Entity> SortFind()
    {
        return Find(entities, x => x.Id, entityIds);
    }

    public static List<T> Lookup<T, TKey>(List<T> input, Func<T, TKey> idProvider, IReadOnlyList<TKey> ids) where T : class
    {
        var result = new List<T>(ids.Count);

        var dictionary = input.ToLookup(idProvider);

        foreach (var id in ids)
        {
            var item = dictionary[id].FirstOrDefault();

            if (item != null)
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static List<T> Dictionary<T, TKey>(List<T> input, Func<T, TKey> idProvider, IReadOnlyList<TKey> ids) where T : class where TKey : notnull
    {
        var result = new List<T>(ids.Count);

        var dictionary = new Dictionary<TKey, T>(input.Count);

        foreach (var item in input)
        {
            dictionary[idProvider(item)] = item;
        }

        foreach (var id in ids)
        {
            if (dictionary.TryGetValue(id, out var item))
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static List<T> Find<T, TKey>(List<T> input, Func<T, TKey> idProvider, IReadOnlyList<TKey> ids) where T : class
    {
        var result = new List<T>(ids.Count);

        foreach (var id in ids)
        {
            T? item = null;

            foreach (var candidate in input)
            {
                if (Equals(id, idProvider(candidate)))
                {
                    item = candidate;
                    break;
                }
            }

            if (item != null)
            {
                result.Add(item);
            }
        }

        return result;
    }
}
