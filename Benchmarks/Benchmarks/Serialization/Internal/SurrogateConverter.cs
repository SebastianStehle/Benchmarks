// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;

namespace Benchmarks.Serialization.Internal
{
#pragma warning disable MA0048 // File name must match type name
    public interface ISurrogate<T>
#pragma warning restore MA0048 // File name must match type name
    {
        void FromSource(T source);

        T ToSource();
    }

    public sealed class SurrogateConverter<T, TSurrogate> : JsonClassConverter<T> where T : class where TSurrogate : ISurrogate<T>, new()
    {
        protected override T? ReadValue(JsonReader reader, Type objectType, JsonSerializer serializer)
        {
            var surrogate = serializer.Deserialize<TSurrogate>(reader);

            return surrogate?.ToSource();
        }

        protected override void WriteValue(JsonWriter writer, T value, JsonSerializer serializer)
        {
            var surrogate = new TSurrogate();

            surrogate.FromSource(value);

            serializer.Serialize(writer, surrogate);
        }
    }
}
