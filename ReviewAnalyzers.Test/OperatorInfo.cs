using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewAnalyzers.Test
{
    public class OperatorInfo
    {
        public string Token { get; }
        public int OperandCount { get; }


        public OperatorInfo(string token, int operandCount)
        {
            Token = token;
            OperandCount = operandCount;
        }

        public override string ToString()
        {
            return $"{Token}({OperandCount})";
        }
    }
}
