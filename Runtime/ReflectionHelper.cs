using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JoHaToolkit.UnityEngine.CheatConsole
{
    public static class ReflectionHelper
    {
        public static (MethodInfo, CheatCommandAttribute)[] GetMethodInfos(string[] assembliesToSearch, bool searchAllAssemblies = false)
        {
            List<(MethodInfo, CheatCommandAttribute)> methodInfos = new();
            Assembly[] assemblies = searchAllAssemblies? AppDomain.CurrentDomain.GetAssemblies() : AppDomain.CurrentDomain.GetAssemblies().Where(a => IsAssembly(a.FullName, assembliesToSearch)).ToArray();

            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    ExtractCheatCommandMethods(methodInfos, type);
                }
            }

            return methodInfos.ToArray();
        }

        private static void ExtractCheatCommandMethods(List<(MethodInfo, CheatCommandAttribute)> methodInfos, Type type)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            foreach (MethodInfo methodInfo in type.GetMethods(flags))
            {
                try
                {
                    CheatCommandAttribute attribute = methodInfo.GetCustomAttribute<CheatCommandAttribute>();
                    if (attribute == null)
                        continue;
                    methodInfos.Add((methodInfo, attribute));

                }
                catch (Exception e)
                {
                    Debug.Log("Error while extracting CheatCommand methods: " + e);
                }
            }
        }

        private static bool IsAssembly(string assemblyName, string[] assembliesToSearch)
        {
            return assembliesToSearch.Any(assemblyName.StartsWith);
        }
    }
    

}