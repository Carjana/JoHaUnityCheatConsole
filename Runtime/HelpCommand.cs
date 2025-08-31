using System;
using System.Text;
using JoHaToolkit.UnityEngine.CheatConsole;
using UnityEngine;

namespace JohaToolkit.UnityEngine.CheatConsole
{
    

    public class HelpCommand : BaseCheatCommand
    {
        public HelpCommand(string commandName, string description) : base(commandName, description)
        {
        }

        public override void Execute(string[] parameter = null)
        {
            PrintHelp();
        }

        public override bool IsValidParameters(string[] parameters) => parameters.Length == 0;

        private void PrintHelp()
        {
            foreach (BaseCheatCommand cheatCommand in CheatCommandExecutor.CheatCommands.Values)
            {
                StringBuilder stringBuilder = new();
                stringBuilder.Append($"{cheatCommand.CommandName} ");

                if (cheatCommand.ParameterNames != null && cheatCommand.ParameterTypes != null)
                    for (int i = 0; i < cheatCommand.ParameterNames.Length; i++)
                    {
                        stringBuilder.Append(
                            $"<{GetParameterTypeName(cheatCommand.ParameterTypes[i])}> {cheatCommand.ParameterNames[i]}");
                        if (i != cheatCommand.ParameterNames.Length - 1)
                            stringBuilder.Append(", ");
                    }

                stringBuilder.Append($" - {cheatCommand.Description ?? "<Missing Description>"}");

                DebugConsole.Instance?.AddLog(stringBuilder.ToString(), Color.gray);
            }
        }

        private string GetParameterTypeName(Type parameterType) =>
            parameterType == typeof(float) ? "float" : parameterType.Name;
    }
}