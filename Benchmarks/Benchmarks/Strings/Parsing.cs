// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using BenchmarkDotNet.Attributes;
using SoftCircuits.HtmlMonkey;
using HtmlPerfKitReader = HtmlPerformanceKit.HtmlReader;

namespace Benchmarks.Strings;

[SimpleJob]
[MemoryDiagnoser]
public class Parsing
{
    private static readonly string Input = File.ReadAllText("Strings\\UGGRoyale.mjml");
    private static readonly MemoryStream Buffer = new MemoryStream(Encoding.UTF8.GetBytes(Input));

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

        var reader = new HtmlKit.HtmlTokenizer(new StringReader(Input));

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
    public object? Read_AngleSharpTokenizer()
    {
        object? latest = null;

        var reader = new HtmlTokenizer(Input);

        AngleSharp.Html.Parser.Tokens.HtmlToken token;
        while ((token = reader.Read()) != null && token.Type != HtmlTokenType.EndOfFile)
        {
            latest = token;
        }

        return latest;
    }

    [Benchmark]
    public object Read_HtmlParser()
    {
        var parser = new HtmlParser();

        return parser.ParseDocument(Input);
    }

    [Benchmark]
    public object? Read_HtmlPerformanceKit()
    {
        object? latest = null;

        var reader = new HtmlPerfKitReader(Buffer);

        while (reader.Read())
        {
            if (reader.AttributeCount > 0)
            {
                for (var i = 0; i < reader.AttributeCount; i++)
                {
                    latest = reader.GetAttribute(i);
                }
            }

            latest = reader.TokenKind;
        }

        return latest;
    }
}

#pragma warning disable MA0048 // File name must match type name
class HtmlTokenizer
#pragma warning restore MA0048 // File name must match type name
{
    private static readonly Func<TextSource, IEntityProvider, IDisposable> HtmlTokenizerFactory;
    private static readonly Func<object, AngleSharp.Html.Parser.Tokens.HtmlToken> GetMethod;
    private readonly IDisposable htmlReader;

    static HtmlTokenizer()
    {
        var tokenizerClassType = typeof(HtmlEntityProvider).Assembly.GetType("AngleSharp.Html.Parser.HtmlTokenizer")!;
        var tokenizerConstructor = tokenizerClassType.GetConstructors()[0];

        HtmlTokenizerFactory = tokenizerConstructor.CreateFactory<TextSource, IEntityProvider, IDisposable>();

        GetMethod = tokenizerClassType.GetMethod("Get")!.CreateILDelegate<AngleSharp.Html.Parser.Tokens.HtmlToken>();
    }

    public HtmlTokenizer(string html)
    {
        htmlReader = HtmlTokenizerFactory(new TextSource(html), HtmlEntityProvider.Resolver);
    }

    public AngleSharp.Html.Parser.Tokens.HtmlToken Read()
    {
        return GetMethod(htmlReader);
    }
}

#pragma warning disable MA0048 // File name must match type name
internal static class ReflectionHelper
#pragma warning restore MA0048 // File name must match type name
{
    public static Func<T1, T2, TReturn> CreateFactory<T1, T2, TReturn>(this ConstructorInfo constructorInfo)
    {
        var parameters = new[]
        {
            Expression.Parameter(typeof(T1)),
            Expression.Parameter(typeof(T2))
        };

        var constructorExpression = Expression.New(constructorInfo, parameters);

        return Expression.Lambda<Func<T1, T2, TReturn>>(constructorExpression, parameters).Compile();
    }

    public static Func<object, TReturn> CreateILDelegate<TReturn>(this MethodInfo methodInfo)
    {
        var parameterTypes = new Type[]
        {
                typeof(object)
        };

        var method = new DynamicMethod(methodInfo.Name, typeof(TReturn), parameterTypes);

        var il = method.GetILGenerator();

        il.Emit(OpCodes.Ldarg_S, 0);
        il.Emit(OpCodes.Call, methodInfo);
        il.Emit(OpCodes.Ret);

        return (Func<object, TReturn>)method.CreateDelegate(typeof(Func<object, TReturn>));
    }
}
