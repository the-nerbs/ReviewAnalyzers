using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ReviewAnalyzers;
using ReviewAnalyzers.Test.Helpers;
using ReviewAnalyzers.Properties;
using MSTestExtensions;

using static ReviewAnalyzers.Test.Factories;

namespace ReviewAnalyzers.Test
{
    [TestClass]
    public class EmptyOrMissingSummaryTests : DiagnosticVerifier
    {
        [TestMethod]
        public void TestNoSourceGivesNoWarnings()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }


        [CombinatorialTestMethod]
        public void TestGlobalType(
            [StandardFactory(AllObjectTypes)] string type,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo accessLevel)
        {
            string source = $@"
{accessLevel.Keyword} {type} NoSummary {{ }}

/// <summary>
/// 
/// </summary>
{accessLevel.Keyword} {type} EmptySummary {{ }}

/// <summary>
/// no diagnostic here!
/// </summary>
{accessLevel.Keyword} {type} YesSummary {{ }}
";

            int expectedColumn = accessLevel.Keyword.Length + 1 + type.Length + 2;

            if (accessLevel.RequiresSummary)
            {
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, type, "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", 2, expectedColumn)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, type, "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", 7, expectedColumn)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }

        [CombinatorialTestMethod]
        public void TestNestedType(
            [StandardFactory(AllObjectTypes)] string type,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo accessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public class Outer
{{
    {accessLevel.Keyword} {type} NoSummary {{ }}

    /// <summary>
    /// 
    /// </summary>
    {accessLevel.Keyword} {type} EmptySummary {{ }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {accessLevel.Keyword} {type} YesSummary {{ }}
}}
";

            int expectedColumn = 4 + accessLevel.Keyword.Length + 1 + type.Length + 2;

            if (accessLevel.RequiresSummary)
            {
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, type, "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", 7, expectedColumn)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, type, "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", 12, expectedColumn)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestClassAndStructMethods(
            [StandardFactory(ClassTypes)] string keyword,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo typeAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo methodAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{typeAccessLevel.Keyword} {keyword} Program
{{
    {methodAccessLevel.Keyword} void NoSummary() {{ }}

    /// <summary>
    /// 
    /// </summary>
    {methodAccessLevel.Keyword} void EmptySummary() {{ }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {methodAccessLevel.Keyword} void YesSummary() {{ }}
}}";

            if (typeAccessLevel.RequiresSummary && methodAccessLevel.RequiresSummary)
            {
                int column = 4 + methodAccessLevel.Keyword.Length + 7;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "method", "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, column)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "method", "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }

        [CombinatorialTestMethod]
        public void TestInterfaceMethod(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo typeAccessLevel)
        {
            string source = @"
/// <summary>
/// no diagnostic here!
/// </summary>
public interface Program
{
    void NoSummary();

    /// <summary>
    /// 
    /// </summary>
    void EmptySummary();

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    void YesSummary();
}";

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, "method", "NoSummary"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, 10)
                    },
                },
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.EmptySummary_MessageFmt, "method", "EmptySummary"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, 10)
                    },
                });
        }


        [CombinatorialTestMethod]
        public void TestClassAndStructConstructor(
            [StandardFactory(ClassTypes)] string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public {keyword} Program
{{
    public Program(int noSummary) {{  }}

    /// <summary>
    /// 
    /// </summary>
    public Program(double emptySummary) {{  }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    public Program(string yesSummary) {{  }}
}}";

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, "constructor", "Program"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, 12)
                    },
                },
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.EmptySummary_MessageFmt, "constructor", "Program"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, 12)
                    },
                });
        }



        [CombinatorialTestMethod]
        public void TestClassAndStructConversionOperator(
            [StandardFactory(ClassTypes)] string keyword,
            [StandardFactory(ExplicitImplicit)] string explicitness)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public {keyword} Program
{{
    public static {explicitness} operator int(Program noSummary) {{ return 0; }}

    /// <summary>
    /// 
    /// </summary>
    public static {explicitness} operator double(Program emptySummary) {{ return 0; }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    public static {explicitness} operator string(Program yesSummary) {{ return 0; }}
}}";

            int column = 18 + explicitness.Length + 2;

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, string.Empty, "operator int"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, column)
                    },
                },
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.EmptySummary_MessageFmt, string.Empty, "operator double"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, column)
                    },
                });
        }


        [CombinatorialTestMethod]
        public void TestMembersOfGlobalEnum(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo accessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{accessLevel.Keyword} enum Program
{{
    NoSummary,

    /// <summary>
    /// 
    /// </summary>
    EmptySummary,

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    YesSummary,
}}";

            if (accessLevel.RequiresSummary)
            {

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "enum member", "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, 5)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "enum member", "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, 5)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }

        [CombinatorialTestMethod]
        public void TestMembersOfNestedEnum(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo enumAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {enumAccessLevel.Keyword} enum Program
    {{
        NoSummary,

        /// <summary>
        /// 
        /// </summary>
        EmptySummary,

        /// <summary>
        /// no diagnostic here!
        /// </summary>
        YesSummary,
    }}
}}";

            if (outerAccessLevel.RequiresSummary && enumAccessLevel.RequiresSummary)
            {
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "enum member", "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, 9)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "enum member", "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 17, 9)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestEventPropertyDeclaration(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    {eventAccessLevel.Keyword} event EventHandler NoSummary
    {{
        add {{ }}
        remove {{ }}
    }}

    /// <summary>
    /// 
    /// </summary>
    {eventAccessLevel.Keyword} event EventHandler EmptySummary
    {{
        add {{ }}
        remove {{ }}
    }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {eventAccessLevel.Keyword} event EventHandler YesSummary
    {{
        add {{ }}
        remove {{ }}
    }}
}}";

            if (outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int column = 4 + eventAccessLevel.Keyword.Length + 21;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "event", "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, column)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "event", "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 16, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestEventFieldDeclaration(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    {eventAccessLevel.Keyword} event EventHandler NoSummary;

    /// <summary>
    /// 
    /// </summary>
    {eventAccessLevel.Keyword} event EventHandler EmptySummary;

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {eventAccessLevel.Keyword} event EventHandler YesSummary;
}}";

            if (outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int column = 4 + eventAccessLevel.Keyword.Length + 21;
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "event", "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, column)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "event", "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestFieldDeclaration(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    {eventAccessLevel.Keyword} int NoSummary;

    /// <summary>
    /// 
    /// </summary>
    {eventAccessLevel.Keyword} int EmptySummary;

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {eventAccessLevel.Keyword} int YesSummary;
}}";

            if (outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int column = 4 + eventAccessLevel.Keyword.Length + 6;
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "field", "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, column)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "field", "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestIndexerDeclaration(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    {eventAccessLevel.Keyword} int this[int noSummary] {{ set {{ }} }}

    /// <summary>
    /// 
    /// </summary>
    {eventAccessLevel.Keyword} int this[double emptySummary] {{ set {{ }} }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {eventAccessLevel.Keyword} int this[string yesSummary] {{ set {{ }} }}
}}";

            if (outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int column = 4 + eventAccessLevel.Keyword.Length + 6;
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "indexer", "this"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, column)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "indexer", "this"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestMethodDeclaration(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    {eventAccessLevel.Keyword} void NoSummary() {{ }}

    /// <summary>
    /// 
    /// </summary>
    {eventAccessLevel.Keyword} void EmptySummary() {{ }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {eventAccessLevel.Keyword} void YesSummary() {{ }}
}}";

            if (outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int column = 4 + eventAccessLevel.Keyword.Length + 7;
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "method", "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, column)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "method", "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestUnaryOperatorDeclaration(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(OverloadableUnaryOperators)] OperatorInfo operatorInfo)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    public static bool operator {operatorInfo.Token}(Outer noSummary) {{ return false; }}
}}

/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer2
{{
    /// <summary>
    /// 
    /// </summary>
    public static bool operator {operatorInfo.Token}(Outer2 emptySummary) {{ return false; }}
}}

/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer2
{{
    /// <summary>
    /// no diagnostic here!
    /// </summary>
    public static bool operator {operatorInfo.Token}(Outer2 yesSummary) {{ return false; }}
}}";

            if (outerAccessLevel.RequiresSummary)
            {
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "operator", operatorInfo.Token),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, 24)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "operator", operatorInfo.Token),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 18, 24)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestBinaryOperatorDeclaration(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(OverloadableBinaryOperators)] OperatorInfo operatorInfo)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    public static bool operator {operatorInfo.Token}(Outer noSummary, int i) {{ return false; }}

    /// <summary>
    /// 
    /// </summary>
    public static bool operator {operatorInfo.Token}(Outer emptySummary, double i) {{ return false; }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    public static bool operator {operatorInfo.Token}(Outer yesSummary, string i) {{ return false; }}
}}";

            if (outerAccessLevel.RequiresSummary)
            {
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "operator", operatorInfo.Token),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, 24)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "operator", operatorInfo.Token),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, 24)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestPropertyDeclaration(
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo propertyAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} class Outer
{{
    {propertyAccessLevel.Keyword} int NoSummary {{ get {{ return 0; }} }}

    /// <summary>
    /// 
    /// </summary>
    {propertyAccessLevel.Keyword} int EmptySummary {{ get {{ return 0; }} }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {propertyAccessLevel.Keyword} int YesSummary {{ get {{ return 0; }} }}
}}";

            if (outerAccessLevel.RequiresSummary && propertyAccessLevel.RequiresSummary)
            {
                int column = 4 + propertyAccessLevel.Keyword.Length + 6;
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.MissingSummary_MessageFmt, "property", "NoSummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, column)
                        },
                    },
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(Resources.EmptySummary_MessageFmt, "property", "EmptySummary"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 12, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new EmptyOrMissingSummaryAnalyzer();
        }
    }
}
