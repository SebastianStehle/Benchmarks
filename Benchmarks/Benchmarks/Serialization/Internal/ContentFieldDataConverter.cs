// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Benchmarks.Serialization.Internal;

public sealed class ContentFieldDataConverter : JsonClassConverter<ContentFieldData>
{
    protected override void WriteValue(JsonWriter writer, ContentFieldData value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        foreach (var (key, jsonValue) in value)
        {
            writer.WritePropertyName(key);

            serializer.Serialize(writer, jsonValue);
        }

        writer.WriteEndObject();
    }

    protected override ContentFieldData ReadValue(JsonReader reader, Type objectType, JsonSerializer serializer)
    {
        var result = new ContentFieldData();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonToken.PropertyName:
                    var propertyName = reader.Value!.ToString()!;

                    if (!reader.Read())
                    {
                        throw new JsonSerializationException("Unexpected end when reading Object.");
                    }

                    var value = serializer.Deserialize<string>(reader)!;

                    if (propertyName == "iv")
                    {
                        propertyName = string.Intern(propertyName);
                    }

                    result[propertyName] = value;
                    break;
                case JsonToken.EndObject:
                    return result;
            }
        }

        throw new JsonSerializationException("Unexpected end when reading Object.");
    }
}
