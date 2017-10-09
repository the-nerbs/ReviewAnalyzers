using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAnalyzers.Test
{
    public class AccessLevelInfo
    {
        public string Keyword { get; }
        public bool RequiresSummary { get; }


        public AccessLevelInfo(string keyword, bool requiresSummary)
        {
            Keyword = keyword;
            RequiresSummary = requiresSummary;
        }


        public override string ToString()
        {
            return Keyword;
        }
    }
}
