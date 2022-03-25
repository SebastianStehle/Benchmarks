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
            ["iv"] = "1",
            ["iv"] = "2",
            ["iv"] = "3",
            ["iv"] = "4",
            ["iv"] = "5",
            ["iv"] = "6",
            ["iv"] = "7",
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
        public Stream Serialize_Surrogate()
        {
            var stream = new MemoryStream();

            using (var textWriter = new StreamWriter(stream))
            {
                surrogateSerializer.Serialize(textWriter, data);
            }

            return stream;
        }

        [Benchmark]
        public Stream Serialize_Custom()
        {
            var stream = new MemoryStream();

            using (var textWriter = new StreamWriter(stream))
            {
                customSerializer.Serialize(textWriter, data);
            }

            return stream;
        }

        [Benchmark]
        public Stream Serialize_Default()
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
