using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Standalone.Models;

namespace Standalone
{
    class SourceHighlightTransformer : DocumentColorizingTransformer
    {
        public IEnumerable<DiagnosticModel> Diagnostics { get; set; }


        public SourceHighlightTransformer()
        { }


        protected override void ColorizeLine(DocumentLine line)
        {
            if (Diagnostics != null)
            {
                int lineStart = line.Offset;
                int lineLength = line.Length;
                int lineEnd = lineStart + lineLength;

                var lineSpan = new TextSpan(lineStart, lineLength);

                foreach (var d in Diagnostics)
                {
                    var diagnosticSpan = d.Location.SourceSpan;

                    if (diagnosticSpan.IntersectsWith(lineSpan))
                    {
                        int startOffset = Math.Max(lineStart, diagnosticSpan.Start);
                        int endOffset = Math.Min(lineEnd, diagnosticSpan.End);

                        switch (d.Severity)
                        {
                            case DiagnosticSeverity.Warning:
                                ChangeLinePart(startOffset, endOffset, ApplyWarning);
                                break;

                            case DiagnosticSeverity.Error:
                                ChangeLinePart(startOffset, endOffset, ApplyError);
                                break;

                            default:
                            case DiagnosticSeverity.Info:
                            case DiagnosticSeverity.Hidden:
                                // ignore
                                break;
                        }
                    }
                }
            }
        }

        private static void ApplyWarning(VisualLineElement line)
        {
            line.BackgroundBrush = new SolidColorBrush(Color.FromRgb(255, 226, 81));
        }

        private static void ApplyError(VisualLineElement line)
        {
            line.BackgroundBrush = Brushes.LightPink;
        }
    }
}
