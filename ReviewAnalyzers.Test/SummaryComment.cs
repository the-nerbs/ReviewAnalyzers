using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviewAnalyzers.Properties;

namespace ReviewAnalyzers.Test
{
    public class SummaryComment
    {
        public string Text { get; }
        public bool ExpectsMissingDiagnostic { get; }
        public bool ExpectsEmptyDiagnostic { get; }

        public int LineLength
        {
            get
            {
                if (Text == null)
                    return 0;

                return Text.Count(ch => ch == '\n');
            }
        }

        public bool ExpectsDiagnostic
        {
            get { return ExpectsMissingDiagnostic || ExpectsEmptyDiagnostic; }
        }

        public string ExpectedMessageFmt
        {
            get
            {
                if (ExpectsEmptyDiagnostic)
                    return Resources.EmptySummary_MessageFmt;

                if (ExpectsMissingDiagnostic)
                    return Resources.MissingSummary_MessageFmt;

                return null;
            }
        }


        public SummaryComment(string text, bool isMissing, bool isEmpty)
        {
            Text = text;
            ExpectsMissingDiagnostic = isMissing;
            ExpectsEmptyDiagnostic = isEmpty;
        }


        public override string ToString()
        {
            return "\"" + Text + "\"";
        }
    }
}
