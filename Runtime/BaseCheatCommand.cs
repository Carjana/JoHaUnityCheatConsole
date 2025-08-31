using System;

namespace JoHaToolkit.UnityEngine.CheatConsole
{
    public abstract class BaseCheatCommand
    {
        public string CommandName { get; set; }
        public string Description { get; set; }
        public Type[] ParameterTypes { get; protected set; }
        public string[] ParameterNames { get; protected set; }

        protected BaseCheatCommand(string commandName, string description)
        {
            CommandName = commandName;
            Description = description;
        }

        public abstract void Execute(string[] parameter = null);

        public abstract bool IsValidParameters(string[] parameters);
    }
}