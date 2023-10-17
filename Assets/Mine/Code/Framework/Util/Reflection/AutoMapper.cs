// 두 Type의 같은 이름의 PropertyInfo끼리 매핑해주기 위한 Delegate를 캐싱해주는 Configuration

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Mine.Code.Framework.Util.Reflection
{
    public static class AutoMapper
    {
        static Dictionary<(Type, Type), List<Action<object, object>>> mappers = new();

        public static void CreateMap<TSource, TTarget>() => CreateMap(typeof(TSource), typeof(TTarget));

        public static void CreateMap(Type sourceType, Type targetType)
        {
            var propertysBySource = PropertyCache.GetPropertyInfos(sourceType);
            var propertysByTarget = PropertyCache.GetPropertyInfos(targetType);

            var mapper = new List<Action<object, object>>();

            foreach (var propertyBySouece in propertysBySource)
            {
                var propertyByTarget = propertysByTarget.FirstOrDefault(x => x.Name == propertyBySouece.Name);
                if (propertyByTarget == null) continue;

                var sourceParameter = Expression.Parameter(typeof(object), "source");
                var targetParameter = Expression.Parameter(typeof(object), "target");

                var sourceCast = Expression.Convert(sourceParameter, sourceType);
                var targetCast = Expression.Convert(targetParameter, targetType);

                var sourceProperty = Expression.Property(sourceCast, propertyBySouece);
                var targetProperty = Expression.Property(targetCast, propertyByTarget);

                var assign = Expression.Assign(targetProperty, sourceProperty);
                var lambda = Expression.Lambda<Action<object, object>>(assign, sourceParameter, targetParameter);

                mapper.Add(lambda.Compile());
            }

            mappers.Add((sourceType, targetType), mapper);
        }

        public static void Map<TSource, TTarget>(TSource source, TTarget target)
        {
            var key = (typeof(TSource), typeof(TTarget));
            if (!mappers.TryGetValue(key, out var mapper))
            {
                CreateMap<TSource, TTarget>();
                mapper = mappers[key];
            }

            foreach (var action in mapper)
            {
                action(source, target);
            }
        }
    }
}
