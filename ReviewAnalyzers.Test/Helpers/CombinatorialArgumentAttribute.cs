using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAnalyzers.Test.Helpers
{
    /// <summary>
    /// Attribute used to describe a set of values to pass for an argument to a combinatorial test.
    /// This uses the values that are passed to the constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CombinatorialArgumentAttribute : Attribute
    {
        /// <summary>
        /// Gets the index of the argument this is for. If null, the argument is specified by name.
        /// </summary>
        public int? ArgumentIndex { get; }

        /// <summary>
        /// Gets the name of the argument this is for. If null, the argument is specified by index.
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// Gets the list of values to pass for the argument.
        /// </summary>
        /// <remarks>
        /// When creating an extension to this attribute, if generating the list of values may
        /// result in an exception, then it should be done when this property is invoked as
        /// opposed to in the constructor as the execution engine may skip the test if the
        /// constructor throws an exception.
        /// </remarks>
        public virtual IReadOnlyList<object> Values { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="CombinatorialArgumentAttribute"/>.
        /// </summary>
        /// <param name="argIndex">The index of the argument this is for.</param>
        /// <param name="values">The set of values to pass for the argument.</param>
        public CombinatorialArgumentAttribute(int argIndex, params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length == 0)
                throw new ArgumentException("expected values", nameof(values));

            ArgumentIndex = argIndex;
            ArgumentName = null;
            Values = values;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CombinatorialArgumentAttribute"/>.
        /// </summary>
        /// <param name="argName">The name of the argument this is for.</param>
        /// <param name="values">The set of values to pass for the argument.</param>
        public CombinatorialArgumentAttribute(string argName, params object[] values)
        {
            if (argName == null)
                throw new ArgumentNullException(nameof(argName));

            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length == 0)
                throw new ArgumentException("expected values", nameof(values));


            ArgumentIndex = null;
            ArgumentName = argName;
            Values = values;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CombinatorialArgumentAttribute"/>.
        /// This overload is to support overriding the Values property.
        /// </summary>
        /// <param name="argIndex">The index of the argument this is for.</param>
        protected CombinatorialArgumentAttribute(int argIndex)
        {
            ArgumentIndex = argIndex;
            ArgumentName = null;
            Values = null;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CombinatorialArgumentAttribute"/>.
        /// This overload is to support overriding the Values property.
        /// </summary>
        /// <param name="argName">The name of the argument this is for.</param>
        protected CombinatorialArgumentAttribute(string argName)
        {
            if (argName == null)
                throw new ArgumentNullException(nameof(argName));

            ArgumentIndex = null;
            ArgumentName = argName;
            Values = null;
        }
    }
}
