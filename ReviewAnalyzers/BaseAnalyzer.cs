using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using ReviewAnalyzers.Properties;

namespace ReviewAnalyzers
{
    /// <summary>
    /// Base class for analyzers defined in this assembly.
    /// </summary>
    public abstract class BaseAnalyzer : DiagnosticAnalyzer
    {
        internal static string CreateIdString(DiagnosticId id)
        {
            int num = (int)id;

            return "REVIEW" + num.ToString("D5");
        }

        internal static LocalizableString FromResource(string resourceName)
        {
            return new LocalizableResourceString(
                resourceName,
                Resources.ResourceManager,
                typeof(Resources)
            );
        }
    }
}
