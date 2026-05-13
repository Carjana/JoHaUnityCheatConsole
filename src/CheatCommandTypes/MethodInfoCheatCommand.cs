using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JoHaCheatConsole
{
    public class MethodInfoCheatCommand : BaseCheatCommand
    {
        public MethodInfo OwningMethodInfo { get; }
        public ParameterInfo[] Parameters { get; }

        private readonly DebugConsoleUnitySceneAPI _sceneAPI;

        public MethodInfoCheatCommand(string commandName, string description, MethodInfo owningMethodInfo, DebugConsoleUnitySceneAPI sceneAPI) : base(commandName, description)
        {
            _sceneAPI = sceneAPI;
            OwningMethodInfo = owningMethodInfo;
            Parameters = OwningMethodInfo.GetParameters();
            ParameterTypes = Parameters.Select(parameter => parameter.ParameterType).ToArray();
            ParameterNames = Parameters.Select(parameter => parameter.Name).ToArray();
        }

        public override void Execute(string[] parameter = null)
        {
            if(parameter == null || parameter.Length != Parameters.Length)
                throw new ArgumentException($"Command {CommandName} expects {Parameters.Length} parameters, but got {parameter?.Length ?? 0} parameters.");
            
            if (!IsValidParameters(parameter))
                throw new ArgumentException($"Invalid parameters for command {CommandName}.");
            
            string[] inputParameters = parameter.ToArray();

            object[] parameters = new object[inputParameters.Length];

            for (int index = 0; index < inputParameters.Length; index++)
            {
                Type parameterType = ParameterTypes[index];
                
                TypeConverter typeConverter = TypeDescriptor.GetConverter(parameterType);

                object param = typeConverter.ConvertFromInvariantString(inputParameters[index]);

                parameters[index] = param ?? throw new ArgumentException($"Failed to convert parameter {index} of {CommandName} to {parameterType.Name}");

            }

            object instanceToInvokeOn = null;
            
            if (!OwningMethodInfo.IsStatic)
            {
                if(!_sceneAPI)
                    throw new NullReferenceException("No Interface to unity scene provided to method Info cheat command! Can't invoke non static method without scene access!");
                Type type = OwningMethodInfo.ReflectedType;
                instanceToInvokeOn = _sceneAPI.GetComponentFromUnity(type);
                if(instanceToInvokeOn == null)
                    throw new NullReferenceException($"Failed to get instance of type {type} from unity scene");
            }
            
            OwningMethodInfo.Invoke(instanceToInvokeOn, parameters);
        }

        public override bool IsValidParameters(string[] parameters)
        {
            if (parameters.Length != Parameters.Length)
                return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(ParameterTypes[i]);
                if (!typeConverter.IsValid(parameters[i]))
                    return false;
            }

            return true;
        }
    }
}