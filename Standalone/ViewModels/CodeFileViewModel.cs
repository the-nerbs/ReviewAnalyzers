using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Standalone.Models;

namespace Standalone.ViewModels
{
    sealed class CodeFileViewModel : ObservableObject
    {
        private CodeFileModel _model;


        public CodeFileModel Model
        {
            get { return _model; }
            set { Set(ref _model, value); }
        }


    }
}
