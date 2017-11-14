using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Standalone.Models;

namespace Standalone.Views
{
    sealed class SolutionExplorerItemNode : SolutionExplorerBaseNode
    {
        private static readonly IReadOnlyDictionary<ItemType, ImageSource> ItemTypeIcons
            = new Dictionary<ItemType, ImageSource>
            {
                [ItemType.Unknown] = App.Current.UnknownItemImg,
                [ItemType.Cpp] = App.Current.CppItemImg,
                [ItemType.CSharp] = App.Current.CSharpItemImg,
                [ItemType.Css] = App.Current.CssItemImg,
                [ItemType.FSharp] = App.Current.FSharpItemImg,
                [ItemType.Props] = App.Current.PropsItemImg,
                [ItemType.Python] = App.Current.PythonItemImg,
                [ItemType.Ruby] = App.Current.RubyItemImg,
                [ItemType.Text] = App.Current.TextItemImg,
                [ItemType.VisualBasic] = App.Current.VBItemImg,
                [ItemType.Xml] = App.Current.XmlItemImg,
                [ItemType.XmlSchema] = App.Current.XmlSchemaItemImg,
            };


        public SolutionExplorerItemNode(CodeFileModel model)
            : base(LookupImage(ItemTypeIcons, model.Type), model.FileName)
        {
            DataContext = model;
        }
    }
}
