using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Standalone.Models;

namespace Standalone.Views
{
    sealed class SolutionExplorerProjectNode : SolutionExplorerBaseNode
    {
        private static readonly IReadOnlyDictionary<ProjectType, ImageSource> ProjectTypeIcons
            = new Dictionary<ProjectType, ImageSource>
            {
                [ProjectType.CPlusPlus] = App.Current.CppProjectImg,
                [ProjectType.CSharp] = App.Current.CSharpProjectImg,
                [ProjectType.Database] = App.Current.DatabaseProjectImg,
                [ProjectType.FSharp] = App.Current.FSharpProjectImg,
                [ProjectType.Generic] = App.Current.GenericProjectImg,
                [ProjectType.Modeling] = App.Current.ModelingProjectImg,
                [ProjectType.Python] = App.Current.PythonProjectImg,
                [ProjectType.Ruby] = App.Current.RubyProjectImg,
                [ProjectType.VisualBasic] = App.Current.VBProjectImg,
            };


        public SolutionExplorerProjectNode(ProjectModel model)
            : base(LookupImage(ProjectTypeIcons, model.Type), model.Name)
        {
            DataContext = model;
        }
    }
}
