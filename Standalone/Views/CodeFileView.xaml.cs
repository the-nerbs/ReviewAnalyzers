using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.AvalonEdit.Highlighting;
using Standalone.ViewModels;

namespace Standalone.Views
{
    /// <summary>
    /// Interaction logic for CodeFileView.xaml
    /// </summary>
    public partial class CodeFileView : UserControl
    {
        private readonly SourceHighlightTransformer _diagnosticHighlighter = new SourceHighlightTransformer();


        internal CodeFileViewModel ViewModel
        {
            get { return DataContext as CodeFileViewModel; }
        }


        public CodeFileView()
        {
            InitializeComponent();

            sourceView.TextArea.TextView.LineTransformers.Add(_diagnosticHighlighter);
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataContextProperty)
            {
                if (e.OldValue is INotifyPropertyChanged oldVm)
                {
                    PropertyChangedEventManager.RemoveHandler(oldVm, HandleModelChanged, nameof(CodeFileViewModel.Model));
                }

                if (e.NewValue is INotifyPropertyChanged newVm)
                {
                    PropertyChangedEventManager.AddHandler(newVm, HandleModelChanged, nameof(CodeFileViewModel.Model));
                }
            }
        }

        private void HandleModelChanged(object sender, PropertyChangedEventArgs e)
        {
            var model = ViewModel?.Model;

            if (model != null)
            {
                try
                {
                    _diagnosticHighlighter.Diagnostics = model.Diagnostics.ToArray();

                    //TODO: check for binary files.
                    sourceView.Load(model.AbsolutePath);

                    string ext = System.IO.Path.GetExtension(model.AbsolutePath);
                    sourceView.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(ext);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Exception while trying to setup source view for " + model.RelativePath + ":" + ex);

                    sourceView.Clear();
                }
            }
            else
            {
                sourceView.Clear();
            }
        }
    }
}
