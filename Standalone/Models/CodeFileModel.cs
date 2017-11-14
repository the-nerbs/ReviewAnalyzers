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
    class CodeFileModel : ObservableObject
    {
        private string _absolutePath;
        private string _relativePath;
        private string _fileName;
        private ItemType _type;
        private ObservableCollection<DiagnosticModel> _diagnostics;


        public string AbsolutePath
        {
            get { return _absolutePath; }
            set { Set(ref _absolutePath, value); }
        }

        public string RelativePath
        {
            get { return _relativePath; }
            set
            {
                Set(ref _relativePath, value);
                FileName = Path.GetFileName(value);
            }
        }

        public string FileName
        {
            get { return _fileName; }
            private set
            {
                Set(ref _fileName, value);
                Type = DetermineType(value);
            }
        }

        public ItemType Type
        {
            get { return _type; }
            private set { Set(ref _type, value); }
        }

        public ObservableCollection<DiagnosticModel> Diagnostics
        {
            get
            {
                EnsureDiagnostics();
                return _diagnostics;
            }
        }


        private void EnsureDiagnostics()
        {
            if (_diagnostics == null)
            {
                _diagnostics = new ObservableCollection<DiagnosticModel>();

                switch (Type)
                {
                    case ItemType.CSharp:
                        var analyzer = new SourceAnalyzer();
                        foreach (var d in analyzer.GetDiagnostics(AbsolutePath))
                        {
                            _diagnostics.Add(new DiagnosticModel(d));
                        }
                        break;

                    default:
                        // only C# files are supported (for now).
                        break;
                }
            }
        }


        private static ItemType DetermineType(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return ItemType.Unknown;

            try
            {
                string ext = Path.GetExtension(path);
                ItemType type;

                if (!TypesByExtension.TryGetValue(ext, out type))
                {
                    type = ItemType.Unknown;
                }

                return type;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception from determining item type: " + ex);
                return ItemType.Unknown;
            }
        }

        private static IReadOnlyDictionary<string, ItemType> TypesByExtension
            = new Dictionary<string, ItemType>
            {
                [".c"] = ItemType.Cpp,
                [".cpp"] = ItemType.Cpp,
                [".cxx"] = ItemType.Cpp,
                [".cc"] = ItemType.Cpp,
                [".ixx"] = ItemType.Cpp,
                [".h"] = ItemType.Cpp,
                [".hh"] = ItemType.Cpp,
                [".hpp"] = ItemType.Cpp,
                [".hxx"] = ItemType.Cpp,
                [".inl"] = ItemType.Cpp,
                [".tli"] = ItemType.Cpp,
                [".tlh"] = ItemType.Cpp,

                [".cs"] = ItemType.CSharp,
                [".csx"] = ItemType.CSharp,

                [".css"] = ItemType.Css,

                [".fs"] = ItemType.FSharp,
                [".fsx"] = ItemType.FSharp,

                [".props"] = ItemType.Props,

                [".py"] = ItemType.Python,

                [".rb"] = ItemType.Ruby,

                [".txt"] = ItemType.Text,

                [".vb"] = ItemType.VisualBasic,

                [".xml"] = ItemType.Xml,
                [".html"] = ItemType.Xml,
                [".xaml"] = ItemType.Xml,

                [".xsd"] = ItemType.XmlSchema,
            };
    }
}
