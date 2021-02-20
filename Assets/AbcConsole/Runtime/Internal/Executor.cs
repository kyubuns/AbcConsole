using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AbcConsole.Internal
{
    public class Executor
    {
        private readonly List<DebugCommand> _debugCommands;

        public Executor()
        {
            var allMethods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(AbcCommandAttribute)));

            _debugCommands = allMethods.Select(x => new DebugCommand(x, x.GetCustomAttribute<AbcCommandAttribute>())).ToList();
        }

        public bool ExecuteMethod(string text)
        {
            var input = text.Split(' ').Select(x => x.Trim()).ToArray();

            var method = _debugCommands.FirstOrDefault(x => string.Equals(x.MethodInfo.Name, input[0], StringComparison.OrdinalIgnoreCase));
            if (method == null)
            {
                Debug.Log($"{input[0]} is not found");
                return false;
            }

            var parameters = new List<object>();
            var args = input.Skip(1).ToList();
            var parameterInfos = method.MethodInfo.GetParameters();
            if (parameterInfos.Length != args.Count)
            {
                Debug.Log($"{method.MethodInfo.Name} needs {parameterInfos.Length} parameters");
                return false;
            }

            foreach (var parameterInfo in parameterInfos)
            {
                var type = parameterInfo.ParameterType;
                if (type.IsPrimitive)
                {
                    var value = args.Count > 0
                        ? TypeDescriptor.GetConverter(type).ConvertFromString(args[0])
                        : Activator.CreateInstance(type);
                    parameters.Add(value);
                }
                else if (type == typeof(string))
                {
                    parameters.Add(args.Count > 0 ? args[0] : default);
                }
                else
                {
                    Debug.Log($"parse error: {type}");
                }

                if (args.Count > 0) args.RemoveAt(0);
            }

            method.MethodInfo.Invoke(null, parameters.ToArray());
            return true;
        }

        public DebugCommand[] GetAutocomplete(string text)
        {
            return _debugCommands.ToArray();
        }
    }
}