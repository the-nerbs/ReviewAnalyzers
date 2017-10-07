using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAnalyzers
{
    internal enum DiagnosticId
    {
        DocumentationBase = 1000,

        // Documentation comment diagnostics
        MissingOrEmptySummary = DocumentationBase,


    }

    internal static class DiagnosticIdExtensions
    {
        internal static string GetCategory(this DiagnosticId id)
        {
            int catId = (int)id / 1000;

            switch (catId)
            {
                case 1:
                    return "Documentation";

                default:
                    Debug.Assert(false, $"unexpected DiagnosticId {id}");
                    throw new ArgumentException();
            }
        }
    }
}
