﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Benchmarks.Serialization.Internal;

public readonly struct JsonValue : IEquatable<JsonValue>
{
    private static readonly char[] PathSeparators = { '.', '[', ']' };

    public static readonly JsonValue Null;
    public static readonly JsonValue True = new JsonValue(true);
    public static readonly JsonValue False = new JsonValue(false);
    public static readonly JsonValue Zero = new JsonValue(0);

    public readonly object? Value;

    public JsonValueType Type
    {
        get
        {
            switch (Value)
            {
                case null:
                    return JsonValueType.Null;
                case bool:
                    return JsonValueType.Boolean;
                case double:
                    return JsonValueType.Number;
                case string:
                    return JsonValueType.String;
                case JsonArray:
                    return JsonValueType.Array;
                case JsonObject:
                    return JsonValueType.Object;
                default:
                    ThrowInvalidType();
                    return default!;
            }
        }
    }

    public bool AsBoolean
    {
        get
        {
            if (Value is bool typed)
            {
                return typed;
            }

            ThrowInvalidType();
            return default!;
        }
    }

    public double AsNumber
    {
        get
        {
            if (Value is double typed)
            {
                return typed;
            }

            ThrowInvalidType();
            return default!;
        }
    }

    public string AsString
    {
        get
        {
            if (Value is string typed)
            {
                return typed;
            }

            ThrowInvalidType();
            return default!;
        }
    }

    public JsonArray AsArray
    {
        get
        {
            if (Value is JsonArray typed)
            {
                return typed;
            }

            ThrowInvalidType();
            return default!;
        }
    }

    public JsonObject AsObject
    {
        get
        {
            if (Value is JsonObject typed)
            {
                return typed;
            }

            ThrowInvalidType();
            return default!;
        }
    }

    public JsonValue(double value)
    {
        this.Value = value;
    }

    public JsonValue(bool value)
    {
        this.Value = value;
    }

    public JsonValue(string? value)
    {
        this.Value = value;
    }

    public JsonValue(JsonArray? value)
    {
        this.Value = value;
    }

    public JsonValue(JsonObject? value)
    {
        this.Value = value;
    }

    public static JsonValue Create<T>(IReadOnlyDictionary<string, T>? values)
    {
        var source = new JsonObject(values?.Count ?? 0);

        if (values != null)
        {
            foreach (var (key, value) in values)
            {
                source[key] = Create(value);
            }
        }

        return source;
    }

    public static JsonValue Create(object? value)
    {
        if (value == null)
        {
            return default;
        }

        if (value is JsonValue v)
        {
            return v;
        }

        switch (value)
        {
            case Guid typed:
                return Create(typed.ToString());
            case bool typed:
                return Create(typed);
            case float typed:
                return Create(typed);
            case double typed:
                return Create(typed);
            case int typed:
                return Create(typed);
            case long typed:
                return Create(typed);
            case string typed:
                return Create(typed);
            case object[] typed:
                return Array(typed);
            case JsonArray typed:
                return typed;
            case JsonObject typed:
                return typed;
            case IReadOnlyDictionary<string, object?> typed:
                return Create(typed);
        }

        ThrowArgumentException(nameof(value));
        return default!;
    }

    public static JsonObject Object()
    {
        return new JsonObject();
    }

    public static JsonArray Array()
    {
        return new JsonArray();
    }

    public static JsonValue Array<T>(IEnumerable<T> values)
    {
        return new JsonArray(values?.OfType<object?>().Select(Create));
    }

    public static JsonValue Array<T>(params T?[] values)
    {
        return new JsonArray(values?.OfType<object?>().Select(Create));
    }

    public static JsonValue Create(double value)
    {
        return new JsonValue(value);
    }

    public static JsonValue Create(bool value)
    {
        return new JsonValue(value);
    }

    public static JsonValue Create(string? value)
    {
        return new JsonValue(value);
    }

    public static JsonValue Create(JsonArray? array)
    {
        return new JsonValue(array);
    }

    public static JsonValue Create(JsonObject? @object)
    {
        return new JsonValue(@object);
    }

    public static implicit operator JsonValue(bool value)
    {
        return new JsonValue(value);
    }

    public static implicit operator JsonValue(double value)
    {
        return new JsonValue(value);
    }

    public static implicit operator JsonValue(string? value)
    {
        return new JsonValue(value);
    }

    public static implicit operator JsonValue(JsonArray? value)
    {
        return new JsonValue(value);
    }

    public static implicit operator JsonValue(JsonObject? value)
    {
        return new JsonValue(value);
    }

    public static bool operator ==(JsonValue left, JsonValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(JsonValue left, JsonValue right)
    {
        return !(left == right);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is JsonValue typed && Equals(typed);
    }

    public bool Equals(JsonValue other)
    {
        return Equals(other.Value, Value);
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        switch (Value)
        {
            case null:
                return "null";
            case bool b:
                return b ? "true" : "false";
            case double d:
                return d.ToString(CultureInfo.InvariantCulture);
            case string s:
                return s;
            case JsonArray a:
                return a.ToString();
            case JsonObject o:
                return o.ToString();
            default:
                ThrowInvalidType();
                return default!;
        }
    }

    public string ToJsonString()
    {
        switch (Value)
        {
            case null:
                return "null";
            case bool b:
                return b ? "true" : "false";
            case double d:
                return d.ToString(CultureInfo.InvariantCulture);
            case string s:
                return $"\"{s}\"";
            case JsonArray a:
                return a.ToString();
            case JsonObject o:
                return o.ToString();
            default:
                ThrowInvalidType();
                return default!;
        }
    }

    public JsonValue Clone()
    {
        switch (Value)
        {
            case null:
                return this;
            case bool:
                return this;
            case double:
                return this;
            case string:
                return this;
            case JsonArray a:
                {
                    var result = new JsonArray(a.Count);

                    foreach (var item in a)
                    {
                        result.Add(item.Clone());
                    }

                    return result;
                }

            case JsonObject o:
                {
                    var result = new JsonObject(o.Count);

                    foreach (var (key, value) in o)
                    {
                        result.Add(key, value.Clone());
                    }

                    return result;
                }

            default:
                ThrowInvalidType();
                return default!;
        }
    }

    public bool TryGetByPath(string? path, out JsonValue result)
    {
        return TryGetByPath(path?.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries), out result!);
    }

    public bool TryGetByPath(IEnumerable<string>? path, [MaybeNullWhen(false)] out JsonValue result)
    {
        result = this;

        if (path == null)
        {
            return false;
        }

        var hasSegment = false;

        foreach (var pathSegment in path)
        {
            hasSegment = true;

            if (!result.TryGetValue(pathSegment, out var found))
            {
                result = default;
                return false;
            }
            else
            {
                result = found;
            }
        }

        return hasSegment;
    }

    public bool TryGetValue(JsonValueType type, string pathSegment, out JsonValue result)
    {
        result = default!;

        if (TryGetValue(pathSegment, out var temp) && temp.Type == type)
        {
            result = temp;
            return true;
        }

        return false;
    }

    public bool TryGetValue(string pathSegment, out JsonValue result)
    {
        result = default!;

        if (pathSegment == null)
        {
            return false;
        }

        switch (Value)
        {
            case null:
                return false;
            case bool:
                return false;
            case double:
                return false;
            case string:
                return false;
            case JsonArray a:
                return a.TryGetValue(pathSegment, out result);
            case JsonObject o:
                return o.TryGetValue(pathSegment, out result);
            default:
                ThrowInvalidType();
                return default!;
        }
    }

    private static void ThrowInvalidType()
    {
        throw new InvalidOperationException("Invalid type.");
    }

    private static void ThrowArgumentException(string parameterName)
    {
        throw new ArgumentException("Invalid type.", parameterName);
    }
}
