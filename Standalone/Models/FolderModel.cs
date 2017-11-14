using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standalone.Models
{
    sealed class FolderModel : ObservableObject
    {
        private string _name;


        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public ObservableCollection<FolderModel> SubDirectories { get; }
            = new ObservableCollection<FolderModel>();

        public ObservableCollection<CodeFileModel> Files { get; }
            = new ObservableCollection<CodeFileModel>();
    }
}
