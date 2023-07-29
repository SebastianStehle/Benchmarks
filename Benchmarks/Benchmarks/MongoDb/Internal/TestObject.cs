// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace Benchmarks.MongoDb.Internal;

public sealed class TestObject
{
    public bool Bool { get; set; }

    public int Int32 { get; set; }

    public long Int64 { get; set; }

    public short Int16 { get; set; }

    public uint UInt32 { get; set; }

    public ulong UInt64 { get; set; }

    public ushort UInt16 { get; set; }

    public string String { get; set; }

    public float Float32 { get; set; }

    public double Float64 { get; set; }

    public string[] Strings { get; set; }

    public Uri Uri { get; set; }

    public Guid Guid { get; set; }

    public TimeSpan TimeSpan { get; set; }

    public DateTime DateTime { get; set; }

    public DateTimeOffset DateTimeOffset { get; set; }

    public TestObject Nested { get; set; }

    public TestObject[] NestedArray { get; set; }

    public static TestObject CreateWithValues(bool nested = true)
    {
        var result = new TestObject
        {
            Bool = true,
            DateTimeOffset = DateTime.Today,
            DateTime = DateTime.UtcNow.Date,
            Float32 = 32.5f,
            Float64 = 32.5d,
            Guid = Guid.NewGuid(),
            Int64 = 64,
            Int32 = 32,
            Int16 = 16,
            String = "squidex",
            Strings = new[] { "hello", "squidex " },
            TimeSpan = TimeSpan.FromSeconds(123),
            UInt64 = 164,
            UInt32 = 132,
            UInt16 = 116,
            Uri = new Uri("http://squidex.io")
        };

        if (nested)
        {
            result.Nested = CreateWithValues(false);
            result.NestedArray = Enumerable.Repeat(0, 1).Select(x => CreateWithValues(false)).ToArray();
        }

        return result;
    }
}
