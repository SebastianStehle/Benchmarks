// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Reflection;
using System.Reflection.Emit;

namespace Benchmarks.Mapping.Internal;

internal static class PropertyAccessor
{
    public delegate TValue Getter<TSource, TValue>(TSource source);

    public delegate void Setter<TSource, TValue>(TSource source, TValue value);

    public static Getter<TSource, TValue> CreateGetter<TSource, TValue>(PropertyInfo propertyInfo)
    {
        if (!propertyInfo.CanRead)
        {
            return x => throw new NotSupportedException();
        }

        var propertyGetMethod = propertyInfo.GetGetMethod()!;

        var getMethod = new DynamicMethod(propertyGetMethod.Name, typeof(TValue), new[] { typeof(TSource) }, false);
        var getGenerator = getMethod.GetILGenerator();
        getGenerator.Emit(OpCodes.Ldarg_0);
        getGenerator.Emit(OpCodes.Call, propertyGetMethod);
        getGenerator.Emit(OpCodes.Ret);

        return getMethod.CreateDelegate<Getter<TSource, TValue>>();
    }

    public static Setter<TSource, TValue> CreateSetter<TSource, TValue>(PropertyInfo propertyInfo)
    {
        if (!propertyInfo.CanWrite)
        {
            return (x, y) => throw new NotSupportedException();
        }

        var propertySetMethod = propertyInfo.GetSetMethod()!;

        var setMethod = new DynamicMethod(propertySetMethod.Name, null, new[] { typeof(TSource), typeof(TValue) }, false);
        var setGenerator = setMethod.GetILGenerator();
        setGenerator.Emit(OpCodes.Ldarg_0);
        setGenerator.Emit(OpCodes.Ldarg_1);
        setGenerator.Emit(OpCodes.Call, propertyInfo.GetSetMethod()!);
        setGenerator.Emit(OpCodes.Ret);

        return setMethod.CreateDelegate<Setter<TSource, TValue>>();
    }
}
