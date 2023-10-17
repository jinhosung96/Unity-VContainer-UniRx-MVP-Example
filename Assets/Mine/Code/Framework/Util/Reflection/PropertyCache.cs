using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Mine.Code.Framework.Util.Reflection
{
    // Type을 키 값으로 PropertyInfo를 캐싱하는 클래스
    public static class PropertyCache
    {
        // Type을 키 값으로 PropertyInfo를 캐싱하는 딕셔너리
        static Dictionary<Type, Dictionary<string, PropertyInfo>> cache = new();

        // Type과 Property 이름을 받아서 PropertyInfo를 반환하는 함수
        public static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            CachePropertyInfo(type);

            // Property 이름을 키 값으로 캐싱된 PropertyInfo를 반환
            return cache[type][propertyName];
        }

        // Type과 Property 이름을 받아서 PropertyInfo를 반환하는 함수
        public static PropertyInfo GetPropertyInfo<T>(string propertyName) => GetPropertyInfo(typeof(T), propertyName);

        // Type을 받아서 Type의 모든 PropertyInfo를 반환하는 메서드
        public static ReadOnlyCollection<PropertyInfo> GetPropertyInfos(Type type)
        {
            CachePropertyInfo(type);

            // 캐싱된 딕셔너리의 모든 PropertyInfo를 반환
            return new ReadOnlyCollection<PropertyInfo>(cache[type].Values.ToList());
        }

        // Type을 받아서 Type의 모든 PropertyInfo를 반환하는 메서드
        public static ReadOnlyCollection<PropertyInfo> GetPropertyInfos<T>() => GetPropertyInfos(typeof(T));

        static void CachePropertyInfo(Type type)
        {
            // Type이 캐싱된 딕셔너리에 없다면
            if (!cache.ContainsKey(type))
            {
                // Type을 키 값으로 새로운 딕셔너리를 생성
                cache.Add(type, new Dictionary<string, PropertyInfo>());

                // Type을 키 값으로 캐싱된 딕셔너리를 가져옴
                Dictionary<string, PropertyInfo> propertyInfoDic = cache[type];

                // Type의 모든 PropertyInfo를 가져옴
                PropertyInfo[] propertyInfos = type.GetProperties();

                // Type의 모든 PropertyInfo를 캐싱된 딕셔너리에 추가
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    propertyInfoDic.Add(propertyInfo.Name, propertyInfo);
                }
            }
        }
    }
}