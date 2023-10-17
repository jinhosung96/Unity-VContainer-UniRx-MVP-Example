using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Mine.Code.Framework.Util.Reflection
{
    public static class DerivedTypeCache
    {
        public struct DeriveTypeContext
        {
            public Assembly[] Assemblies;
            public Type[] DerivedTypes;
        }

        static Dictionary<(Type, Assembly[]), DeriveTypeContext> contexts = new();

        public static ReadOnlyCollection<Type> GetDerivedTypes(Type type, params Assembly[] assemblies)
        {
            if (contexts.TryGetValue((type, assemblies), out DeriveTypeContext context) == true)
            {
                return new ReadOnlyCollection<Type>(context.DerivedTypes);
            }

            InternalInit(type, assemblies);

            return new ReadOnlyCollection<Type>(contexts[(type, assemblies)].DerivedTypes);
        }

        static void InternalInit(Type type, Assembly[] assemblies)
        {
            DeriveTypeContext context = new DeriveTypeContext();
            if (assemblies != null && assemblies.Length > 0)
            {
                List<Type> temp = new();
                foreach (Assembly assembly in assemblies)
                {
                    temp.AddRange(assembly.GetTypes().Where(t => type.IsAssignableFrom(t) && t != type && t.IsInterface == false && t.IsAbstract == false));
                }

                context.Assemblies = assemblies;
                context.DerivedTypes = temp.ToArray();
            }
            else
            {
                context.DerivedTypes = type.Assembly.GetTypes().Where(t => type.IsAssignableFrom(t) && t != type && t.IsInterface == false && t.IsAbstract == false).ToArray();
            }

            contexts.Add((type, assemblies), context);
        }
    }
}