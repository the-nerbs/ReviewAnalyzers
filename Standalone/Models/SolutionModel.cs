using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standalone.Models
{
    class SolutionModel : ObservableObject
    {
        private string _name;
        private string _localPath;
        private string _serverPath;
        private FolderModel _solutionItems;


        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public string LocalPath
        {
            get { return _localPath; }
            set { Set(ref _localPath, value); }
        }

        public string ServerPath
        {
            get { return _serverPath; }
            set { Set(ref _serverPath, value); }
        }

        public FolderModel SolutionItems
        {
            get { return _solutionItems; }
            set { Set(ref _solutionItems, value); }
        }

        public ObservableCollection<ProjectModel> Projects { get; }
            = new ObservableCollection<ProjectModel>();
    }
}
