using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Standalone.Models;

namespace Standalone.Views
{
    sealed class SolutionExplorerFolderNode : SolutionExplorerBaseNode
    {
        private static readonly ImageSource CollapsedImageSource = App.Current.FolderImg;
        private static readonly ImageSource ExpandedImageSource = App.Current.FolderOpenImg;


        public SolutionExplorerFolderNode(FolderModel model)
            : base(CollapsedImageSource, model.Name)
        {
            DataContext = model;
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsExpandedProperty)
            {
                // I started writing this line before I realized what was going on... I'm so sorry:
                //   Image.Source = e.NewValue as bool? ?? false ? ExpandedImageSource : CollapsedImageSource;

                Image.Source = (bool)e.NewValue
                    ? ExpandedImageSource
                    : CollapsedImageSource;
            }
        }
    }
}
