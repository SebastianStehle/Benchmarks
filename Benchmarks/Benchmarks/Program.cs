// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Running;
using Benchmarks.Mapping;
using Benchmarks.MongoDb;
using Benchmarks.Strings;
using System.Diagnostics;

namespace Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        var x = new Mapper();

        x.Map_Manually();
        x.Map_SimpleMapper();

        Console.WriteLine("Mapping");

        for (var i = 0; i < 5; i++)
        {
            MapManually(x);
        }

        for (var i = 0; i < 5; i++)
        {
            MapSimpleMapper(x);
        }

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }

    private static void MapManually(Mapper x)
    {
        var watch = Stopwatch.StartNew();

        List<Mapper.TestObject> results = new List<Mapper.TestObject>();

        for (var i = 0; i < 1000; i++)
        {
            results.Add(x.Map_Manually());
        }

        watch.Stop();

        Console.WriteLine("Test Manually {0}", watch.Elapsed);
    }

    private static void MapSimpleMapper(Mapper x)
    {
        var watch = Stopwatch.StartNew();

        List<Mapper.TestObject> results = new List<Mapper.TestObject>();

        for (var i = 0; i < 1000; i++)
        {
            results.Add(x.Map_SimpleMapper());
        }

        watch.Stop();

        Console.WriteLine("Test Mapper {0}", watch.Elapsed);
    }
}
