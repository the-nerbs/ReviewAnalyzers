using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standalone.Models
{
    class ProjectModel : ObservableObject
    {
        private string _name;
        private string _relativePath;
        private ProjectType _type;


        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public string RelativePath
        {
            get { return _relativePath; }
            set
            {
                Set(ref _relativePath, value);
                Type = DetermineType(value);
            }
        }

        public ProjectType Type
        {
            get { return _type; }
            private set { Set(ref _type, value); }
        }

        public ObservableCollection<FolderModel> Folders { get; }
            = new ObservableCollection<FolderModel>();

        public ObservableCollection<CodeFileModel> CodeFiles { get; }
            = new ObservableCollection<CodeFileModel>();


        private static ProjectType DetermineType(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return ProjectType.Generic;

            try
            {
                string ext = Path.GetExtension(path);
                ProjectType type;

                if (!TypesByExtension.TryGetValue(ext, out type))
                {
                    type = ProjectType.Generic;
                }

                return type;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception from determining project type: " + ex);
                return ProjectType.Generic;
            }
        }

        private static IReadOnlyDictionary<string, ProjectType> TypesByExtension
            = new Dictionary<string, ProjectType>
            {
                [".vcxproj"] = ProjectType.CPlusPlus,
                [".csproj"] = ProjectType.CSharp,
                [".dbproj"] = ProjectType.Database,
                [".sqlproj"] = ProjectType.Database,
                [".fsproj"] = ProjectType.FSharp,
                [".proj"] = ProjectType.Generic,
                [".modelproj"] = ProjectType.Modeling,
                [".pyproj"] = ProjectType.Python,
                [".rbproj"] = ProjectType.Ruby,
                [".vbproj"] = ProjectType.VisualBasic,
            };
    }
}
