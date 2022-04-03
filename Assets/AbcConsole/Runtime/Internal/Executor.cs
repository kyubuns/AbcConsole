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

            var methods = _debugCommands.Where(x => string.Equals(x.MethodInfo.Name, input[0], StringComparison.OrdinalIgnoreCase)).ToArray();
            if (methods.Length == 0)
            {
                Debug.Log($"{input[0]} is not found");
                return false;
            }

            var parameters = new List<object>();
            var args = input.Skip(1).ToList();
            var method = methods.FirstOrDefault(x => x.MethodInfo.GetParameters().Length == args.Count);
            if (method == null)
            {
                Debug.Log($"{methods[0].MethodInfo.Name} needs {string.Join(", ", methods.Select(x => x.MethodInfo.GetParameters().Length))} parameters");
                return false;
            }
            var parameterInfos = method.MethodInfo.GetParameters();

            foreach (var parameterInfo in parameterInfos)
            {
                var type = parameterInfo.ParameterType;
                if (type.IsPrimitive)
                {
                    try
                    {
                        var value = args.Count > 0
                            ? TypeDescriptor.GetConverter(type).ConvertFromString(args[0])
                            : Activator.CreateInstance(type);
                        parameters.Add(value);
                    }
                    catch (FormatException formatException)
                    {
                        Debug.Log($"parse error: {args[0]} / {formatException.Message}");
                        return false;
                    }
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

        internal DebugCommand[] GetAutocomplete(string text)
        {
            var input = text.Split(' ').Select(x => x.Trim()).ToArray();
            if (input.Length == 0 || string.IsNullOrWhiteSpace(input[0])) return new DebugCommand[] { };

            var methodName = input[0].ToLowerInvariant();
            var methodsStartsWith = _debugCommands
                .Where(x => x.LowerMethodName.StartsWith(methodName))
                .OrderBy(x => x.MethodInfo.Name.Length)
                .ThenBy(x => x.MethodInfo.Name)
                .ToArray();

            if (methodsStartsWith.Length >= Root.MaxAutocompleteSize)
            {
                return methodsStartsWith.Take(Root.MaxAutocompleteSize).ToArray();
            }

            var methodsContains = _debugCommands
                .Where(x => x.LowerMethodName.Contains(methodName))
                .OrderBy(x => x.MethodInfo.Name.Length)
                .ThenBy(x => x.MethodInfo.Name);

            return methodsStartsWith.Concat(methodsContains).Distinct().Take(Root.MaxAutocompleteSize).ToArray();
        }
    }
}