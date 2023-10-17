using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Mine.Code.Framework.Util.Reflection
{
    public static class ReflectionUtility
    {
        #region Assembly 구하는 방법 정리

        // Assembly.GetExecutingAssembly() : 현재 실행중인 Assembly
        // type.Assembly : type이 포함된 Assembly
        // Assembly.GetCallingAssembly() : 현재 호출된 메소드를 포함하는 Assembly
        // Assembly.GetEntryAssembly() : 현재 실행중인 프로그램의 Assembly
        // Assembly.GetAssembly(typeof(Foo)) : Foo가 포함된 Assembly

        #endregion

        #region 객체 생성 방법 정리

        // (T)Activator.CreateInstance(type, args) : type의 객체를 생성하고 args를 생성자에 전달

        #endregion
    
        // Assembly 내의 특정 타입의 하위 타입들을 찾아서 반환한다.
        public static ReadOnlyCollection<Type> GetDerivedTypes<T>(params Assembly[] assemblies)
        {
            return DerivedTypeCache.GetDerivedTypes(typeof(T), assemblies);
        }
    
        // Assembly 내의 특정 Attribute가 붙은 타입들을 찾아서 반환한다.
        public static ReadOnlyCollection<Type> GetTypesByAttribute<T>(params Assembly[] assemblies)
        {
            return TypeByAttributeCache.GetTypesByAttribute(typeof(T), assemblies);
        }
        
        // 지정된 타입의 필드 멤버 목록을 가져온다.
        public static ReadOnlyCollection<FieldInfo> GetFields(Type type)
        {
            return FieldCache.GetFieldInfos(type);
        }
        
        // 지정된 타입의 속성 멤버 목록을 가져온다.
        public static ReadOnlyCollection<PropertyInfo> GetProperties(Type type)
        {
            return PropertyCache.GetPropertyInfos(type);
        }
    }
}