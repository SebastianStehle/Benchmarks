// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Benchmarks.Mapping.Internal;

namespace Benchmarks.Mapping;

[SimpleJob]
[MemoryDiagnoser]
public class Mapper
{
    private readonly TestObject source = new TestObject
    {
        Bool = true,
        Byte = 42,
        Bytes = new byte[] { 42 },
        DateTime = DateTime.Now,
        DateTimeOffset = DateTimeOffset.Now,
        Float32 = 42,
        Float64 = 42,
        Guid = Guid.NewGuid(),
        Int16 = 42,
        Int32 = 42,
        Int64 = 424,
        String = "42",
        Strings = new[] { "42" },
        TimeSpan = DateTime.Now.TimeOfDay,
        UInt16 = 42,
        UInt32 = 42,
        UInt64 = 42,
        Uri = new Uri("https://squidex.io"),
    };

    public class TestObject
    {
        public bool Bool { get; set; }

        public byte Byte { get; set; }

        public byte[] Bytes { get; set; }

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
    }

    [Benchmark]
    public TestObject Map_Manually()
    {
        return new TestObject
        {
            Bool = source.Bool,
            Byte = source.Byte,
            Bytes = source.Bytes,
            DateTime = source.DateTime,
            DateTimeOffset = source.DateTimeOffset,
            Float32 = source.Float32,
            Float64 = source.Float64,
            Guid = source.Guid,
            Int16 = source.Int16,
            Int32 = source.Int32,
            Int64 = source.Int64,
            String = source.String,
            Strings = source.Strings,
            TimeSpan = source.TimeSpan,
            UInt16 = source.UInt16,
            UInt32 = source.UInt32,
            UInt64 = source.UInt64,
            Uri = source.Uri,
        };
    }

    [Benchmark]
    public TestObject Map_SimpleMapper()
    {
        return SimpleMapper.Map(source, new TestObject());
    }
}
