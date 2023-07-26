using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using RemoveProjectFromSolution.Arguments;
using Rhyous.SimpleArgs;
using Rhyous.Tools;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RemoveProjectFromSolution
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            new ArgsManager<ArgsHandler>().Start(args);
            var solutionListFile = Args.Value("L");
            var solutionFullPath = Args.Value("S");
            var projectToRemove = Args.Value("P");

            var projectRemover = new ProjectRemover(new Retry());
            if (!string.IsNullOrEmpty(solutionFullPath))
                await projectRemover.RemoveAsync(solutionFullPath, projectToRemove);
            else if (!string.IsNullOrEmpty(solutionListFile))
                await projectRemover.RemoveAllAsync(solutionListFile, projectToRemove);
        }
    }

    internal class ProjectRemover
    {
        private readonly Retry _retry;

        public ProjectRemover(Retry retry)
        {
            _retry = retry;
        }
        public async Task RemoveAllAsync(string solutionListFile, string projectToRemove)
        {
            if (!File.Exists(solutionListFile))
                throw new Exception($"File does not exist: {solutionListFile}");
            var slnPaths = File.ReadAllLines(solutionListFile);
            foreach (var slnPath in slnPaths)
            {
                await RemoveAsync(slnPath, projectToRemove);
            }
        }

        public async Task RemoveAsync(string solutionFullPath, string projectToRemove)
        {
            // Get an instance of the Visual Studio IDE
            DTE2 dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE.17.0");
            var set = false;
            while (!set)
            {
                try { dte.SuppressUI = true; set = true; }
                catch (COMException) { System.Threading.Thread.Sleep(100); }
            }
            Solution4 solution = _retry.Run((DTE2 dte2) => (Solution4)dte2.Solution, dte);

            // Create an instance of SolutionEvents
            SolutionEvents solutionEvents = dte.Events.SolutionEvents;

            // Handle the ProjectAdded event
            solutionEvents.ProjectAdded += (projectAdded) =>
            {
                // Set SuppressUI to true to suppress the "Get projects from source control" popup
                dte.SuppressUI = true;
            };
            try { solution.Open(solutionFullPath); }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to open {solutionFullPath}");
                Console.WriteLine(e.Message);
            }

            set = false;
            Window activeWindow = null;
            while (!set)
            {
                try { activeWindow = dte.ActiveWindow; set = true; }
                catch (COMException) { System.Threading.Thread.Sleep(100); }
            }

            // Find the project to delete
            Project project = FindProject(projectToRemove, solution);

            // Delete the project
            if (project != null)
            {
                solution.Remove(project);
            }
            else
            {
                Console.WriteLine($"{solutionFullPath}: Project {projectToRemove} not found!");
            }

            // Save and close the solution
            try { solution.SaveAs(solutionFullPath); }
            catch (Exception e)
            {
                Console.WriteLine($"{solutionFullPath}: Failed to save.");
                Console.WriteLine(e.Message);
            }
            solution.Close();
        }

        private Project FindProject(string projectName, Solution4 solution)
        {
            foreach (Project p in solution.Projects)
            {
                //Console.WriteLine(p.Name);
                if (p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
                {
                    return p;
                }

                // If the project is located within a solution folder, search within the folder
                if (p.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    var found = FindProject(projectName, p);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }

        private static Project FindProject(string projectName, Project p)
        {
            foreach (ProjectItem pi in p.ProjectItems)
            {
                //Console.WriteLine(p.Name);
                var set = false;
                while (!set)
                {
                    try
                    {
                        if (pi != null && pi.SubProject != null && pi.SubProject.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
                        {
                            return pi.SubProject;
                        };
                        set = true;
                    }
                    catch (COMException) { System.Threading.Thread.Sleep(100); }
                }


                // If the project is located within a solution folder, search within the folder
                if (pi != null && pi.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    var found = FindProject(projectName, pi.SubProject);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }
    }
}
