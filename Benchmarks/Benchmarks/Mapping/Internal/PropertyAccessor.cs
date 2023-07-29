// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Reflection;
using System.Reflection.Emit;

#pragma warning disable MA0048 // File name must match type name

namespace Benchmarks.Mapping.Internal;

public sealed class PropertyAccessor<TSource>
{
    private readonly IPropertyAccessor<TSource> internalAccessor;

    public PropertyAccessor(PropertyInfo propertyInfo)
    {
        var type = typeof(PropertyWrapper<,>).MakeGenericType(propertyInfo.DeclaringType!, propertyInfo.PropertyType);

        internalAccessor = (IPropertyAccessor<TSource>)Activator.CreateInstance(type, propertyInfo)!;
    }

    public object? Get(TSource target)
    {
        return internalAccessor.Get(target);
    }

    public void Set(TSource target, object? value)
    {
        internalAccessor.Set(target, value);
    }
}

public sealed class PropertyAccessor<TSource, TValue>
{
    private readonly IPropertyAccessor<TSource, TValue> internalAccessor;

    public PropertyAccessor(PropertyInfo propertyInfo)
    {
        var type = typeof(PropertyWrapper<,>).MakeGenericType(propertyInfo.DeclaringType!, propertyInfo.PropertyType);

        internalAccessor = (IPropertyAccessor<TSource, TValue>)Activator.CreateInstance(type, propertyInfo)!;
    }

    public TValue Get(TSource target)
    {
        return internalAccessor.Get(target);
    }

    public void Set(TSource target, TValue value)
    {
        internalAccessor.Set(target, value);
    }
}

internal interface IPropertyAccessor<TSource>
{
    object? Get(TSource source);

    void Set(TSource source, object? value);
}

internal interface IPropertyAccessor<TSource, TValue>
{
    TValue Get(TSource source);

    void Set(TSource source, TValue value);
}

internal sealed class PropertyWrapper<TSource, TValue> : IPropertyAccessor<TSource>, IPropertyAccessor<TSource, TValue>
{
    private readonly Func<TSource, TValue> getMethod;
    private readonly Action<TSource, TValue> setMethod;

    public PropertyWrapper(PropertyInfo propertyInfo)
    {
        /*
        var bakingField =
            propertyInfo.DeclaringType!.GetField($"<{propertyInfo.Name}>k__BackingField",
            BindingFlags.NonPublic |
            BindingFlags.Instance);
        */

        if (propertyInfo.CanRead)
        {
            var propertyGetMethod = propertyInfo.GetGetMethod()!;

            var getCaller = new DynamicMethod(propertyGetMethod.Name, typeof(TValue), new[] { typeof(TSource) }, false);

            var getGenerator = getCaller.GetILGenerator();
            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Call, propertyGetMethod);
            getGenerator.Emit(OpCodes.Ret);

            getMethod = getCaller.CreateDelegate<Func<TSource, TValue>>();
        }
        else
        {
            getMethod = x => throw new NotSupportedException();
        }

        if (propertyInfo.CanWrite)
        {
            var propertySetMethod = propertyInfo.GetSetMethod()!;

            var setter = new DynamicMethod(propertySetMethod.Name, null, new[] { typeof(TSource), typeof(TValue) }, false);

            var setGenerator = setter.GetILGenerator();
            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            setGenerator.Emit(OpCodes.Call, propertyInfo.GetSetMethod()!);
            setGenerator.Emit(OpCodes.Ret);

            setMethod = setter.CreateDelegate<Action<TSource, TValue>>();
        }
        else
        {
            setMethod = (x, y) => throw new NotSupportedException();
        }
    }

    object? IPropertyAccessor<TSource>.Get(TSource source)
    {
        return getMethod(source);
    }

    void IPropertyAccessor<TSource>.Set(TSource source, object? value)
    {
        setMethod(source, (TValue)value!);
    }

    TValue IPropertyAccessor<TSource, TValue>.Get(TSource source)
    {
        return getMethod(source);
    }

    void IPropertyAccessor<TSource, TValue>.Set(TSource source, TValue value)
    {
        setMethod(source, value);
    }
}
