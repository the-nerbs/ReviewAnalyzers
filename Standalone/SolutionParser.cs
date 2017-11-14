using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Standalone.Models;

namespace Standalone
{
    class SolutionParser
    {
        private static readonly Regex SolutionItemPattern = new Regex(
            @"^\s*(?<name>[^=]+)=(?<path>[^=]+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        private static readonly HashSet<string> IgnoreItems = new HashSet<string>()
        {
            "Reference",
            "ProjectReference",
            "Analyzer",
            "Service",
            "BootstrapperPackage",
        };


        public SolutionModel ParseSolution(string path)
        {
            var solution = SolutionFile.Parse(path);
            string name = Path.GetFileNameWithoutExtension(path);

            var model = new SolutionModel
            {
                Name = name,
                LocalPath = path,
            };

            foreach (var project in solution.ProjectsInOrder)
            {
                ProjectModel projModel = null;

                if (project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
                {
                    // a recognized MSBuild project xml format.
                    projModel = LoadProject(project);
                }
                else if (project.ProjectType != SolutionProjectType.SolutionFolder)
                {
                    // unknown type
                    projModel = new ProjectModel
                    {
                        Name = project.ProjectName,
                        RelativePath = project.RelativePath,
                    };
                }

                if (projModel != null)
                {
                    model.Projects.Add(projModel);
                }
            }

            string[] solutionItems = ParseSolutionItems(path);

            if (solutionItems != null && solutionItems.Length > 0)
            {
                model.SolutionItems = new FolderModel();
                model.SolutionItems.Name = "Solution Items";

                string solutionDir = Path.GetDirectoryName(path);

                foreach (string item in solutionItems)
                {
                    model.SolutionItems.Files.Add(new CodeFileModel
                    {
                        AbsolutePath = Path.GetFullPath(Path.Combine(solutionDir, item)),
                        RelativePath = item,
                    });
                }
            }

            return model;
        }

        private ProjectModel LoadProject(ProjectInSolution projectInfo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(projectInfo.AbsolutePath);

            ProjectModel model = new ProjectModel
            {
                Name = projectInfo.ProjectName,
                RelativePath = projectInfo.RelativePath,
            };

            string projectDir = Path.GetDirectoryName(projectInfo.AbsolutePath);

            // note: all C# code files use the Compile task.
            foreach (XmlElement itemGroup in doc.GetElementsByTagName("ItemGroup", "*"))
            {
                foreach (XmlElement item in itemGroup.ChildNodes.OfType<XmlElement>()
                                                     .Where(el => !IgnoreItems.Contains(el.LocalName)))
                {
                    string path = item.GetAttribute("Include");

                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        AddItem(model, path, projectDir);
                    }
                }
            }

            return model;
        }


        private void AddItem(ProjectModel model, string path, string projectDir)
        {
            string[] parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            var fileModel = new CodeFileModel
            {
                AbsolutePath = Path.GetFullPath(Path.Combine(projectDir, path)),
                RelativePath = path,
            };

            if (parts.Length > 1)
            {
                FolderModel folder = GetOrAdd(model.Folders, parts[0]);

                for (int i = 1; i < parts.Length-1; i++)
                {
                    folder = GetOrAdd(folder.SubDirectories, parts[i]);
                }

                folder.Files.Add(fileModel);
            }
            else
            {
                model.CodeFiles.Add(fileModel);
            }
        }

        private string[] ParseSolutionItems(string slnPath)
        {
            var items = new List<string>();

            using (var reader = new StreamReader(slnPath))
            {
                string line = reader.ReadLine();

                while (line != null)
                {
                    while (line != null &&
                           !line.StartsWith("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\")"))
                    {
                        line = reader.ReadLine();
                    }

                    // skip the 'Project(...) =' line
                    if (line != null)
                    {
                        line = reader.ReadLine();
                    }

                    // skip the 'ProjectSection(...) =' line
                    if (line != null)
                    {
                        line = reader.ReadLine();
                    }

                    while (line != null &&
                           line.Trim() != "EndProjectSection")
                    {
                        var m = SolutionItemPattern.Match(line);

                        if (m.Success)
                        {
                            string path = m.Groups["path"].Value;

                            items.Add(path.Trim());
                        }

                        line = reader.ReadLine();
                    }
                }
            }

            return items.ToArray();
        }


        private static string NormalizeDirectoryPath(string path)
        {
            // just make sure there's a separator on the end.
            if (path.Last() != Path.DirectorySeparatorChar &&
                path.Last() != Path.AltDirectorySeparatorChar)
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }

        private static FolderModel GetOrAdd(ObservableCollection<FolderModel> collection, string name)
        {
            FolderModel model = collection.FirstOrDefault(
                m => StringComparer.OrdinalIgnoreCase.Equals(m.Name, name)
            );

            if (model == null)
            {
                model = new FolderModel
                {
                    Name = name,
                };

                collection.Add(model);
            }

            return model;
        }
    }
}
