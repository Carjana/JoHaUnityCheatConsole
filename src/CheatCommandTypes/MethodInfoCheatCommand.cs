using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace JoHaCheatConsole
{
    public class MethodInfoCheatCommand : BaseCheatCommand
    {
        public MethodInfo MethodInfo { get; }
        public ParameterInfo[] Parameters { get; }

        public MethodInfoCheatCommand(string commandName, string description, MethodInfo methodInfo) : base(commandName, description)
        {
            MethodInfo = methodInfo;
            Parameters = MethodInfo.GetParameters();
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
            
            MethodInfo.Invoke(null, parameters);
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