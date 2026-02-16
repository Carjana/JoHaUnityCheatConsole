using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JoHaCheatConsole
{
    public static class CheatCommandExecutor
    {
        public static Dictionary<string, BaseCheatCommand> CheatCommands { get; } = new();
        
        private static BaseCheatCommand[] _commandCache;
        private static string _lastCommand;

        public static void Init(string[] assembliesToSearch, bool searchAllAssemblies = false)
        {
            GenerateCheatCommandsList(assembliesToSearch, searchAllAssemblies);
            _commandCache = CheatCommands.Values.ToArray();
        }

        private static void GenerateCheatCommandsList(string[] assembliesToSearch, bool searchAllAssemblies = false)
        {
            Assembly[] assemblies = searchAllAssemblies? AppDomain.CurrentDomain.GetAssemblies() : AppDomain.CurrentDomain.GetAssemblies().Where(a => assembliesToSearch.Any(a.FullName.StartsWith)).ToArray();

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                    foreach (MethodInfo methodInfo in type.GetMethods(flags))
                    {
                        if(!ReflectionHelper.TryGetCheatCommandAttribute(methodInfo, out CheatCommandAttribute attribute))
                            continue;
                        
                        MethodInfoCheatCommand cheatCommand = ReflectionHelper.MethodInfoToCheatCommand(methodInfo, attribute);
                        if (CheatCommands.TryAdd(cheatCommand.CommandName, cheatCommand))
                            continue;
                        
                        Debug.LogWarning($"{cheatCommand.CommandName} already exists in the cheat commands list! Check the command names! ({cheatCommand.MethodInfo.DeclaringType}.{cheatCommand.MethodInfo.Name})");
                    }
                }
            }
        }

        public static bool AddCommand(string name, string description, Action action) => AddCommand(new ZeroParameterCheatCommand(name, description, action));
        
        public static bool AddCommand(BaseCheatCommand command)
        {
            if (CheatCommands.TryAdd(command.CommandName, command)) 
                return true;
            Debug.LogWarning($"{command.CommandName} already exists in the cheat commands list! Command names must be unique!");
            return false;
        }

        public static bool RemoveCommand(string commandName) => CheatCommands.Remove(commandName);

        public static bool IsValidCommand(string command)
        {
            string[] commandParts = SplitCommand(command);
            if (commandParts.Length == 0)
                return false;

            if (!IsValidCommandName(commandParts.First()))
                return false;

            BaseCheatCommand cheatCommand = CheatCommands.GetValueOrDefault(commandParts.First());
            return cheatCommand != null && cheatCommand.IsValidParameters(commandParts.Skip(1).ToArray());
        }
        
        public static bool IsValidCommandName(string commandName) => CheatCommands.ContainsKey(commandName);

        private static string[] SplitCommand(string command) => command.Split(' ').Where(part => part.Trim() != "").ToArray();

        public static void Execute(string command)
        {
            if (!IsValidCommand(command))
            {
                DebugConsole.Instance?.AddLog($"Invalid Command! {command} (Check Params)", Color.yellow);
                return;
            }

            string[] commandParts = SplitCommand(command);

            BaseCheatCommand baseCheatCommand = CheatCommands.GetValueOrDefault(commandParts.First());
            try
            {
                baseCheatCommand.Execute(commandParts.Skip(1).ToArray());

                DebugConsole.Instance.AddLog($"Executed Command \"{command}\" Successfully!", Color.green);
            }
            catch (Exception e)
            {
                DebugConsole.Instance.AddLog($"Failed to execute Command {baseCheatCommand.CommandName} \n{e}", Color.red);
            }
        }
        
        public static BaseCheatCommand[] GetPossibleCommands(string command)
        {
            if (_lastCommand != null && _lastCommand.Equals(command))
                return _commandCache;

            string[] commandParts = SplitCommand(command);

            if (commandParts.Length == 0)
            {
                _lastCommand = command;
                _commandCache = CheatCommands.Values.ToArray();
                return _commandCache;
            }

            bool cutOff = command.Contains(" ");
            
            BaseCheatCommand[] possibleCommandsByName = CheatCommands.Values.Where(cheatCommand =>
                (!cutOff && cheatCommand.CommandName.StartsWith(commandParts.First())) ||
                (cutOff && cheatCommand.CommandName.Equals(commandParts[0]))
                ).ToArray();

            _lastCommand = command;
            _commandCache = possibleCommandsByName;
            
            return possibleCommandsByName;
        }
    }

}