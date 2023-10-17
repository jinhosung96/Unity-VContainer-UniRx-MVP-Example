using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Mine.Code.Framework.Util.Reflection
{
    public static class TypeByAttributeCache
    {
        public struct TypeContext
        {
            public Assembly[] Assemblies;
            public Type[] Types;
        }

        static Dictionary<(Type, Assembly[]), TypeContext> contexts = new();

        public static ReadOnlyCollection<Type> GetTypesByAttribute(Type attributeType, params Assembly[] assemblies)
        {
            if (contexts.TryGetValue((attributeType, assemblies), out TypeContext context) == true)
            {
                return new ReadOnlyCollection<Type>(context.Types);
            }

            InternalInit(attributeType, assemblies);

            return new ReadOnlyCollection<Type>(contexts[(attributeType, assemblies)].Types);
        }

        static void InternalInit(Type attributeType, Assembly[] assemblies)
        {
            TypeContext context = new TypeContext();
            if (assemblies != null && assemblies.Length > 0)
            {
                List<Type> temp = new();
                foreach (Assembly assembly in assemblies)
                {
                    temp.AddRange(assembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Any()));
                }

                context.Assemblies = assemblies;
                context.Types = temp.ToArray();
            }
            else
            {
                context.Types = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Any()).ToArray();
            }

            contexts.Add((attributeType, assemblies), context);
        }
    }
}