using System;
using System.Collections;
using System.Collections.Generic;
using Rhyous.AddSystemConfigurationReference.Arguments;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90;
using Rhyous.SimpleArgs;
using VSLangProj;

namespace Rhyous.AddSystemConfigurationReference
{
    class Program
    {
        private static string _projectName;

        static void Main(string[] args)
        {
            new ArgsManager<ArgsHandler>().Start(args);
            var solutionFilePath = Args.Value("S");
            var assembliesCommaSeparated = Args.Value("A");
            _projectName = Args.Value("P");
            var assemblies = assembliesCommaSeparated.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

            // Add System.Configuration reference to the projects
            AddReferenceToProjects(solution.Projects, assemblies);

            // Save and close the solution
            solution.SaveAs(solutionFilePath);
            //solution.Close();

            Console.WriteLine("System.Configuration reference added to projects.");
        }

        static void AddReferenceToProjects(Projects allProjects, IEnumerable<string> assemblies)
        {
            foreach (Project p in allProjects)
            {
                if (p.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    AddReferenceToProjects(p.ProjectItems, assemblies);
                }
                else
                {
                    AddReferencesToProject(p, assemblies);
                }
            }
        }

        static void AddReferenceToProjects(ProjectItems items, IEnumerable<string> assemblies)
        {
            if (items == null)
                return;
            foreach (ProjectItem item in items)
            {
                if (item != null && item.SubProject != null && item.SubProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    AddReferenceToProjects(item.SubProject.ProjectItems, assemblies);
                }
                else
                {
                    if (item.SubProject == null)
                        continue;
                    AddReferencesToProject(item.SubProject, assemblies);
                }
            }
        }

        static void AddReferencesToProject(Project project, IEnumerable<string> assemblies)
        {
            foreach (var a in assemblies)
            {
                AddReferenceToProject(project, a);
            }
            //project.Save();
        }

        static void AddReferenceToProject(Project project, string assemblyReference)
        {
            var name = project.Name;
            if (!string.IsNullOrWhiteSpace(_projectName) && _projectName != name)
                return;
            var csproj = project.FileName;
            if (project.CodeModel == null)
                return;

            // Check if the reference is already added
            VSProject vsProject = project.Object as VSProject;
            if (vsProject != null)
            {
                foreach (Reference reference in vsProject.References)
                {
                    if (reference.Name == assemblyReference)
                    {
                        Console.WriteLine($"Reference '{assemblyReference}' already exists in project: {csproj}");
                        return;
                    }
                }
            }

            // Add the reference
            try 
            { 
                vsProject.References.Add(assemblyReference); 
                Console.WriteLine($"Added '{assemblyReference}' reference to project: {csproj}");
            } 
            catch (Exception e)
            {
                Console.WriteLine($"Error adding '{assemblyReference}' reference to project: {csproj}.");
                Console.WriteLine(e.Message);
            }
        }
    }
}