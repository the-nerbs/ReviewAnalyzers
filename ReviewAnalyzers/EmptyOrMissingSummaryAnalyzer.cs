using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using ReviewAnalyzers.Properties;

namespace ReviewAnalyzers
{
    /// <summary>
    /// Analyzer which checks for empty or missing <![CDATA[<summary>]]> documentation comments on
    /// types and members that are publicly visible.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EmptyOrMissingSummaryAnalyzer : BaseAnalyzer
    {
        internal const DiagnosticId ID = DiagnosticId.MissingOrEmptySummary;


        private static DiagnosticDescriptor MissingRule =
            new DiagnosticDescriptor(
                id: CreateIdString(ID),
                title: FromResource(nameof(Resources.MissingSummary_Title)),
                messageFormat: FromResource(nameof(Resources.MissingSummary_MessageFmt)),
                category: ID.GetCategory(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: FromResource(nameof(Resources.MissingSummary_Description))
            );

        private static DiagnosticDescriptor EmptyRule =
            new DiagnosticDescriptor(
                id: CreateIdString(ID),
                title: FromResource(nameof(Resources.EmptySummary_Title)),
                messageFormat: FromResource(nameof(Resources.EmptySummary_MessageFmt)),
                category: ID.GetCategory(),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: FromResource(nameof(Resources.EmptySummary_Description))
            );


        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(MissingRule, EmptyRule); }
        }


        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeTypeDeclaration,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.InterfaceDeclaration,
                SyntaxKind.EnumDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration,
                SyntaxKind.ConstructorDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration,
                SyntaxKind.ConversionOperatorDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzeEnumMemberDeclaration,
                SyntaxKind.EnumMemberDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzeEventDeclaration,
                SyntaxKind.EventDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration,
                SyntaxKind.EventFieldDeclaration, SyntaxKind.FieldDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration,
                SyntaxKind.IndexerDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration,
                SyntaxKind.MethodDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration,
                SyntaxKind.OperatorDeclaration
            );

            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration,
                SyntaxKind.PropertyDeclaration
            );
        }


        private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseTypeDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;
                }
            }
        }

        private static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as ConstructorDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;
                }
            }
        }

        private static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as ConversionOperatorDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                string ident = node.OperatorKeyword.ToString() + " " + node.Type.ToString();

                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.OperatorKeyword.GetLocation(), ident);
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.OperatorKeyword.GetLocation(), ident);
                        break;
                }
            }
        }

        private static void AnalyzeEnumMemberDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as EnumMemberDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;
                }
            }
        }

        private static void AnalyzeEventDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as EventDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;
                }
            }
        }

        private static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as BaseFieldDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        foreach (var field in node.Declaration.Variables)
                        {
                            Report(context, EmptyRule, field.Identifier.GetLocation(), field.Identifier.ToString());
                        }
                        break;

                    default:
                    case SummaryState.Missing:
                        foreach (var field in node.Declaration.Variables)
                        {
                            Report(context, MissingRule, field.Identifier.GetLocation(), field.Identifier.ToString()); 
                        }
                        break;
                }
            }
        }

        private static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as IndexerDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.ThisKeyword.GetLocation(), node.ThisKeyword.ToString());
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.ThisKeyword.GetLocation(), node.ThisKeyword.ToString());
                        break;
                }
            }
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as MethodDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;
                }
            }
        }

        private static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as OperatorDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                string ident = node.OperatorToken.ToString();

                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.OperatorKeyword.GetLocation(), ident);
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.OperatorKeyword.GetLocation(), ident);
                        break;
                }
            }
        }

        private static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = context.Node as PropertyDeclarationSyntax;
            Debug.Assert(node != null);

            if (IsPublicOrProtected(node))
            {
                switch (AnalyzeSummary(node))
                {
                    case SummaryState.Valid:
                        // all's good - nothing to report.
                        break;

                    case SummaryState.Empty:
                        Report(context, EmptyRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;

                    default:
                    case SummaryState.Missing:
                        Report(context, MissingRule, node.Identifier.GetLocation(), node.Identifier.ToString());
                        break;
                }
            }
        }


        private static bool HasPublicOrProtectedModifier(SyntaxTokenList modifiers)
        {
            return modifiers.Any(SyntaxKind.PublicKeyword)
                || modifiers.Any(SyntaxKind.ProtectedKeyword);
        }

        private static bool IsPublicOrProtected(SyntaxNode node)
        {
            switch (node)
            {
                case ClassDeclarationSyntax classDecl:
                    if (!HasPublicOrProtectedModifier(classDecl.Modifiers))
                    {
                        return false;
                    }
                    goto default;

                case StructDeclarationSyntax structDecl:
                    if (!HasPublicOrProtectedModifier(structDecl.Modifiers))
                    {
                        return false;
                    }
                    goto default;

                case InterfaceDeclarationSyntax interfaceDecl:
                    if (!HasPublicOrProtectedModifier(interfaceDecl.Modifiers))
                    {
                        return false;
                    }
                    goto default;

                case EnumDeclarationSyntax enumDecl:
                    if (!HasPublicOrProtectedModifier(enumDecl.Modifiers))
                    {
                        return false;
                    }
                    goto default;

                    // note: includes method, ctor, finalizer, operator, conversion operator, ...
                case BaseMethodDeclarationSyntax methodDecl:
                    if (!HasPublicOrProtectedModifier(methodDecl.Modifiers) &&
                        methodDecl.Parent.Kind() != SyntaxKind.InterfaceDeclaration)
                    {
                        return false;
                    }
                    goto default;

                case BasePropertyDeclarationSyntax propertyDecl:
                    if (!HasPublicOrProtectedModifier(propertyDecl.Modifiers))
                    {
                        return false;
                    }
                    goto default;

                case BaseFieldDeclarationSyntax fieldDecl:
                    if (!HasPublicOrProtectedModifier(fieldDecl.Modifiers))
                    {
                        return false;
                    }
                    goto default;

                case EnumMemberDeclarationSyntax enumMemberDecl:
                    goto default;

                case CompilationUnitSyntax unit:
                    return true;

                default:
                    return IsPublicOrProtected(node.Parent);
            }
        }
        
        private static SummaryState AnalyzeSummary(SyntaxNode node)
        {
            var docComments = node.GetLeadingTrivia()
                                  .Select(t => t.GetStructure())
                                  .OfType<DocumentationCommentTriviaSyntax>()
                                  .FirstOrDefault();

            string tag, content;

            if (docComments == null ||
                !TryGetSummaryComment(docComments, out tag, out content))
            {
                // no summary or alternatives.
                return SummaryState.Missing;
            }

            if (tag == "summary" &&
                string.IsNullOrEmpty(content))
            {
                // summary tag with empty comment.
                return SummaryState.Empty;
            }

            // summary tag with content, or an alternative
            return SummaryState.Valid;
        }

        private static bool TryGetSummaryComment(DocumentationCommentTriviaSyntax docNode, out string tag, out string content)
        {
            bool foundTag = false;
            tag = null;
            content = null;

            // check elements with content
            var elements = docNode.Content
                                  .OfType<XmlElementSyntax>()
                                  .ToArray();

            if (elements.Length > 0)
            {
                // look for the summary tag first, and only if thats not found check for the alternatives
                var summaryElem = elements
                    .FirstOrDefault(el => el.StartTag.Name.ToString() == "summary");

                if (summaryElem != null)
                {
                    tag = summaryElem.StartTag.Name.ToString();
                    content = summaryElem.Content.ToString();
                    foundTag = true;
                }
                else
                {
                    var alternative = elements
                        .FirstOrDefault(el => SummaryAlternatives.Contains(el.StartTag.Name.ToString()));

                    if (alternative != null)
                    {
                        tag = alternative.StartTag.Name.ToString();
                        content = alternative.Content.ToString();
                        foundTag = true;
                    }
                }
            }

            if (!foundTag)
            {
                // check empty elements (eg: <tag />)
                var emptyElems = docNode.Content.OfType<XmlEmptyElementSyntax>().ToArray();

                if (emptyElems.Length > 0)
                {
                    var summaryElem = emptyElems
                        .FirstOrDefault(el => el.Name.ToString() == "summary");

                    if (summaryElem != null)
                    {
                        tag = summaryElem.Name.ToString();
                        foundTag = true;
                    }
                    else
                    {
                        var alternative = emptyElems
                            .FirstOrDefault(el => SummaryAlternatives.Contains(el.Name.ToString()));

                        if (alternative != null)
                        {
                            tag = alternative.Name.ToString();
                            foundTag = true;
                        }
                    }
                }
            }

            if (foundTag && content != null)
            {
                // replace any newline characters or comment leads (///) with a single space.
                content = Regex.Replace(content, @"(\r|\n|///)", " ").Trim();
            }

            return foundTag;
        }

        private static void Report(SyntaxNodeAnalysisContext context, DiagnosticDescriptor diagnostic,
                                   Location location, string elemIdent)
        {
            string elemName;
            if (!ElementNames.TryGetValue(context.Node.Kind(), out elemName))
            {
                elemName = "UNKNOWN";
            }

            context.ReportDiagnostic(
                Diagnostic.Create(diagnostic, location, elemName, elemIdent)
            );
        }


        private enum SummaryState
        {
            Valid,
            Empty,
            Missing
        }

        private static IReadOnlyDictionary<SyntaxKind, string> ElementNames =
            new Dictionary<SyntaxKind, string>
            {
                [SyntaxKind.ClassDeclaration]              = "class",
                [SyntaxKind.StructDeclaration]             = "struct",
                [SyntaxKind.InterfaceDeclaration]          = "interface",
                [SyntaxKind.EnumDeclaration]               = "enum",
                [SyntaxKind.ConstructorDeclaration]        = "constructor",
                [SyntaxKind.ConversionOperatorDeclaration] = string.Empty,
                [SyntaxKind.EnumMemberDeclaration]         = "enum member",
                [SyntaxKind.EventDeclaration]              = "event",
                [SyntaxKind.EventFieldDeclaration]         = "event",
                [SyntaxKind.FieldDeclaration]              = "field",
                [SyntaxKind.IndexerDeclaration]            = "indexer",
                [SyntaxKind.MethodDeclaration]             = "method",
                [SyntaxKind.OperatorDeclaration]           = "operator",
                [SyntaxKind.PropertyDeclaration]           = "property",
            };

        // if an element has a single doc comment tag with one of these names, then
        // it is considered valid since it has an alternative to a <summary>.
        private static readonly HashSet<string> SummaryAlternatives =
            new HashSet<string>()
            {
                "inheritdoc",
                "see",
            };
    }
}
