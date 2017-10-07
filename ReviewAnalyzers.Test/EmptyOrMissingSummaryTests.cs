using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ReviewAnalyzers;
using ReviewAnalyzers.Test.Helpers;
using ReviewAnalyzers.Properties;

namespace ReviewAnalyzers.Test
{
    [TestClass]
    public class EmptyOrMissingSummaryTests : DiagnosticVerifier
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void TestNoSourceGivesNoWarnings()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }


        [DataTestMethod]
        [DataRow("class")]
        [DataRow("interface")]
        [DataRow("struct")]
        [DataRow("enum")]
        public void TestPublicTypeNoSummary(string keyword)
        {
            string source = $@"
public {keyword} Program
{{ }}
";

            int expectedColumn = 9 + keyword.Length;

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, keyword, "Program"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 2, expectedColumn)
                    },
                });
        }

        [DataTestMethod]
        [DataRow("class")]
        [DataRow("interface")]
        [DataRow("struct")]
        [DataRow("enum")]
        public void TestNonPublicTypeNoSummary(string keyword)
        {
            string source = $@"
internal {keyword} Program
{{ }}
";

            // no diagnostics since the type isn't public
            VerifyCSharpDiagnostic(source);
        }

        [DataTestMethod]
        [DataRow("class")]
        [DataRow("interface")]
        [DataRow("struct")]
        [DataRow("enum")]
        public void TestDefaultAccessibilityTypeNoSummary(string keyword)
        {
            string source = $@"
{keyword} Program
{{ }}
";

            // no diagnostics since the type isn't public
            VerifyCSharpDiagnostic(source);
        }


        [DataTestMethod]
        [DataRow("class")]
        [DataRow("interface")]
        [DataRow("struct")]
        [DataRow("enum")]
        public void TestNestedPublicTypeNoSummary(string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public class Outer
{{
    public {keyword} Nested
    {{ }}
}}
";

            int expectedColumn = 13 + keyword.Length;

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, keyword, "Nested"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, expectedColumn)
                    },
                });
        }

        [DataTestMethod]
        [DataRow("class")]
        [DataRow("interface")]
        [DataRow("struct")]
        [DataRow("enum")]
        public void TestNestedProtectedTypeNoSummary(string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public class Outer
{{
    protected {keyword} Nested
    {{ }}
}}
";

            int expectedColumn = 16 + keyword.Length;

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, keyword, "Nested"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, expectedColumn)
                    },
                });
        }

        [DataTestMethod]
        [DataRow("class")]
        [DataRow("interface")]
        [DataRow("struct")]
        [DataRow("enum")]
        public void TestNestedProtectedInternalTypeNoSummary(string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public class Outer
{{
    protected internal {keyword} Nested
    {{ }}
}}
";

            int expectedColumn = 25 + keyword.Length;

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, keyword, "Nested"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, expectedColumn)
                    },
                });
        }

        [DataTestMethod]
        [DataRow("class")]
        [DataRow("interface")]
        [DataRow("struct")]
        [DataRow("enum")]
        public void TestNestedInternalTypeNoSummary(string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public class Outer
{{
    internal {keyword} Nested
    {{ }}
}}
";

            int expectedColumn = 15 + keyword.Length;

            VerifyCSharpDiagnostic(source);
        }

        [DataTestMethod]
        [DataRow("class")]
        [DataRow("interface")]
        [DataRow("struct")]
        [DataRow("enum")]
        public void TestNestedPrivateTypeNoSummary(string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public class Outer
{{
    private {keyword} Nested
    {{ }}
}}
";

            int expectedColumn = 13 + keyword.Length;

            VerifyCSharpDiagnostic(source);
        } 


        [TestMethod]
        [DataRow("class")]
        [DataRow("struct")]
        public void TestClassAndStructMethods(string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public {keyword} Program
{{
    public void NoSummaryPublic() {{ }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    public void YesSummaryPublic() {{ }}

    protected void NoSummaryProtected() {{ }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    protected void YesSummaryProtected() {{ }}

    protected internal void NoSummaryProtectedInternal() {{ }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    protected internal void YesSummaryProtectedInternal() {{ }}

    internal void NoSummaryInternal() {{ }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    internal void YesSummaryInternal() {{ }}

    private void NoSummaryPrivate() {{ }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    private void YesSummaryPrivate() {{ }}
}}";

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, "method", "NoSummaryPublic"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, 17)
                    },
                },
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, "method", "NoSummaryProtected"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 14, 20)
                    },
                },
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, "method", "NoSummaryProtectedInternal"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 21, 29)
                    },
                });
        }

        [TestMethod]
        public void TestInterfaceMethod()
        {
            string source = @"
/// <summary>
/// no diagnostic here!
/// </summary>
public interface Program
{
    void NoSummary();

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
                });
        }

        [DataTestMethod]
        [DataRow("class")]
        [DataRow("struct")]
        public void TestClassAndStructConstructor(string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public {keyword} Program
{{
    public Program(int noSummary) {{  }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    public Program(double yesSummary) {{  }}
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
                });
        }


        [DataTestMethod]
        [DataRow("class")]
        [DataRow("struct")]
        public void TestClassAndStructConversionOperator(string keyword)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public {keyword} Program
{{
    public static implicit operator int(Program x) {{ return 0; }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    public static implicit operator double(Program x) {{ return 0; }}

    public static explicit operator bool(Program x) {{ return 0; }}

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    public static explicit operator string(Program x) {{ return 0; }}
}}";

            VerifyCSharpDiagnostic(source,
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, string.Empty, "operator int"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 7, 28)
                    },
                },
                new DiagnosticResult
                {
                    Id = "REVIEW" + ((int)DiagnosticId.MissingOrEmptySummary).ToString("D5"),
                    Message = string.Format(Resources.MissingSummary_MessageFmt, string.Empty, "operator bool"),
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] {
                        new DiagnosticResultLocation("Test0.cs", 14, 28)
                    },
                });
        }

        
        [DataTestMethod]
        [DataRow("public", true)]
        [DataRow("internal", false)]
        [DataRow("", false)]
        public void TestMembersOfGlobalEnum(string accessLevel, bool expectsDiagnostic)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
{accessLevel} enum Program
{{
    NoSummary,

    /// <summary>
    /// no diagnostic here!
    /// </summary>
    YesSummary,
}}";

            if (expectsDiagnostic)
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
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }

        [DataTestMethod]
        [DataRow("public", true)]
        [DataRow("protected", true)]
        [DataRow("protected internal", true)]
        [DataRow("internal", false)]
        [DataRow("private", false)]
        [DataRow("", false)]
        public void TestMembersOfNestedEnum(string accessLevel, bool expectsDiagnostic)
        {
            string source = $@"
/// <summary>
/// no diagnostic here!
/// </summary>
public class Outer
{{
    /// <summary>
    /// no diagnostic here!
    /// </summary>
    {accessLevel} enum Program
    {{
        NoSummary,

        /// <summary>
        /// no diagnostic here!
        /// </summary>
        YesSummary,
    }}
}}";

            if (expectsDiagnostic)
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
                    });
            }
            else
            {
                VerifyCSharpDiagnostic(source);
            }
        }


        [CombinatorialTestMethod]
        [CombinatorialArgument("i", 0, 1, 2)]
        [CombinatorialEnumArgument(1, typeof(DiagnosticSeverity))]
        public void TestingThings(int i, DiagnosticSeverity j)
        {
            TestContext.WriteLine($"i = {i};  j = {j}");
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new EmptyOrMissingSummaryAnalyzer();
        }
    }
}