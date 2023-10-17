using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Mine.Code.Framework.Util.Reflection
{
    // Type을 키 값으로 FieldInfo를 캐싱하는 클래스
    public static class FieldCache
    {
        // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리
        static Dictionary<Type, Dictionary<string, FieldInfo>> cache = new();

        // Type과 FieldName을 받아서 FieldInfo를 반환하는 메서드
        public static FieldInfo GetFieldInfo(Type type, string fieldName)
        {
            // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리에 Type이 없다면
            // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리에 Type에 대한 FieldInfo들을 캐싱
            CacheFieldInfos(type);

            // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리의 Type의 FieldName을 반환
            return cache[type][fieldName];
        }

        // Type과 FieldName을 받아서 FieldInfo를 반환하는 메서드
        public static FieldInfo GetFieldInfo<T>(string fieldName) => GetFieldInfo(typeof(T), fieldName);

        // Type을 받아서 Type의 모든 FieldInfo를 반환하는 메서드
        public static ReadOnlyCollection<FieldInfo> GetFieldInfos(Type type)
        {
            // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리에 Type이 없다면
            // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리에 Type에 대한 FieldInfo들을 캐싱
            CacheFieldInfos(type);

            // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리의 Type의 모든 FieldInfo를 반환
            return new ReadOnlyCollection<FieldInfo>(cache[type].Values.ToList());
        }

        // Type을 받아서 Type의 모든 FieldInfo를 반환하는 메서드
        public static ReadOnlyCollection<FieldInfo> GetFieldInfos<T>() => GetFieldInfos(typeof(T));

        static void CacheFieldInfos(Type type)
        {
            // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리에 Type이 없다면
            if (!cache.ContainsKey(type))
            {
                // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리에 Type을 추가
                cache.Add(type, new Dictionary<string, FieldInfo>());

                // Type의 모든 FieldInfo를 가져옴
                FieldInfo[] fieldInfos = type.GetFields();

                // Type을 키 값으로 FieldInfo를 캐싱하는 딕셔너리의 Type에 FieldInfo를 추가
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    cache[type].Add(fieldInfo.Name, fieldInfo);
                }
            }
        }
    }
}
