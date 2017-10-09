using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAnalyzers.Test
{
    internal static class Factories
    {
        public const string AllObjectTypes              = nameof(AllObjectTypesFactory);
        public const string ClassTypes                  = nameof(ClassTypesFactory);
        public const string GlobalAccessLevels          = nameof(GlobalAccessLevelsFactory);
        public const string MemberAccessLevels          = nameof(MemberAccessLevelsFactory);
        public const string OverloadableOperators       = nameof(OverloadableOperatorsFactory);
        public const string OverloadableUnaryOperators  = nameof(OverloadableUnaryOperatorsFactory);
        public const string OverloadableBinaryOperators = nameof(OverloadableBinaryOperatorsFactory);
        public const string ExplicitImplicit            = nameof(ExplicitImplicitFactory);
        public const string SummaryComments             = nameof(SummaryCommentsFactory);

        public static string[] AllObjectTypesFactory()
        {
            return new[] {
                "class",
                "struct",
                "interface",
                "enum"
            };
        }

        public static string[] ClassTypesFactory()
        {
            return new[] {
                "class",
                "struct",
            };
        }

        public static AccessLevelInfo[] GlobalAccessLevelsFactory()
        {
            return new[] {
                new AccessLevelInfo("public", true),
                new AccessLevelInfo("internal", false),
                new AccessLevelInfo("", false)
            };
        }

        public static AccessLevelInfo[] MemberAccessLevelsFactory()
        {
            return new[] {
                new AccessLevelInfo("public", true),
                new AccessLevelInfo("protected", true),
                new AccessLevelInfo("protected internal", true),
                new AccessLevelInfo("internal", false),
                new AccessLevelInfo("private", false),
                new AccessLevelInfo("", false)
            };
        }

        public static OperatorInfo[] OverloadableOperatorsFactory()
        {
            return new[] {
                new OperatorInfo("+", 1),
                new OperatorInfo("-", 1),
                new OperatorInfo("!", 1),
                new OperatorInfo("~", 1),
                new OperatorInfo("++", 1),
                new OperatorInfo("--", 1),
                new OperatorInfo("true", 1),        // yes, these are operators you can overload.
                new OperatorInfo("false", 1),       // no, I don't understand them :\

                new OperatorInfo("+", 2),
                new OperatorInfo("-", 2),
                new OperatorInfo("*", 2),
                new OperatorInfo("/", 2),
                new OperatorInfo("%", 2),
                new OperatorInfo("&", 2),
                new OperatorInfo("|", 2),
                new OperatorInfo("^", 2),
                new OperatorInfo("<<", 2),
                new OperatorInfo(">>", 2),
                //new OperatorInfo("==", 2),    // note: cannot overload only one or the other, so
                //new OperatorInfo("!=", 2),    // these need to be tested as a special case.
                new OperatorInfo("<", 2),
                new OperatorInfo("<=", 2),
                new OperatorInfo(">", 2),
                new OperatorInfo(">=", 2),
            };
        }

        public static OperatorInfo[] OverloadableUnaryOperatorsFactory()
        {
            return OverloadableOperatorsFactory()
                .Where(op => op.OperandCount == 1)
                .ToArray();
        }

        public static OperatorInfo[] OverloadableBinaryOperatorsFactory()
        {
            return OverloadableOperatorsFactory()
                .Where(op => op.OperandCount == 2)
                .ToArray();
        }

        public static string[] ExplicitImplicitFactory()
        {
            return new[] { "explicit", "implicit" };
        }

        private static SummaryComment[] SummaryCommentsFactory()
        {
            return new[] {
                // has an actual summary comment
                new SummaryComment("/// <summary> no diagnostic here! </summary>", isMissing: false, isEmpty: false),

                // OK alternative - Doxygen inherit from overridden member (base not verified!)
                new SummaryComment("/// <inheritdoc />", isMissing: false, isEmpty: false),

                // OK alternative - references a base member (base not verified!)
                new SummaryComment("/// <see cref=\"BaseClass.Member\">", isMissing: false, isEmpty: false),

                // invalid - no summary comment
                new SummaryComment(string.Empty, isMissing: true, isEmpty: false),

                // invalid - not a documentation comment
                new SummaryComment("// <summary> only 2 slashes </summary>", isMissing: true, isEmpty: false),

                // invalid - null summary comment
                new SummaryComment("/// <summary />", isMissing: false, isEmpty: true),

                // invalid - empty summary comment
                new SummaryComment("/// <summary></summary>", isMissing: false, isEmpty: true),

                // invalid - whitespace-only summary comment
                new SummaryComment("/// <summary>     </summary>", isMissing: false, isEmpty: true),
            };
        }
    }
}
