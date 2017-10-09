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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(AllObjectTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo accessLevel)
        {
            string source = $@"
{summary.Text}
{accessLevel.Keyword} {outerType} Test {{ }}";

            if (summary.ExpectsDiagnostic && accessLevel.RequiresSummary)
            {
                int line = 2 + summary.LineLength + 1;
                int column = accessLevel.Keyword.Length + 1 + outerType.Length + 2;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, outerType, "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(AllObjectTypes)] string outerType,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo accessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public class Outer
{{
    {summary.Text}
    {accessLevel.Keyword} {outerType} Test {{ }}
}}";

            if (summary.ExpectsDiagnostic && accessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;
                int column = 4 + accessLevel.Keyword.Length + 1 + outerType.Length + 2;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, outerType, "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestClassAndStructConstructor(
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public {outerType} Test
{{
    {summary.Text}
    public Test() {{ }}
}}";
            if (summary.ExpectsDiagnostic)
            {
                int line = 7 + summary.LineLength + 1;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "constructor", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, 12)
                        },
                    });
            }
        }


        [CombinatorialTestMethod]
        public void TestClassAndStructConversionOperator(
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(ExplicitImplicit)] string explicitness)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public {outerType} Program
{{
    {summary.Text}
    public static {explicitness} operator int(Program noSummary) {{ return 0; }}
}}";

            if (summary.ExpectsDiagnostic)
            {
                int line = 7 + summary.LineLength + 1;
                int column = 18 + explicitness.Length + 2;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, string.Empty, "operator int"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        },
                    });
            }
        }


        [CombinatorialTestMethod]
        public void TestMembersOfGlobalEnum(
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo accessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{accessLevel.Keyword} enum Program
{{
    {summary.Text}
    Test,
}}";

            if (summary.ExpectsDiagnostic && accessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "enum member", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, 5)
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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo enumAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} {outerType} Outer
{{
    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {enumAccessLevel.Keyword} enum Program
    {{
        {summary.Text}
        Test,
    }}
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary && enumAccessLevel.RequiresSummary)
            {
                int line = 12 + summary.LineLength + 1;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "enum member", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, 9)
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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} {outerType} Outer
{{
    {summary.Text}
    {eventAccessLevel.Keyword} event EventHandler Test
    {{
        add {{ }}
        remove {{ }}
    }}
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;
                int column = 4 + eventAccessLevel.Keyword.Length + 21;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "event", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} {outerType} Outer
{{
    {summary.Text}
    {eventAccessLevel.Keyword} event EventHandler Test;
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;
                int column = 4 + eventAccessLevel.Keyword.Length + 21;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "event", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} {outerType} Outer
{{
    {summary.Text}
    {eventAccessLevel.Keyword} int Test;
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;
                int column = 4 + eventAccessLevel.Keyword.Length + 6;
                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "field", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo eventAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} {outerType} Outer
{{
    {summary.Text}
    {eventAccessLevel.Keyword} int this[int test] {{ set {{ }} }}
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary && eventAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;
                int column = 4 + eventAccessLevel.Keyword.Length + 6;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "indexer", "this"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestClassAndStructMethodDeclaration(
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo typeAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo methodAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{typeAccessLevel.Keyword} {outerType} Program
{{
    {summary.Text}
    {methodAccessLevel.Keyword} void Test() {{ }}
}}";

            if (summary.ExpectsDiagnostic && typeAccessLevel.RequiresSummary && methodAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;
                int column = 4 + methodAccessLevel.Keyword.Length + 7;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "method", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestInterfaceMethodDeclaration(
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo typeAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public interface Program
{{
    {summary.Text}
    void Test();
}}";

            if (summary.ExpectsDiagnostic)
            {
                int line = 7 + summary.LineLength + 1;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "method", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", line, 10)
                        },
                    });
            }
        }


        [CombinatorialTestMethod]
        public void TestUnaryOperatorDeclaration(
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(OverloadableUnaryOperators)] OperatorInfo operatorInfo)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} {outerType} Outer
{{
    {summary.Text}
    public static bool operator {operatorInfo.Token}(Outer noSummary) {{ return false; }}
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "operator", operatorInfo.Token),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, 24)
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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(OverloadableBinaryOperators)] OperatorInfo operatorInfo)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} {outerType} Outer
{{
    {summary.Text}
    public static bool operator {operatorInfo.Token}(Outer noSummary, int i) {{ return false; }}
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "operator", operatorInfo.Token),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, 24)
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
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(ClassTypes)] string outerType,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel,
            [StandardFactory(MemberAccessLevels)] AccessLevelInfo propertyAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} {outerType} Outer
{{
    {summary.Text}
    {propertyAccessLevel.Keyword} int Test {{ get {{ return 0; }} }}
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary && propertyAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;
                int column = 4 + propertyAccessLevel.Keyword.Length + 6;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "property", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        },
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        public void TestInterfacePropertyDeclaration(
            [StandardFactory(SummaryComments)] SummaryComment summary,
            [StandardFactory(GlobalAccessLevels)] AccessLevelInfo outerAccessLevel)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{outerAccessLevel.Keyword} interface Outer
{{
    {summary.Text}
    int Test {{ get; }}
}}";

            if (summary.ExpectsDiagnostic && outerAccessLevel.RequiresSummary)
            {
                int line = 7 + summary.LineLength + 1;

                VerifyCSharpDiagnostic(source,
                    new DiagnosticResult
                    {
                        Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                        Message = string.Format(summary.ExpectedMessageFmt, "property", "Test"),
                        Severity = DiagnosticSeverity.Warning,
                        Locations = new[] {
                            new DiagnosticResultLocation("Test0.cs", line, 9)
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
