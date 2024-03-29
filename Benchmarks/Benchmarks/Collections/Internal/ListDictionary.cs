﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Benchmarks.Collections.Internal;

public partial class ListDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    private readonly List<KeyValuePair<TKey, TValue>> entries = new List<KeyValuePair<TKey, TValue>>();
    private readonly IEqualityComparer<TKey> comparer;

    private struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator
    {
        private readonly ListDictionary<TKey, TValue> dictionary;
        private int index = -1;
        private KeyValuePair<TKey, TValue> value = default!;

        readonly KeyValuePair<TKey, TValue> IEnumerator<KeyValuePair<TKey, TValue>>.Current
        {
            get => value!;
        }

        readonly object IEnumerator.Current
        {
            get => value!;
        }

        public Enumerator(ListDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        public readonly void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (index >= dictionary.entries.Count - 1)
            {
                return false;
            }

            index++;

            value = dictionary.entries[index];
            return true;
        }

        public void Reset()
        {
            index = -1;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            if (!TryGetValue(key, out var result))
            {
                throw new KeyNotFoundException();
            }

            return result;
        }
        set
        {
            var index = -1;

            for (var i = 0; i < entries.Count; i++)
            {
                if (Equals(entries[i].Key, key))
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                entries[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                entries.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }
    }

    public ICollection<TKey> Keys
    {
        get => new KeyCollection(this);
    }

    public ICollection<TValue> Values
    {
        get => new ValueCollection(this);
    }

    public int Count
    {
        get => entries.Count;
    }

    public int Capacity
    {
        get => entries.Capacity;
    }

    public bool IsReadOnly
    {
        get => false;
    }

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
    {
        get => new KeyCollection(this);
    }

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
    {
        get => new ValueCollection(this);
    }

    public ListDictionary()
        : this(1, null)
    {
    }

    public ListDictionary(ListDictionary<TKey, TValue> source, IEqualityComparer<TKey>? comparer = null)
    {
        entries = source.entries.ToList();

        this.comparer = comparer ?? EqualityComparer<TKey>.Default;
    }

    public ListDictionary(int capacity, IEqualityComparer<TKey>? comparer = null)
    {
        entries = new List<KeyValuePair<TKey, TValue>>(capacity);

        this.comparer = comparer ?? EqualityComparer<TKey>.Default;
    }

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {
            throw new ArgumentException("Key already exists.", nameof(key));
        }

        entries.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        entries.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        foreach (var entry in entries)
        {
            if (Equals(entry.Key, item.Key) && Equals(entry.Value, item.Value))
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        foreach (var entry in entries)
        {
            if (Equals(entry.Key, key))
            {
                return true;
            }
        }

        return false;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        foreach (var entry in entries)
        {
            if (Equals(entry.Key, key))
            {
                value = entry.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            if (Equals(entry.Key, key))
            {
                entries.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];

            if (Equals(entry.Key, item.Key) && Equals(entry.Value, item.Value))
            {
                entries.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public void TrimExcess()
    {
        entries.TrimExcess();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        entries.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    private bool Equals(TKey lhs, TKey rhs)
    {
        if (comparer.GetHashCode(lhs) != comparer.GetHashCode(rhs))
        {
            return false;
        }

        return comparer.Equals(lhs, rhs);
    }
}
