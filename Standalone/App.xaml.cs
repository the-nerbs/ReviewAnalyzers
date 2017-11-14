using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Standalone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        new public static App Current
        {
            get { return (App)Application.Current; }
        }


        public BitmapImage FolderImg
        {
            get { return (BitmapImage)FindResource(nameof(FolderImg)); }
        }

        public BitmapImage FolderOpenImg
        {
            get { return (BitmapImage)FindResource(nameof(FolderOpenImg)); }
        }


        public BitmapImage SolutionImg
        {
            get { return (BitmapImage)FindResource(nameof(SolutionImg)); }
        }


        #region Project type images

        public BitmapImage CppProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(CppProjectImg)); }
        }

        public BitmapImage CSharpProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(CSharpProjectImg)); }
        }

        public BitmapImage DatabaseProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(DatabaseProjectImg)); }
        }

        public BitmapImage FSharpProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(FSharpProjectImg)); }
        }

        public BitmapImage GenericProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(GenericProjectImg)); }
        }

        public BitmapImage ModelingProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(ModelingProjectImg)); }
        }

        public BitmapImage PythonProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(PythonProjectImg)); }
        }

        public BitmapImage RubyProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(RubyProjectImg)); }
        }

        public BitmapImage VBProjectImg
        {
            get { return (BitmapImage)FindResource(nameof(VBProjectImg)); }
        }

        #endregion


        #region Item type images

        public BitmapImage UnknownItemImg
        {
            get { return (BitmapImage)FindResource(nameof(UnknownItemImg)); }
        }

        public BitmapImage CppItemImg
        {
            get { return (BitmapImage)FindResource(nameof(CppItemImg)); }
        }

        public BitmapImage CSharpItemImg
        {
            get { return (BitmapImage)FindResource(nameof(CSharpItemImg)); }
        }

        public BitmapImage CssItemImg
        {
            get { return (BitmapImage)FindResource(nameof(CssItemImg)); }
        }

        public BitmapImage FSharpItemImg
        {
            get { return (BitmapImage)FindResource(nameof(FSharpItemImg)); }
        }

        public BitmapImage PropsItemImg
        {
            get { return (BitmapImage)FindResource(nameof(PropsItemImg)); }
        }

        public BitmapImage PythonItemImg
        {
            get { return (BitmapImage)FindResource(nameof(PythonItemImg)); }
        }

        public BitmapImage RubyItemImg
        {
            get { return (BitmapImage)FindResource(nameof(RubyItemImg)); }
        }

        public BitmapImage TextItemImg
        {
            get { return (BitmapImage)FindResource(nameof(TextItemImg)); }
        }

        public BitmapImage VBItemImg
        {
            get { return (BitmapImage)FindResource(nameof(VBItemImg)); }
        }

        public BitmapImage XmlItemImg
        {
            get { return (BitmapImage)FindResource(nameof(XmlItemImg)); }
        }

        public BitmapImage XmlSchemaItemImg
        {
            get { return (BitmapImage)FindResource(nameof(XmlSchemaItemImg)); }
        }

        #endregion
    }
}
