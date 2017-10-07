using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAnalyzers.Test.Helpers
{
    /// <summary>
    /// Attribute used to describe a set of values to pass for an argument to a combinatorial test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CombinatorialEnumArgument : CombinatorialArgumentAttribute
    {
        private readonly Lazy<IReadOnlyList<object>> _values;


        /// <summary>
        /// Gets the enumeration type from which the values are generated.
        /// </summary>
        public Type EnumType { get; }

        /// <inheritdoc />
        public override IReadOnlyList<object> Values
        {
            get { return _values.Value; }
        }


        /// <summary>
        /// Constructs a new instance of <see cref="CombinatorialEnumArgument"/>.
        /// </summary>
        /// <param name="argIndex">The index of the argument this is for.</param>
        /// <param name="enumType">The enumeration type to generate values from.</param>
        public CombinatorialEnumArgument(int argIndex, Type enumType)
            : base(argIndex)
        {
            _values = new Lazy<IReadOnlyList<object>>(GenerateValues);

            EnumType = enumType;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="CombinatorialEnumArgument"/>.
        /// </summary>
        /// <param name="argName">The name of the argument this is for.</param>
        /// <param name="enumType">The enumeration type to generate values from.</param>
        public CombinatorialEnumArgument(string argName, Type enumType)
            : base(argName)
        {
            _values = new Lazy<IReadOnlyList<object>>(GenerateValues);

            EnumType = enumType;
        }


        private IReadOnlyList<object> GenerateValues()
        {
            if (!EnumType.IsEnum)
            {
                throw new ArgumentException("Type is not an enumeration.", nameof(EnumType));
            }

            return Enum.GetValues(EnumType)
                       .Cast<object>()
                       .ToArray();
        }
    }
}
