// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

#pragma warning disable RECS0108 // Warns about static fields in generic types

namespace Benchmarks.Mapping.Internal;

public static class SimpleMapper
{
    private readonly record struct MappingContext
    {
        required public CultureInfo Culture { get; init; }

        required public bool NullableAsOptional { get; init; }
    }

    private interface IPropertyMapper<TSource, TTarget>
    {
        void MapProperty(TSource source, TTarget target, ref MappingContext context);
    }

    private static class SimplePropertyMapperFactory
    {
        public static IPropertyMapper<TSource, TTarget> Create<TSource, TTarget>(Type valueType, PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            var method =
                typeof(SimplePropertyMapperFactory)
                    .GetMethod(nameof(SimplePropertyMapperFactory.CreateCore),
                        BindingFlags.Static |
                        BindingFlags.NonPublic)!
                    .MakeGenericMethod(typeof(TSource), typeof(TTarget), valueType);

            return (IPropertyMapper<TSource, TTarget>)method.Invoke(null, new object?[] { sourceProperty, targetProperty })!;
        }

        private static IPropertyMapper<TSource, TTarget> CreateCore<TSource, TTarget, TValue>(PropertyInfo sourceProperty, PropertyInfo targetProperty)
        {
            return new SimplePropertyMapper<TSource, TTarget, TValue>(
                new PropertyAccessor<TSource, TValue>(sourceProperty),
                new PropertyAccessor<TTarget, TValue>(targetProperty));
        }
    }

    private sealed class SimplePropertyMapper<TSource, TTarget, TValue> : IPropertyMapper<TSource, TTarget>
    {
        private readonly PropertyAccessor<TSource, TValue> sourceAccessor;
        private readonly PropertyAccessor<TTarget, TValue> targetAccessor;

        public SimplePropertyMapper(
            PropertyAccessor<TSource, TValue> sourceAccessor,
            PropertyAccessor<TTarget, TValue> targetAccessor)
        {
            this.sourceAccessor = sourceAccessor;
            this.targetAccessor = targetAccessor;
        }

        public void MapProperty(TSource source, TTarget target, ref MappingContext context)
        {
            var value = sourceAccessor.Get(source);

            targetAccessor.Set(target, value);
        }
    }

    private sealed class StringConversionPropertyMapper<TSource, TTarget> : PropertyMapper<TSource, TTarget>
    {
        public StringConversionPropertyMapper(
            PropertyAccessor<TSource> sourceAccessor,
            PropertyAccessor<TTarget> targetAccessor)
            : base(sourceAccessor, targetAccessor)
        {
        }

        public override void MapProperty(TSource source, TTarget target, ref MappingContext context)
        {
            var value = GetValue(source);

            SetValue(target, value?.ToString());
        }
    }

    private sealed class NullablePropertyMapper<TSource, TTarget> : PropertyMapper<TSource, TTarget>
    {
        private readonly object? defaultValue;

        public NullablePropertyMapper(
            PropertyAccessor<TSource> sourceAccessor,
            PropertyAccessor<TTarget> targetAccessor,
            object? defaultValue)
            : base(sourceAccessor, targetAccessor)
        {
            this.defaultValue = defaultValue;
        }

        public override void MapProperty(TSource source, TTarget target, ref MappingContext context)
        {
            var value = GetValue(source);

            if (value == null)
            {
                if (context.NullableAsOptional)
                {
                    return;
                }
                else
                {
                    value = defaultValue;
                }
            }

            SetValue(target, value);
        }
    }

    private sealed class ConversionPropertyMapper<TSource, TTarget> : PropertyMapper<TSource, TTarget>
    {
        private readonly Type targetType;

        public ConversionPropertyMapper(
            PropertyAccessor<TSource> sourceAccessor,
            PropertyAccessor<TTarget> targetAccessor,
            Type targetType)
            : base(sourceAccessor, targetAccessor)
        {
            this.targetType = targetType;
        }

        public override void MapProperty(TSource source, TTarget target, ref MappingContext context)
        {
            var value = GetValue(source);

            if (value == null)
            {
                return;
            }

            try
            {
                var converted = Convert.ChangeType(value, targetType, context.Culture);

                SetValue(target, converted);
            }
            catch
            {
                return;
            }
        }
    }

    private sealed class TypeConverterPropertyMapper<TSource, TTarget> : PropertyMapper<TSource, TTarget>
    {
        private readonly TypeConverter converter;

        public TypeConverterPropertyMapper(
            PropertyAccessor<TSource> sourceAccessor,
            PropertyAccessor<TTarget> targetAccessor,
            TypeConverter converter)
            : base(sourceAccessor, targetAccessor)
        {
            this.converter = converter;
        }

        public override void MapProperty(TSource source, TTarget target, ref MappingContext context)
        {
            var value = GetValue(source);

            if (value == null)
            {
                return;
            }

            try
            {
                var converted = converter.ConvertFrom(null, context.Culture, value);

                SetValue(target, converted);
            }
            catch
            {
                return;
            }
        }
    }

    private class PropertyMapper<TSource, TTarget> : IPropertyMapper<TSource, TTarget>
    {
        private readonly PropertyAccessor<TSource> sourceAccessor;
        private readonly PropertyAccessor<TTarget> targetAccessor;

        public PropertyMapper(PropertyAccessor<TSource> sourceAccessor, PropertyAccessor<TTarget> targetAccessor)
        {
            this.sourceAccessor = sourceAccessor;
            this.targetAccessor = targetAccessor;
        }

        public virtual void MapProperty(TSource source, TTarget target, ref MappingContext context)
        {
            var value = GetValue(source);

            SetValue(target, value);
        }

        protected void SetValue(TTarget target, object? value)
        {
            targetAccessor.Set(target, value);
        }

        protected object? GetValue(TSource source)
        {
            return sourceAccessor.Get(source);
        }
    }

    private static class ClassMapper<TSource, TTarget> where TSource : class where TTarget : class
    {
        private static readonly List<IPropertyMapper<TSource, TTarget>> Mappers = new List<IPropertyMapper<TSource, TTarget>>();

        static ClassMapper()
        {
            var sourceClassType = typeof(TSource);
            var sourceProperties =
                sourceClassType.GetPublicProperties()
                    .Where(x => x.CanRead).ToList();

            var targetClassType = typeof(TTarget);
            var targetProperties =
                targetClassType.GetPublicProperties()
                    .Where(x => x.CanWrite).ToList();

            foreach (var sourceProperty in sourceProperties)
            {
                var targetProperty = targetProperties.Find(x => x.Name == sourceProperty.Name);

                if (targetProperty == null)
                {
                    continue;
                }

                var sourceType = sourceProperty.PropertyType;
                var targetType = targetProperty.PropertyType;

                if (sourceType == targetType)
                {
                    Mappers.Add(SimplePropertyMapperFactory.Create<TSource, TTarget>(sourceType, sourceProperty, targetProperty));
                }
                else if (targetType == typeof(string))
                {
                    Mappers.Add(new StringConversionPropertyMapper<TSource, TTarget>(
                        new PropertyAccessor<TSource>(sourceProperty),
                        new PropertyAccessor<TTarget>(targetProperty)));
                }
                else if (IsNullableOf(sourceType, targetType))
                {
                    Mappers.Add(new NullablePropertyMapper<TSource, TTarget>(
                        new PropertyAccessor<TSource>(sourceProperty),
                        new PropertyAccessor<TTarget>(targetProperty),
                        Activator.CreateInstance(targetType)));
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(targetType);

                    if (converter.CanConvertFrom(sourceType))
                    {
                        Mappers.Add(new TypeConverterPropertyMapper<TSource, TTarget>(
                            new PropertyAccessor<TSource>(sourceProperty),
                            new PropertyAccessor<TTarget>(targetProperty),
                            converter));
                    }
                    else if (sourceType.Implements<IConvertible>() || targetType.Implements<IConvertible>())
                    {
                        Mappers.Add(new ConversionPropertyMapper<TSource, TTarget>(
                            new PropertyAccessor<TSource>(sourceProperty),
                            new PropertyAccessor<TTarget>(targetProperty),
                            targetType));
                    }
                }
            }

            static bool IsNullableOf(Type type, Type wrappedType)
            {
                return type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    type.GenericTypeArguments[0] == wrappedType;
            }
        }

        public static TTarget MapClass(TSource source, TTarget destination, ref MappingContext context)
        {
            for (var i = 0; i < Mappers.Count; i++)
            {
                var mapper = Mappers[i];

                mapper.MapProperty(source, destination, ref context);
            }

            return destination;
        }
    }

    public static TTarget Map<TSource, TTarget>(TSource source, TTarget target)
        where TSource : class
        where TTarget : class
    {
        return Map(source, target, CultureInfo.CurrentCulture, true);
    }

    public static TTarget Map<TSource, TTarget>(TSource source, TTarget target, CultureInfo culture, bool nullableAsOptional)
        where TSource : class
        where TTarget : class
    {
        var context = new MappingContext
        {
            Culture = culture,
            NullableAsOptional = nullableAsOptional
        };

        return ClassMapper<TSource, TTarget>.MapClass(source, target, ref context);
    }
}
