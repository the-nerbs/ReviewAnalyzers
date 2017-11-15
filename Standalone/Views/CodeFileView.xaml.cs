using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Editor;
using Microsoft.CodeAnalysis;
using Standalone.ViewModels;

namespace Standalone.Views
{
    /// <summary>
    /// Interaction logic for CodeFileView.xaml
    /// </summary>
    public partial class CodeFileView : UserControl
    {
        private static readonly IReadOnlyDictionary<DiagnosticSeverity, Color> SeverityColors =
            new Dictionary<DiagnosticSeverity, Color>
            {
                [DiagnosticSeverity.Hidden] = Colors.Transparent,
                [DiagnosticSeverity.Info] = Colors.Blue,
                [DiagnosticSeverity.Warning] = Colors.Green,
                [DiagnosticSeverity.Error] = Colors.Red,
            };


        private readonly TextMarkerService _diagnosticHighlighter;
        private readonly ToolTip _toolTip = new ToolTip();


        internal CodeFileViewModel ViewModel
        {
            get { return DataContext as CodeFileViewModel; }
        }


        public CodeFileView()
        {
            InitializeComponent();

            _diagnosticHighlighter = new TextMarkerService(sourceView.Document);

            TextView tv = sourceView.TextArea.TextView;
            tv.LineTransformers.Add(_diagnosticHighlighter);
            tv.BackgroundRenderers.Add(_diagnosticHighlighter);
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
                    _diagnosticHighlighter.RemoveAll((t) => true);

                    //TODO: check for binary files.
                    sourceView.Load(model.AbsolutePath);

                    string ext = Path.GetExtension(model.AbsolutePath);
                    sourceView.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(ext);

                    foreach (var diagnostic in model.Diagnostics)
                    {
                        int start = diagnostic.Location.SourceSpan.Start;
                        int length = diagnostic.Location.SourceSpan.Length;

                        var marker = _diagnosticHighlighter.Create(start, length);

                        marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                        marker.MarkerColor = SeverityColors[diagnostic.Severity];
                        marker.ToolTip = diagnostic.DiagnosticId + ": " + diagnostic.Message;
                    }
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

        private void HandleMouseHover(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(sourceView);
            TextViewPosition? tvp = sourceView.GetPositionFromPoint(mousePos);

            if (tvp.HasValue)
            {
                int offset = sourceView.Document.GetOffset(tvp.Value.Location);

                var content = _diagnosticHighlighter
                    .GetMarkersAtOffset(offset)
                    .Select(m => m?.ToolTip)
                    .Where(tt => tt != null)
                    .ToArray();

                if (content.Length > 0)
                {
                    var contentPanel = new StackPanel();
                    contentPanel.Orientation = Orientation.Vertical;

                    foreach (var elem in content)
                    {
                        if (elem is UIElement uiElem)
                        {
                            // item's already a UI element, just throw it on the panel.
                            contentPanel.Children.Add(uiElem);
                        }
                        else
                        {
                            // item's not a UI element, just let the framework figure out how to present it.
                            contentPanel.Children.Add(new ContentPresenter { Content = elem });
                        }
                    }

                    _toolTip.Content = contentPanel;
                    _toolTip.PlacementTarget = this;
                    _toolTip.IsOpen = true;
                }
            }
        }

        private void HandleMouseHoverStopped(object sender, MouseEventArgs e)
        {
            _toolTip.IsOpen = false;
        }
    }
}
