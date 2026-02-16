using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JoHaCheatConsole
{
    public static class ReflectionHelper
    {
        public static MethodInfoCheatCommand MethodInfoToCheatCommand(MethodInfo info, CheatCommandAttribute attribute)
        {
            return new MethodInfoCheatCommand(attribute.CommandName ?? info.Name,
                attribute.Description, info);
        }
        
        public static bool TryGetCheatCommandAttribute(MethodInfo methodInfo, out CheatCommandAttribute attribute)
        {
            attribute = null;
            try
            {
                attribute = methodInfo.GetCustomAttribute<CheatCommandAttribute>();
            }
            catch (Exception e)
            {
                Debug.Log("Error while extracting CheatCommand methods: " + e);
                return false;
            }
            
            return attribute != null;
        }
    }
    

}