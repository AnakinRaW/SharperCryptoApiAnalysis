using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using SharperCryptoApiAnalysis.Interop.Services;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace SharperCryptoApiAnalysis.VisualStudio.Integration
{
    //Based on https://github.com/madskristensen/AddAnyFile with minor refactoring
    public static class ProjectHelpers
    {
        private static readonly ISharperCryptoAnalysisServiceProvider ServiceProvider =
            Services.SharperCryptoAnalysisServiceProvider;

        public static Project GetProjectOfSelectedItem()
        {
            var selectedItem = GetSelectedItem();
            var selectedProjectItem = selectedItem as ProjectItem;
            var selectedProject = selectedItem as Project;
            return selectedProjectItem?.ContainingProject ?? selectedProject ?? GetActiveProject();
        }

        public static string GetFolderOfSelectedItem()
        {
            var selectedItem = GetSelectedItem();
            var folder = FindFolder(selectedItem);

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                return null;

            return folder;
        }

        public static object GetSelectedItem()
        {
            var monitorSelection = ServiceProvider.GetService<SVsShellMonitorSelection>() as IVsMonitorSelection;

            monitorSelection.GetCurrentSelection(out var hierarchyPointer,
                out var itemId,
                out _,
                out var selectionContainerPointer);

            try
            {
                if (Marshal.GetTypedObjectForIUnknown(
                    hierarchyPointer,
                    typeof(IVsHierarchy)) is IVsHierarchy selectedHierarchy)
                {
                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(itemId,
                        (int) __VSHPROPID.VSHPROPID_ExtObject, out var selectedObject));
                    return selectedObject;
                }

                return null;
            }
            finally
            {
                Marshal.Release(hierarchyPointer);
                Marshal.Release(selectionContainerPointer);
            }
        }

        public static Project GetActiveProject()
        {
            var dte = Services.Dte;
            try
            {
                if (dte.ActiveSolutionProjects is Array activeSolutionProjects && activeSolutionProjects.Length > 0)
                    return activeSolutionProjects.GetValue(0) as Project;
                var doc = dte.ActiveDocument;

                if (doc != null && !string.IsNullOrEmpty(doc.FullName))
                {
                    var item = dte.Solution?.FindProjectItem(doc.FullName);
                    if (item != null)
                        return item.ContainingProject;
                }
            }
            catch
            {
            }
            return null;
        }

        public static ProjectItem AddFileToSelectedItem(Project project, FileInfo file, string itemType = null, object selectedItem = null)
        {
            ProjectItem projectItem = null;

            if (selectedItem == null)
                selectedItem = GetSelectedItem();

            if (selectedItem is ProjectItem projItem)
            {
                if ("{6BB5F8F0-4483-11D3-8BCF-00C04F8EC28C}" == projItem.Kind) // Constants.vsProjectItemKindVirtualFolder
                    projectItem = projItem.ProjectItems.AddFromFile(file.FullName);
            }

            if (projectItem == null)
            {
                if (project.IsKind(ProjectTypes.Aspnet5, ProjectTypes.Ssdt))
                    return Services.Dte.Solution.FindProjectItem(file.FullName);

                var root = project.GetRootFolder();

                if (string.IsNullOrEmpty(root) || !file.FullName.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    return null;

                var item = project.ProjectItems.AddFromFile(file.FullName);
                item.SetItemType(itemType);
                return item;
            }
            return projectItem;
        }

        public static void OpenFile(FileInfo file)
        {
            VsShellUtilities.OpenDocument(Services.SharperCryptoAnalysisServiceProvider, file.FullName);
            try
            {
                Services.Dte.ExecuteCommand("SolutionExplorer.SyncWithActiveDocument");
            }
            catch
            {
            }       
            Services.Dte.ActiveDocument.Activate();
        }

        public static void SetItemType(this ProjectItem item, string itemType)
        {
            try
            {
                if (item?.ContainingProject == null)
                    return;

                if (string.IsNullOrEmpty(itemType)
                    || item.ContainingProject.IsKind(ProjectTypes.WebsiteProject)
                    || item.ContainingProject.IsKind(ProjectTypes.UniversalApp))
                    return;

                item.Properties.Item("ItemType").Value = itemType;
            }
            catch
            {
            }
        }

        public static string FindFolder(object item)
        {
            if (item == null)
                return null;

            var dte = Services.Dte;
            if (dte.ActiveWindow is Window2 window && window.Type == vsWindowType.vsWindowTypeDocument)
            {
                // if a document is active, use the document's containing directory
                Document doc = dte.ActiveDocument;
                if (doc != null && !string.IsNullOrEmpty(doc.FullName))
                {
                    ProjectItem docItem = dte.Solution.FindProjectItem(doc.FullName);

                    if (docItem?.Properties != null)
                    {
                        string fileName = docItem.Properties.Item("FullPath").Value.ToString();
                        if (File.Exists(fileName))
                            return Path.GetDirectoryName(fileName);
                    }
                }
            }

            var projectItem = item as ProjectItem;
            if (projectItem != null && projectItem.Kind == "{6BB5F8F0-4483-11D3-8BCF-00C04F8EC28C}") //Constants.vsProjectItemKindVirtualFolder
            {
                var items = projectItem.ProjectItems;
                foreach (ProjectItem it in items)
                {
                    if (File.Exists(it.FileNames[1]))
                    {
                        return Path.GetDirectoryName(it.FileNames[1]);
                    }
                }
            }
            else
            {
                var project = item as Project;
                if (projectItem != null)
                {
                    string fileName = projectItem.FileNames[1];

                    if (File.Exists(fileName))
                    {
                        return Path.GetDirectoryName(fileName);
                    }
                    return fileName;


                }
                if (project != null)
                {
                    return project.GetRootFolder();
                }
            }
            return string.Empty;
        }

        public static string GetRootFolder(this Project project)
        {
            if (project == null)
                return null;

            if (project.IsKind("{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")) //ProjectKinds.vsProjectKindSolutionFolder
                return Path.GetDirectoryName(Services.Dte.Solution.FullName);

            if (string.IsNullOrEmpty(project.FullName))
                return null;

            string fullPath;
            try
            {
                fullPath = project.Properties.Item("FullPath").Value as string;
            }
            catch (ArgumentException)
            {
                try
                {
                    // MFC projects don't have FullPath, and there seems to be no way to query existence
                    fullPath = project.Properties.Item("ProjectDirectory").Value as string;
                }
                catch (ArgumentException)
                {
                    // Installer projects have a ProjectPath.
                    fullPath = project.Properties.Item("ProjectPath").Value as string;
                }
            }

            if (string.IsNullOrEmpty(fullPath))
                return File.Exists(project.FullName) ? Path.GetDirectoryName(project.FullName) : null;

            if (Directory.Exists(fullPath))
                return fullPath;

            return File.Exists(fullPath) ? Path.GetDirectoryName(fullPath) : null;
        }

        public static bool IsKind(this Project project, params string[] kindGuids)
        {
            foreach (string guid in kindGuids)
            {
                if (project.Kind.Equals(guid, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static string GetRootNamespace(this Project project)
        {
            if (project == null)
                return null;

            string ns = project.Name ?? string.Empty;

            try
            {
                Property prop = project.Properties.Item("RootNamespace");

                if (prop != null && prop.Value != null && !string.IsNullOrEmpty(prop.Value.ToString()))
                    ns = prop.Value.ToString();
            }
            catch { /* Project doesn't have a root namespace */ }

            return CleanNameSpace(ns, false);
        }

        public static string GetFileNameSpace(Project project, string filePath)
        {
            var relative = PackageUtilities.MakeRelative(project.GetRootFolder(), Path.GetDirectoryName(filePath));
            var rootNs = project.GetRootNamespace();
            return $"{rootNs}.{CleanNameSpace(relative)}".TrimEnd('.');
        }

        public static string CleanNameSpace(string ns, bool stripPeriods = true)
        {
            if (stripPeriods)
                ns = ns.Replace(".", "");

            ns = ns.Replace(" ", "")
                .Replace("-", "")
                .Replace("\\", ".");

            return ns;
        }
    }
}
