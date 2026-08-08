using System;
using System.ComponentModel;

namespace JoHaCheatConsole
{
    public class OneParameterCheatCommand<T> : BaseCheatCommand
    {
        private readonly Action<T> _action;
        public OneParameterCheatCommand(string commandName, string description, Action<T> action, string parameterName) : base(commandName, description)
        {
            _action = action;
            ParameterTypes = new []{typeof(T)};
            ParameterNames =  new []{parameterName};
        }

        public override void Execute(string[] parameter = null)
        {
            if (parameter == null)
                return;
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
            object param = typeConverter.ConvertFromInvariantString(parameter[0]);
            _action?.Invoke((T)param);
        }

        public override bool IsValidParameters(string[] parameters)
        {
            if (parameters.Length is > 1 or <= 0)
                return false;
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));
            return typeConverter.IsValid(parameters[0]);
            
            
        }
    }
}