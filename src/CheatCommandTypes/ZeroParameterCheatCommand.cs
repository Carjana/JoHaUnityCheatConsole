using System;

namespace JoHaCheatConsole
{
    public class ZeroParameterCheatCommand : BaseCheatCommand
    {
        private readonly Action _action;

        public ZeroParameterCheatCommand(string commandName, string description, Action action) : base(commandName, description)
        {
            _action = action;
        }

        public override void Execute(string[] parameters = null) => _action.Invoke();

        public override bool IsValidParameters(string[] parameters) => parameters.Length == 0;
    }
}