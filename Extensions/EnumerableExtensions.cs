using System.Reflection;
using System.Text.RegularExpressions;

namespace System.Collection.Generic
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> AssertAll<T>(this IEnumerable<T> enumerable, Func<T, bool> assert, Func<T, string> error)
        {
            foreach (var item in enumerable)
            {
                if (!assert(item)) throw new Exception(error(item));
                yield return item;
            }
        }

        public static IEnumerable<T> Transform<T>(this IEnumerable<string> enumerable, Regex regex)
        {
            var type = typeof(T);
            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            var parameters = constructor.GetParameters();

            foreach (var line in enumerable)
            {
                T result;
                var match = regex.Match(line);
                try
                {
                    result = (T)_TransformMatchToObject(match, constructor, parameters);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Regex ({regex}) failed to transform data: {line}", ex);
                }

                yield return result;
            }
        }

        private static object _TransformMatchToObject(Match match, ConstructorInfo constructor, ParameterInfo[] parameters)
        {
            if (!match.Success) throw new Exception($"Regex did not match input.");
            if (parameters.Length != match.Groups.Count + 1) throw new Exception($"Regex match count ({match.Groups.Count}) does not match object constructor parameter count ({parameters.Length}).");

            var arguments = Enumerable
                .Range(0, parameters.Length)
                .Select(index => _TransformValueToParameter(match.Groups[index + 1].Value, parameters[index]))
                .ToArray();

            return constructor.Invoke(arguments);
        }

        private static object _TransformValueToParameter(string value, ParameterInfo parameter)
        {
            switch (parameter.ParameterType.FullName)
            {
                case "System.Char":
                    return value.Single();

                case "System.Int32":
                    return Int32.Parse(value);

                case "System.Int64":
                    return Int64.Parse(value);

                case "System.String":
                    return value;

                default:
                    throw new Exception($"Unhandled parameter type: {parameter.ParameterType.FullName}");
            }
        }
    }
}
