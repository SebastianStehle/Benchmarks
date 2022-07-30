// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;

namespace Benchmarks.Reflection
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class TypeChecks
    {
        private List<object?> values;
        private List<JsonValueType> result;

        [IterationSetup]
        public void Setup()
        {
            values = new List<object?>(10000);

            for (var i = 0; i < values.Count; i++)
            {
                var mod = i & 6;

                switch (mod)
                {
                    case 0:
                        values.Add(true);
                        break;
                    case 1:
                        values.Add(string.Empty);
                        break;
                    case 2:
                        values.Add(0.5d);
                        break;
                    case 3:
                        values.Add(new List<object>());
                        break;
                    case 4:
                        values.Add(new Dictionary<string, object>());
                        break;
                    default:
                        values.Add(null);
                        break;
                }
            }

            result = new List<JsonValueType>(values.Count);
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
        public List<JsonValueType> Switch()
        {
            foreach (var value in values)
            {
                result.Add(Check(value));
            }

            return result;

            static JsonValueType Check(object? value)
            {
                switch (value)
                {
                    case null:
                        return JsonValueType.Null;
                    case bool:
                        return JsonValueType.Boolean;
                    case double:
                        return JsonValueType.Number;
                    case string:
                        return JsonValueType.String;
                    case List<object>:
                        return JsonValueType.Array;
                    case Dictionary<string, object>:
                        return JsonValueType.Object;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        [Benchmark]
        public List<JsonValueType> Types()
        {
            foreach (var value in values)
            {
                result.Add(Check(value));
            }

            return result;

            static JsonValueType Check(object? value)
            {
                var type = value?.GetType();

                if (type == null)
                {
                    return JsonValueType.Null;
                }

                if (type == typeof(bool))
                {
                    return JsonValueType.Boolean;
                }

                if (type == typeof(double))
                {
                    return JsonValueType.Number;
                }

                if (type == typeof(string))
                {
                    return JsonValueType.String;
                }

                if (type == typeof(List<string>))
                {
                    return JsonValueType.Array;
                }

                if (type == typeof(Dictionary<string, object>))
                {
                    return JsonValueType.Object;
                }

                throw new InvalidOperationException();
            }
        }
    }
}
