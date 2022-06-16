// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Benchmarks.Serialization.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Benchmarks.Serialization
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class LargeJson
    {
        private readonly string json = File.ReadAllText("Serialization/Internal/LargeJson.json");
        private readonly JsonValueConverter converter = new JsonValueConverter();

        [Benchmark]
        public Dictionary<string, Dictionary<string, JsonValue>> Newtonsoft_Dynamic2()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, JsonValue>>>(json, converter)!;
        }

        [Benchmark]
        public AuditReportClassification SystemTextJson()
        {
            return System.Text.Json.JsonSerializer.Deserialize<AuditReportClassification>(json)!;
        }

        [Benchmark]
        public JsonDocument SystemTextJson_Dynamic()
        {
            return JsonDocument.Parse(json);
        }

        [Benchmark]
        public AuditReportClassification Newtonsoft()
        {
            return JsonConvert.DeserializeObject<AuditReportClassification>(json)!;
        }

        [Benchmark]
        public JObject Newtonsoft_Dynamic()
        {
            return JObject.Parse(json)!;
        }
    }
}
