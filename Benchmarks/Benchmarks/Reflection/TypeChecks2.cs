// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;

namespace Benchmarks.Reflection;

[SimpleJob]
[MemoryDiagnoser]
public class TypeChecks2
{
    private readonly JsonValue value = new JsonValue("HELLO");

    public readonly struct JsonValue
    {
        public readonly JsonValueType Type;

        public readonly object Value;

        public JsonValue(string source)
        {
            Value = source;

            Type = JsonValueType.String;
        }
    }

    public enum JsonValueType
    {
        Array,
        Boolean,
        Null,
        Number,
        Object,
        String
    }

    [Benchmark]
    public bool Compare_Type()
    {
        return value.Type == JsonValueType.String;
    }

    [Benchmark]
    public bool Compare_Is()
    {
        return value.Value is string;
    }

    [Benchmark]
    public bool Compare_GetType()
    {
        return value.Value.GetType() == typeof(string);
    }
}
