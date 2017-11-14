using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Standalone.Models;

namespace Standalone.Views
{
    sealed class SolutionExplorerSolutionNode : SolutionExplorerBaseNode
    {
        private static readonly ImageSource SolutionImage = App.Current.SolutionImg;


        public SolutionExplorerSolutionNode(SolutionModel model)
            : base(SolutionImage, model.Name)
        {
            DataContext = model;
        }
    }
}
