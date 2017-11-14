using Standalone.Models;
using Standalone.ViewModels;
using System;
using System.Collections.Generic;
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

namespace Standalone.Views
{
    /// <summary>
    /// Interaction logic for SolutionExplorer.xaml
    /// </summary>
    public partial class SolutionExplorer : UserControl
    {
        private MainViewModel MainViewModel
        {
            get { return DataContext as MainViewModel; }
        }


        public SolutionExplorer()
        {
            InitializeComponent();
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataContextProperty)
            {
                if (e.OldValue is MainViewModel oldVm)
                {
                    WeakEventManager<MainViewModel, EventArgs>.RemoveHandler(oldVm, nameof(oldVm.SolutionLoaded), HandleSolutionLoaded);
                }

                if (e.NewValue is MainViewModel newVm)
                {
                    WeakEventManager<MainViewModel, EventArgs>.AddHandler(newVm, nameof(newVm.SolutionLoaded), HandleSolutionLoaded);
                }
            }
        }


        private void HandleSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var vm = MainViewModel;
            var selectedNode = treeView.SelectedItem as TreeViewItem;

            if (vm != null && selectedNode != null)
            {
                vm.SelectedFile = selectedNode.DataContext as CodeFileModel;
            }
        }

        private void HandleSolutionLoaded(object sender, EventArgs e)
        {
            treeView.Items.Clear();

            var solutionModel = MainViewModel.Solution;
            if (solutionModel != null)
            {
                treeView.Items.Add(GetTreeItem(solutionModel));
            }
        }


        private TreeViewItem GetTreeItem(SolutionModel model)
        {
            var solution = new SolutionExplorerSolutionNode(model);

            if (model.SolutionItems != null)
            {
                solution.Items.Add(GetTreeItem(model.SolutionItems));
            }

            if (model.Projects != null && model.Projects.Count > 0)
            {
                foreach (var project in model.Projects.OrderBy(p => p.Name))
                {
                    solution.Items.Add(GetTreeItem(project));
                }
            }

            return solution;
        }

        private TreeViewItem GetTreeItem(ProjectModel model)
        {
            var project = new SolutionExplorerProjectNode(model);

            if (model.Folders != null && model.Folders.Count > 0)
            {
                foreach (var folder in model.Folders.OrderBy(f => f.Name))
                {
                    project.Items.Add(GetTreeItem(folder));
                }
            }

            if (model.CodeFiles != null && model.CodeFiles.Count > 0)
            {
                foreach (var file in model.CodeFiles.OrderBy(f => f.FileName))
                {
                    project.Items.Add(GetTreeItem(file));
                }
            }

            return project;
        }

        private TreeViewItem GetTreeItem(FolderModel model)
        {
            var item = new SolutionExplorerFolderNode(model);

            if (model.SubDirectories != null && model.SubDirectories.Count > 0)
            {
                foreach (var subdir in model.SubDirectories.OrderBy(d => d.Name))
                {
                    item.Items.Add(GetTreeItem(subdir));
                }
            }

            if (model.Files != null && model.Files.Count > 0)
            {
                foreach (var file in model.Files.OrderBy(f => f.FileName))
                {
                    item.Items.Add(GetTreeItem(file));
                }
            }

            return item;
        }

        private TreeViewItem GetTreeItem(CodeFileModel model)
        {
            return new SolutionExplorerItemNode(model);
        }
    }
}
