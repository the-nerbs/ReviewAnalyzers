using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReviewAnalyzers.Test.Helpers
{
    /// <summary>
    /// Attribute for a combinatorial data test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CombinatorialTestMethodAttribute : TestMethodAttribute
    {
        /// <inheritdoc />
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            try
            {
                var attrs = testMethod.GetAttributes<CombinatorialArgumentAttribute>(inherit: true);

                ValidateArguments(testMethod.MethodInfo, attrs);

                object[][] args = GetArgumentsInOrder(testMethod.MethodInfo, attrs);

                ValidateArgumentTypes(testMethod.MethodInfo, args);

                var indexes = new int[args.Length];
                bool done;

                var results = new List<TestResult>();
                do
                {
                    // run the test with the current values.
                    var argsToPass = Enumerable.Range(0, args.Length)
                                               .Select(i => args[i][indexes[i]])
                                               .ToArray();

                    var result = testMethod.Invoke(argsToPass);
                    result.DisplayName = GetTestDisplayName(testMethod, argsToPass);
                    results.Add(result);

                    // update the current indexes.
                    int updateIdx = indexes.Length - 1;

                    for (; updateIdx >= 0; updateIdx--)
                    {
                        indexes[updateIdx]++;

                        if (indexes[updateIdx] < args[updateIdx].Length)
                        {
                            // still values left for this parameter.
                            break;
                        }

                        // carry to the next slot
                        indexes[updateIdx] = 0;
                    }

                    // If we carried off the end of the array, we're done
                    done = (updateIdx < 0);

                } while (!done);

                return results.ToArray();
            }
            catch (Exception ex)
            {
                return new[]
                {
                    new TestResult
                    {
                        Outcome = UnitTestOutcome.Error,
                        TestFailureException = ex
                    }
                };
            }
        }


        private static void ValidateArguments(MethodInfo method, CombinatorialArgumentAttribute[] attributes)
        {
            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length != attributes.Length)
            {
                throw new Exception(
                    $"Count of {nameof(CombinatorialArgumentAttribute)}s does not match number of parameters."
                );
            }

            var matched = new bool[parameters.Length];
            foreach (var attr in attributes)
            {
                int index;

                if (attr.ArgumentIndex != null)
                {
                    index = attr.ArgumentIndex.Value;
                    if (index < 0 || index >= parameters.Length)
                    {
                        throw new Exception(
                            $"{index} is not a valid parameter index."
                        );
                    }
                }
                else
                {
                    index = Array.FindIndex(parameters, p => p.Name == attr.ArgumentName);
                    if (index == -1)
                    {
                        throw new Exception(
                            $"Test method does not have an argument named {attr.ArgumentName}."
                        );
                    }
                }

                if (matched[index])
                {
                    throw new Exception(
                        $"Argument {index} has duplicate combinatorial data."
                    );
                }

                matched[index] = true;
            }

            for (int i = 0; i < matched.Length; i++)
            {
                if (!matched[i])
                {
                    throw new Exception(
                        $"Argument {parameters[i].Name} is missing a {nameof(CombinatorialArgumentAttribute)}."
                    );
                }
            }
        }

        private static object[][] GetArgumentsInOrder(MethodInfo method, CombinatorialArgumentAttribute[] args)
        {
            var argsAndOrder = new Tuple<int, object[]>[args.Length];

            ParameterInfo[] parameters = method.GetParameters();

            for (int i = 0; i < args.Length; i++)
            {
                int index = args[i].ArgumentIndex
                         ?? Array.FindIndex(parameters, p => p.Name == args[i].ArgumentName);

                argsAndOrder[i] = Tuple.Create(index, args[i].Values.ToArray());
            }

            return argsAndOrder
                .OrderBy(tpl => tpl.Item1)
                .Select(tpl => tpl.Item2)
                .ToArray();
        }

        private static void ValidateArgumentTypes(MethodInfo method, object[][] arguments)
        {
            ParameterInfo[] allParameters = method.GetParameters();

            for (int i = 0; i < arguments.Length; i++)
            {
                object[] values = arguments[i];
                ParameterInfo parameter = allParameters[i];

                for (int j = 0; j < values.Length; j++)
                {
                    object v = values[i];

                    if (!parameter.ParameterType.IsAssignableFrom(v.GetType()))
                    {
                        throw new ArgumentException(
                            $"Argument value ({GetValueString(v)}) has a type that is incompatible with parameter {parameter.Name}."
                        );
                    }
                }
            }
        }


        private static string GetTestDisplayName(ITestMethod method, object[] arguments)
        {
            StringBuilder name = new StringBuilder(method.TestMethodName);
            name.Append('(');

            bool first = true;

            foreach (var value in arguments)
            {
                if (!first)
                {
                    name.Append(", ");
                }

                name.Append(GetValueString(value));

                first = false;
            }

            name.Append(')');
            return name.ToString();
        }

        private static string GetValueString(object value)
        {
            switch (value)
            {
                case null:
                    return "null";

                case string str:
                    return "\"" + str + "\"";

                case object enumVal
                when enumVal.GetType().IsEnum:
                    return enumVal.GetType().Name + "." + value;

                default:
                    return value.ToString();
            }
        }
    }
}
