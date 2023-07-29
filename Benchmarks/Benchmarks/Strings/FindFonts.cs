// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using BenchmarkDotNet.Attributes;

namespace Benchmarks.Strings;

[SimpleJob]
[MemoryDiagnoser]
public class FindFonts
{
    private static readonly char[] SplitChars = { ' ', ',' };

    private static readonly Dictionary<string, string> Fonts = new Dictionary<string, string>
    {
        ["Open Sans"] = "https://fonts.googleapis.com/css?family=Open+Sans:300,400,500,700",
        ["Droid Sans"] = "https://fonts.googleapis.com/css?family=Droid+Sans:300,400,500,700",
        ["Lato"] = "https://fonts.googleapis.com/css?family=Lato:300,400,500,700",
        ["Roboto"] = "https://fonts.googleapis.com/css?family=Roboto:300,400,500,700",
        ["Ubuntu"] = "https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700"
    };

    [Params("", "Ubuntu", "Ubuntu,Roboto", "Ubuntu, Roboto")]
    public string Input { get; set; }

    [Benchmark]
    public object? Trim()
    {
        var result = new HashSet<string>();

        var fonts = Input.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

        foreach (var font in fonts)
        {
            if (Fonts.TryGetValue(font.Trim(), out var defaultFont))
            {
                result.Add(defaultFont);
            }
        }

        return result;
    }

    [Benchmark]
    public object? Contains()
    {
        var result = new HashSet<string>();

        foreach (var (key, value) in Fonts)
        {
            if (Input.Contains(key, StringComparison.Ordinal))
            {
                result.Add(value);
            }
        }

        return result;
    }
}
