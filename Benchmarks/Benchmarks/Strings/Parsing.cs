// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Xml;
using System.Xml.Linq;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using BenchmarkDotNet.Attributes;
using HtmlKit;
using SoftCircuits.HtmlMonkey;

namespace Benchmarks.Strings;

[SimpleJob]
[MemoryDiagnoser]
public class Parsing
{
    private static readonly string Input = File.ReadAllText("Strings\\UGGRoyale.mjml");

    [Benchmark]
    public object Read_XmlReader()
    {
        var result = new List<object>();

        var reader = XmlReader.Create(new StringReader(Input));

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                for (var i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                }
            }

            result.Add(reader.NodeType);
        }

        return result;
    }

    [Benchmark]
    public object Read_HtmlReader()
    {
        var result = new List<object>();

        var reader = new HtmlTokenizer(new StringReader(Input));

        while (reader.ReadNextToken(out var token))
        {
            result.Add(token);
        }

        return result;
    }

    [Benchmark]
    public object Read_XmlDocument()
    {
        var doc = new XmlDocument();
        doc.LoadXml(Input);

        return doc;
    }

    [Benchmark]
    public object Read_XDocument()
    {
        return XDocument.Parse(Input);
    }

    [Benchmark]
    public object Read_AngleSharp()
    {
        return HtmlDocument.FromHtml(Input);
    }

    [Benchmark]
    public object Read_AngleSharpTokenizer()
    {
        return HtmlDocument.FromHtml(Input);
    }

    [Benchmark]
    public object Read_HtmlParser()
    {
        var parser = new HtmlParser();

        return parser.ParseDocument(Input);
    }
}
