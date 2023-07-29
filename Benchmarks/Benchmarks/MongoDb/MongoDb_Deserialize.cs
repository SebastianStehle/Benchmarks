// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text;
using BenchmarkDotNet.Attributes;
using Benchmarks.MongoDb.Internal;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Benchmarks.MongoDb;

[SimpleJob]
[MemoryDiagnoser]
public class MongoDb_Deserialize
{
    private readonly TestObject source = TestObject.CreateWithValues(true);
    private readonly JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();
    private readonly MemoryStream sourceJson = new MemoryStream();
    private readonly MemoryStream sourceBson = new MemoryStream();
    private readonly MemoryStream sourceBsonJsonDocument = new MemoryStream();
    private readonly MemoryStream sourceBsonJsonBinary = new MemoryStream();
    private readonly MemoryStream sourceBsonJsonString = new MemoryStream();

    private class WrapperPlain
    {
        public TestObject Source { get; set; }
    }

    private class WrapperJsonDocument
    {
        [BsonJson]
        [BsonRepresentation(BsonType.Document)]
        public TestObject Source { get; set; }
    }

    private class WrapperJsonBinary
    {
        [BsonJson]
        [BsonRepresentation(BsonType.Binary)]
        public TestObject Source { get; set; }
    }

    private class WrapperJsonString
    {
        [BsonJson]
        [BsonRepresentation(BsonType.String)]
        public TestObject Source { get; set; }
    }

    static MongoDb_Deserialize()
    {
        BsonJsonConvention.Register();
    }

    public MongoDb_Deserialize()
    {
        System.Text.Json.JsonSerializer.Serialize(sourceJson, source);

        using (var writer = new BsonBinaryWriter(sourceBson))
        {
            BsonSerializer.Serialize(writer, typeof(WrapperPlain), new WrapperPlain { Source = source });
        }

        using (var writer = new BsonBinaryWriter(sourceBsonJsonDocument))
        {
            BsonSerializer.Serialize(writer, typeof(WrapperJsonDocument), new WrapperJsonDocument { Source = source });
        }

        using (var writer = new BsonBinaryWriter(sourceBsonJsonBinary))
        {
            BsonSerializer.Serialize(writer, typeof(WrapperJsonBinary), new WrapperJsonBinary { Source = source });
        }

        using (var writer = new BsonBinaryWriter(sourceBsonJsonString))
        {
            BsonSerializer.Serialize(writer, typeof(WrapperJsonString), new WrapperJsonString { Source = source });
        }
    }

    [Benchmark]
    public object? Json_Newtonsoft()
    {
        sourceJson.Position = 0;

        using var readerText = new StreamReader(sourceJson, leaveOpen: true);
        using var readerJson = new JsonTextReader(readerText);

        return jsonSerializer.Deserialize<TestObject>(readerJson);
    }

    [Benchmark]
    public object? Json_System()
    {
        sourceJson.Position = 0;

        return System.Text.Json.JsonSerializer.Deserialize<TestObject>(sourceJson);
    }

    [Benchmark]
    public object? Bson()
    {
        sourceBson.Position = 0;

        using var reader = new BsonBinaryReader(sourceBson);

        return BsonSerializer.Deserialize<WrapperPlain>(reader);
    }

    [Benchmark]
    public object? Bson_over_JsonDocument()
    {
        sourceBsonJsonDocument.Position = 0;

        using var reader = new BsonBinaryReader(sourceBsonJsonDocument);

        return BsonSerializer.Deserialize<WrapperJsonDocument>(reader);
    }

    [Benchmark]
    public object? Bson_over_JsonBinary()
    {
        sourceBsonJsonBinary.Position = 0;

        using var reader = new BsonBinaryReader(sourceBsonJsonBinary);

        return BsonSerializer.Deserialize<WrapperJsonBinary>(reader);
    }

    [Benchmark]
    public object? Bson_over_JsonString()
    {
        sourceBsonJsonString.Position = 0;

        using var reader = new BsonBinaryReader(sourceBsonJsonString);

        return BsonSerializer.Deserialize<WrapperJsonString>(reader);
    }
}
