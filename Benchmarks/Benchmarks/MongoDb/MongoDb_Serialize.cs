// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

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
public class MongoDb_Serialize
{
    private readonly TestObject source = TestObject.CreateWithValues(true);
    private readonly JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();

    static MongoDb_Serialize()
    {
        BsonJsonConvention.Register();
    }

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

    [Benchmark]
    public object? Json_Newtonsoft()
    {
        var stream = new MemoryStream();

        using var writerText = new StreamWriter(stream);
        using var writerJson = new JsonTextWriter(writerText);

        jsonSerializer.Serialize(writerJson, source);

        return stream;
    }

    [Benchmark]
    public object? Json_System()
    {
        var stream = new MemoryStream();

        System.Text.Json.JsonSerializer.Serialize(stream, source);

        return stream;
    }

    [Benchmark]
    public object? Bson()
    {
        var stream = new MemoryStream();

        using var writer = new BsonBinaryWriter(stream);

        BsonSerializer.Serialize(writer, typeof(WrapperPlain), new WrapperPlain { Source = source });

        return stream;
    }

    [Benchmark]
    public object? Bson_over_JsonDocument()
    {
        var stream = new MemoryStream();

        using var writer = new BsonBinaryWriter(stream);

        BsonSerializer.Serialize(writer, typeof(WrapperJsonDocument), new WrapperJsonDocument { Source = source });

        return stream;
    }

    [Benchmark]
    public object? Bson_over_JsonBinary()
    {
        var stream = new MemoryStream();

        using var writer = new BsonBinaryWriter(stream);

        BsonSerializer.Serialize(writer, typeof(WrapperJsonBinary), new WrapperJsonBinary { Source = source });

        return stream;
    }

    [Benchmark]
    public object? Bson_over_JsonString()
    {
        var stream = new MemoryStream();

        using var writer = new BsonBinaryWriter(stream);

        BsonSerializer.Serialize(writer, typeof(WrapperJsonString), new WrapperJsonString { Source = source });

        return stream;
    }
}
