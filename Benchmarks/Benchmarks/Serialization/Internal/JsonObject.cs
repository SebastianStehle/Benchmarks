// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Benchmarks.Serialization.Internal;

public class JsonObject : Dictionary<string, JsonValue>
{
    public JsonObject()
    {
    }

    public JsonObject(int capacity)
        : base(capacity)
    {
    }

    public JsonObject(JsonObject source)
        : base(source)
    {
    }

    public override string ToString()
    {
        return $"{{{string.Join(", ", this.Select(x => $"\"{x.Key}\":{x.Value.ToJsonString()}"))}}}";
    }

    public new JsonObject Add(string key, JsonValue value)
    {
        this[key] = value;

        return this;
    }
}
