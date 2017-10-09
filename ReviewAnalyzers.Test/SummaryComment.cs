using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAnalyzers.Test
{
    public class SummaryComment
    {
        public string Text { get; }
        public bool ExpectsDiagnostic { get; }

        public SummaryComment(string text, bool expectsDiagnostic)
        {
            Text = text;
            ExpectsDiagnostic = expectsDiagnostic;
        }

        public override string ToString()
        {
            return "\"" + Text + "\"";
        }
    }
}
