using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSTestExtensions;

namespace ReviewAnalyzers.Test
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    class StandardFactoryAttribute : CombinatorialFactoryAttribute
    {
        public new Type FactoryDeclaringType
        {
            get { return typeof(Factories); }
        }

        public StandardFactoryAttribute(string factoryMethodName) 
           : base(factoryMethodName)
        {
            base.FactoryDeclaringType = typeof(Factories);
        }
    }
}
