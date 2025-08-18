using System;
using System.Collections.Generic;
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
                .Where(x =>
                {
                    try
                    {
                        return x.CustomAttributes.Any(y => y.AttributeType == typeof(AbcCommandAttribute));
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        return false;
                    }
                });

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
                var value = args[0];
                if (type.IsPrimitive)
                {
                    try
                    {
                        if (type == typeof(Boolean)) parameters.Add(Boolean.Parse(value));
                        if (type == typeof(Byte)) parameters.Add(Byte.Parse(value));
                        if (type == typeof(SByte)) parameters.Add(SByte.Parse(value));
                        if (type == typeof(Int16)) parameters.Add(Int16.Parse(value));
                        if (type == typeof(UInt16)) parameters.Add(UInt16.Parse(value));
                        if (type == typeof(Int32)) parameters.Add(Int32.Parse(value));
                        if (type == typeof(UInt32)) parameters.Add(UInt32.Parse(value));
                        if (type == typeof(Int64)) parameters.Add(Int64.Parse(value));
                        if (type == typeof(UInt64)) parameters.Add(UInt64.Parse(value));
                        if (type == typeof(Char)) parameters.Add(Char.Parse(value));
                        if (type == typeof(Double)) parameters.Add(Double.Parse(value));
                        if (type == typeof(Single)) parameters.Add(Single.Parse(value));
                    }
                    catch (Exception e)
                    {
                        if (e is FormatException e1)
                        {
                            Debug.Log($"parse error: {value} / {e1.Message}");
                            return false;
                        }

                        if (e.InnerException is FormatException e2)
                        {
                            Debug.Log($"parse error: {value} / {e.Message} / {e2.Message}");
                            return false;
                        }

                        throw;
                    }
                }
                else if (type.IsEnum)
                {
                    try
                    {
                        if (Int32.TryParse(value, out var i))
                        {
                            parameters.Add(i);
                        }
                        else
                        {
                            parameters.Add(Enum.Parse(type, value, true));
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is ArgumentException e1)
                        {
                            Debug.Log($"enum parse error: {value} / {e1.Message}");
                            return false;
                        }

                        throw;
                    }
                }
                else if (type == typeof(string))
                {
                    parameters.Add(args.Count > 0 ? value : default);
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