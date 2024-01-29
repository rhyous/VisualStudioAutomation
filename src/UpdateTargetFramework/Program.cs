using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using Rhyous.RemoveProjectFromSolution.Arguments;
using Rhyous.SimpleArgs;
using System;

namespace Rhyous.UpdateTargetFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            new ArgsManager<ArgsHandler>().Start(args);
            var solutionFilePath = Args.Value("S");

            DTE2 dte = null;
            try
            {
                // Create an instance of Visual Studio automation
                dte = (DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.17.0");
            }
            catch (Exception)
            {
                Console.WriteLine("Could not find an instance of Visual Studio.");
                return;
            }

            // Open the solution
            Solution4 solution = (Solution4)dte.Solution;
            solution.Open(solutionFilePath);

            // Update the projects to TargetFramework version 4.8
            UpdateProjectsToDotNet48(solution.Projects);

            // Save and close the solution
            solution.SaveAs(solutionFilePath);
            //solution.Close();

            Console.WriteLine("Projects updated to TargetFramework 4.8.");
        }

        static void UpdateProjectsToDotNet48(Projects allProjects)
        {
            foreach (Project p in allProjects)
            {
                if (p.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    UpdateProjectsToDotNet48(p.ProjectItems);
                }
                else
                {
                    UpdateProjectToDotNet48(p);
                }
            }
        }

        static void UpdateProjectsToDotNet48(ProjectItems items)
        {
            if (items == null)
                return;
            foreach (ProjectItem item in items)
            {
                if (item != null && item.SubProject != null && item.SubProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    UpdateProjectsToDotNet48(item.SubProject.ProjectItems);
                }
                else
                {
                    if (item.SubProject == null)
                        continue;
                    UpdateProjectToDotNet48(item.SubProject);
                }
            }
        }

        static void UpdateProjectToDotNet48(Project project)
        {
            var name = project.Name;
            var fileName = project.FileName;
            // Check if the project supports changing the TargetFramework
            if (project.Properties != null)
            {
                Property targetFrameworkProperty = project.Properties.Item("TargetFrameworkMoniker");
                if (targetFrameworkProperty != null)
                {
                    if (targetFrameworkProperty.Value.ToString() == ".NETFramework,Version=v4.8")
                        return;
                    // Change the TargetFramework to ".NETFramework,Version=v4.8"
                    targetFrameworkProperty.Value = ".NETFramework,Version=v4.8";
                    Console.WriteLine($"Updated TargetFramework to .NET Framework 4.8 for project: {fileName}");
                }
            }
            project.Save();
        }
    }
}