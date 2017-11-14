using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Standalone.Models;
using Microsoft.Win32;

namespace Standalone.ViewModels
{
    sealed class MainViewModel : ObservableObject
    {
        private SolutionModel _solution;
        private CodeFileModel _selectedFile;


        public event EventHandler<EventArgs> SolutionLoaded;


        public SolutionModel Solution
        {
            get { return _solution; }
            set { Set(ref _solution, value); }
        }

        public CodeFileModel SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                Set(ref _selectedFile, value);
                FileViewModel.Model = value;
            }
        }

        public CodeFileViewModel FileViewModel { get; } = new CodeFileViewModel();


        public Command LoadSolutionFileCmd { get; }
        public Command LoadSolutionFromServerCmd { get; }


        public MainViewModel()
        {
            LoadSolutionFileCmd = new Command(LoadSolutionFromFile);

            // note: server interactions not yet implemented
            LoadSolutionFromServerCmd = new Command(LoadSolutionFromServer, () => false);
        }


        private void LoadSolutionFromFile()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Solution Files (*.sln)|*.sln",
                Multiselect = false,
            };

            // note: nullable booleans...
            if (ofd.ShowDialog() == true)
            {
                LoadSolution(ofd.FileName, null);
            }
        }

        private void LoadSolutionFromServer()
        {
            throw new NotImplementedException();
        }


        private void LoadSolution(string localPath, string serverPath)
        {
            var parser = new SolutionParser();
            SolutionModel model = parser.ParseSolution(localPath);

            if (!string.IsNullOrEmpty(serverPath))
            {
                model.ServerPath = serverPath;
            }

            _solution = model;
            OnSolutionLoaded();
        }


        private void OnSolutionLoaded()
        {
            SolutionLoaded?.Invoke(this, EventArgs.Empty);
        }

    }
}
