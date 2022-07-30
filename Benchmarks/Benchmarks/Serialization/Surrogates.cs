// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;
using Benchmarks.Serialization.Internal;
using Newtonsoft.Json;

namespace Benchmarks.Serialization
{
    [SimpleJob]
    [MemoryDiagnoser]
    public sealed class Surrogates
    {
        private readonly ContentFieldData data = new ContentFieldData
        {
            ["1"] = "1",
            ["2"] = "2",
            ["3"] = "3",
            ["4"] = "4",
            ["5"] = "5",
            ["6"] = "6",
            ["7"] = "7",
        };

        private readonly JsonSerializer surrogateSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new SurrogateConverter<ContentFieldData, ContentFieldDataSurrogate>()
            }
        });

        private readonly JsonSerializer customSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new ContentFieldDataConverter()
            }
        });

        private readonly JsonSerializer defaultSerializer = JsonSerializer.CreateDefault();

        [Benchmark]
        public object? Serialize_Surrogate()
        {
            var stream = new MemoryStream();

            using (var textWriter = new StreamWriter(stream))
            {
                surrogateSerializer.Serialize(textWriter, data);
            }

            return stream;
        }

        [Benchmark]
        public object? Serialize_Custom()
        {
            var stream = new MemoryStream();

            using (var textWriter = new StreamWriter(stream))
            {
                customSerializer.Serialize(textWriter, data);
            }

            return stream;
        }

        [Benchmark]
        public object? Serialize_Default()
        {
            var stream = new MemoryStream();

            using (var textWriter = new StreamWriter(stream))
            {
                defaultSerializer.Serialize(textWriter, data);
            }

            return stream;
        }
    }
}
