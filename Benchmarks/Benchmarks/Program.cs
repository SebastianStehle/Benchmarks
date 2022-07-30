// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Running;
using Benchmarks.MongoDb;

namespace Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var x = new MongoDb_Deserialize();
            x.Json_System();
            x.Json_Newtonsoft();
            x.Bson();
            x.Bson_over_JsonDocument();
            x.Bson_over_JsonBinary();
            x.Bson_over_JsonString();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
