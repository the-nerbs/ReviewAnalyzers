using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Standalone.Models
{
    class DiagnosticModel : ObservableObject
    {
        private Diagnostic _innerDiagnostic;


        public Diagnostic InnerDiagnostic
        {
            get { return _innerDiagnostic; }
            set
            {
                Set(ref _innerDiagnostic, value);
                OnPropertyChanged(string.Empty);
            }
        }

        public Location Location
        {
            get { return _innerDiagnostic?.Location; }
        }

        public DiagnosticSeverity Severity
        {
            get { return _innerDiagnostic?.Severity ?? DiagnosticSeverity.Hidden; }
        }

        public string DiagnosticId
        {
            get { return _innerDiagnostic?.Id; }
        }

        public string Message
        {
            get { return InnerDiagnostic.GetMessage(); }
        }


        public DiagnosticModel()
            : this(null)
        { }

        public DiagnosticModel(Diagnostic diagnostic)
        {
            _innerDiagnostic = diagnostic;
        }
    }
}
