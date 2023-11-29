using System.Reflection;

namespace System.Text.RegularExpressions
{
    internal static class RegexExtensions
    {
        public static IEnumerable<T> TransformData<T>(this Regex regex, IEnumerable<string> data)
        {
            var type = typeof(T);
            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            var parameters = constructor.GetParameters();
            var arguments = new object[parameters.Length];

            foreach (var line in data)
            {
                var match = regex.Match(line);
                if (!match.Success) throw new Exception($"Regex ({regex}) unable to parse data: {line}");

                for (var index = 0; index < parameters.Length; index++)
                {
                    var parameter = parameters[index];
                    var value = match.Groups[index + 1].Value;
                    switch (parameter.ParameterType.FullName)
                    {
                        case "System.Int32":
                            arguments[index] = Int32.Parse(value);
                            break;

                        case "System.Int64":
                            arguments[index] = Int64.Parse(value);
                            break;

                        case "System.String":
                            arguments[index] = value;
                            break;

                        default:
                            throw new Exception($"Unhandled parameter type: {parameter.ParameterType.FullName}");
                    }
                }

                yield return (T)constructor.Invoke(arguments);
            }
        }
    }
}
