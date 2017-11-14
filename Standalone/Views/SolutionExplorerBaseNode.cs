using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Standalone.Views
{
    abstract class SolutionExplorerBaseNode : TreeViewItem
    {
        private readonly Image _image;

        protected Image Image
        {
            get { return _image; }
        }


        protected SolutionExplorerBaseNode(ImageSource image, string text)
        {
            _image = new Image
            {
                Source = image,
                MaxWidth = 16,
                MaxHeight = 16,
                Margin = new Thickness(0, 0, 4, 0),
            };

            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                    {
                        _image,
                        new TextBlock
                        {
                            Text = text
                        },
                    }
            };
        }


        protected static ImageSource LookupImage<TKey>(IReadOnlyDictionary<TKey, ImageSource> map, TKey key, TKey defaultKey = default(TKey))
        {
            try
            {
                if (map.TryGetValue(key, out var image))
                {
                    return image;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error looking up solution explorer image: " + ex);
            }

            return map[defaultKey];
        }
    }
}
