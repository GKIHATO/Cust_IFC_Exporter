using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System;
using Microsoft.Win32;
using BIM.IFC.Export.UI;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB.ExternalService;
using Revit.IFC.Export.Exporter;
using System.Threading;
using System.ComponentModel;
using System.Activities.Statements;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.IO.Pipes;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Autodesk.Revit.DB.IFC;
using System.Windows.Forms;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Activities.Expressions;
using System.Xml.Linq;
using Xbim.MvdXml;
using System.Text.RegularExpressions;
using Xbim.Ifc4.Kernel;

namespace Cust_IFC_Exporter
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
   {

        string fileSavePath = null;

        IList<Document> documentsToExport = null;

        bool? exportAsOfficialMVDs = null;

        string ER_Name = null;

        string MVD_Name = null;

        IFCVersion ifcVersion;

        string mvdFilePath = null;

        List<string> docsFailedToExport;

        List<string> docsIsExported;

        List<string> revitDocsExported;

        ExternalEvent revalidateEvent;

        ReValidateEventHandler revalidateEventHandler;

        public Result Execute(ExternalCommandData commandData,ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;

/*                      List<string> docsExported = new List<string>();

                       docsExported.Add(@"C:\Users\s4495385\Desktop\CIVL3510_Lab1_45361065_Meehan_11.rvt");

                       List<string> csvPath = new List<string>();

                       csvPath.Add(@"C:\Users\s4495385\Desktop\Validate Result_CIVL3510_Lab1_45361065_Meehan_11.csv");

                       ExternalEvent resetEvent = ExternalEvent.Create(new ResetViewEventHandler());

                       ExternalEvent zoomToFitEvent = ExternalEvent.Create(new ZoomToFit());

                       ShowElementsEventHandler showElementsEventHandler = new ShowElementsEventHandler();

                       ExternalEvent showEvent = ExternalEvent.Create(showElementsEventHandler);

                       FillInfoEventHandler fillInfoEventHandler = new FillInfoEventHandler();

                       ExternalEvent fillEvent = ExternalEvent.Create(fillInfoEventHandler);

                       ResultWindow resultWindow = new ResultWindow(docsExported, csvPath, uiapp, resetEvent, zoomToFitEvent, showEvent, showElementsEventHandler,fillEvent,fillInfoEventHandler);

                       resultWindow.Show();*/

            IFCExport_MainWindow newWindow = new IFCExport_MainWindow(uiapp);

            bool? dialogResult = newWindow.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                fileSavePath = newWindow.fileSavePath;

                documentsToExport = IFCExport_MainWindow.documentsToExport;

                MVD_Name = newWindow.MVD_Name;

                ER_Name = newWindow.ER_Name;

                ifcVersion = newWindow.ifcVersion;

                mvdFilePath = newWindow.mvdFilePath;

                exportAsOfficialMVDs = newWindow.exportAsOfficialMVDs;

                if (newWindow.exportAsOfficialMVDs.Value)
                {
                    ExportAsOfficialMVDs(uiapp);
                }
                else
                {
                    ExportAsNonOfficialMVDs(uiapp);
                }
            }
            return Result.Succeeded;
        }

        private void ExportAsNonOfficialMVDs(UIApplication uiapp)
        {
            IFCExportConfiguration.CreateBuiltInConfiguration(IFCVersion.IFC2x3CV2, 0, false, false, false, false, false, false, false, false, false, includeSteelElements: true);

            IFCExportConfiguration newConfig = new IFCExportConfiguration();

            newConfig.Name = MVD_Name;

            newConfig.IFCVersion = ifcVersion;

            if (ifcVersion == IFCVersion.IFC2x3 || ifcVersion == IFCVersion.IFC2x2)
            {
                newConfig.SpaceBoundaries = 1;

                newConfig.ExportInternalRevitPropertySets = true;
            }

            newConfig.ExportBaseQuantities = true;

            if (ifcVersion == IFCVersion.IFC2x2)
            {
                newConfig.IncludeSteelElements = false;
            }

            newConfig.StoreIFCGUID = true;

            newConfig.ExportUserDefinedParameterMapping = true;

            newConfig.ExportUserDefinedPsets = true;

            string resourceFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

            string paraMappingTablePath = Path.Combine(resourceFolderPath, "IFC Mapping Table_Parameters.txt");

            string familyMappingFilePath = Path.Combine(resourceFolderPath, "IFC Export Class Mapping Table.txt");

            string customPropertySetFilePath = Path.Combine(resourceFolderPath, "CustomPSet.txt");

            newConfig.ExportUserDefinedParameterMappingFileName = paraMappingTablePath;

            newConfig.ExportUserDefinedPsetsFileName = customPropertySetFilePath;

            int count = 0;

            docsFailedToExport = new List<string>();

            docsIsExported = new List<string>();

            revitDocsExported = new List<string>();

            foreach (Document doc in documentsToExport)
            {
                count++;

                string fileName = Path.GetFileNameWithoutExtension(doc.PathName) + ".ifc";

                string fullFileName = fileSavePath +"\\" + fileName;

                if (File.Exists(fullFileName))
                {
                    TaskDialog messageDialog = new TaskDialog("File Exist!");

                    messageDialog.MainContent = @"File with the same name already exist in the target directory, overwirte?";
                    
                    messageDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                    TaskDialogResult taskDialogResult = messageDialog.Show();

                    if (taskDialogResult == TaskDialogResult.Cancel)
                    {
                        docsFailedToExport.Add(Path.GetFileName(doc.PathName));
                        continue;
                    }
                }

                IFCExportOptions exportOptions = new IFCExportOptions();

                exportOptions.FamilyMappingFile = familyMappingFilePath;

                Transaction newTransaction = new Transaction(doc, "Export IFC");

                newTransaction.Start();

                newConfig.UpdateOptions(exportOptions, newConfig.ActiveViewId);

                bool result = doc.Export(fileSavePath, fileName, exportOptions);

                newTransaction.Commit();

                ReplaceMVDName(fullFileName);

                if (result == false)
                {
                    docsFailedToExport.Add(Path.GetFileName(doc.PathName));
                }
                else
                { 
                    docsIsExported.Add(fullFileName);

                    revitDocsExported.Add(doc.PathName);
                   
                }

            }

            string title = "Export Complete";

            string message = null;

            string textMessages = null ;

            if (docsFailedToExport.Count > 0)
            {
                foreach (string docName in docsFailedToExport)
                {
                    textMessages+=docName;
                    textMessages+="\n";
                }

                message = $"{docsFailedToExport.Count} out of {documentsToExport.Count} file(s) are failed to export:";
               

                if(docsFailedToExport.Count==documentsToExport.Count)
                {
                    title = "Export failed!";
                }
                else
                {
                    message += @"Click Ok to validate against the mvdXML file, Cancel to abort (show files)!";
                }
            }
            else
            {              
                message = $"Total of {documentsToExport.Count} file(s) are all export successfully!";

                textMessages += @"Click Ok to validate against the mvdXML file, Cancel to abort (show files)!";
            }

            TaskDialog newDialog = new TaskDialog(title);

            newDialog.MainInstruction = message;

            newDialog.MainContent = textMessages.ToString();

            newDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

            TaskDialogResult dialogResult = newDialog.Show();

            if (docsFailedToExport.Count != documentsToExport.Count)
            {
                if (dialogResult == TaskDialogResult.Ok)
                {
                    ValidateStatusBar newStatusBar = new ValidateStatusBar(docsIsExported, ER_Name, mvdFilePath,fileSavePath);

                    bool? validateResult=newStatusBar.ShowDialog();

                    if(validateResult.HasValue && validateResult.Value)
                    {
                        ShowValidateResultInRevit(newStatusBar.csvFileNames,uiapp);
                    }
                    else
                    {
                        MessageBox.Show("Validate Cancelled! Please check validate results through Excel!");

                        Process.Start("explorer.exe", fileSavePath);
                    }
                }
                else
                {
                    Process.Start("explorer.exe", fileSavePath);
                }
            }
        }

        private void ReplaceMVDName(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return; 
            }

            string linePrefix= "FILE_DESCRIPTION";

            // Read the entire content of the IFC file
            string[] lines = File.ReadAllLines(filePath);

            // Create a temporary list to hold the modified lines
            for (int i = 0; i < lines.Length; i++)
            {
                // Check if the current line matches the pattern
                if (lines[i].StartsWith(linePrefix))
                {
                    string line = lines[i];
                   
                    string pattern_1 = @"\bViewDefinition\s*\[\s*(.*?)\s*\]";

                    string newValue_1 = $"[{MVD_Name}]";

                    string newLine = Regex.Replace(line, pattern_1, "ViewDefinition " + newValue_1);

                    string pattern_2 = @"\bExchangeRequirement\s*\[\s*(.*?)\s*\]";

                    string newValue_2 = $"[{ER_Name}]";

                    lines[i] = Regex.Replace(newLine, pattern_2, "ExchangeRequirement " + newValue_2);

                    break;

                }
            }

            // Write the modified content back to the same file
            File.WriteAllLines(filePath, lines);
        }

        private void ShowValidateResultInRevit(List<string> csvSavePath,UIApplication uiapp)
        {

            ExternalEvent resetEvent = ExternalEvent.Create(new ResetViewEventHandler());

            ExternalEvent zoomToFitEvent = ExternalEvent.Create(new ZoomToFit());

            ShowElementsEventHandler showElementsEventHandler = new ShowElementsEventHandler();

            ExternalEvent showEvent = ExternalEvent.Create(showElementsEventHandler);

            FillInfoEventHandler fillInfoEventHandler = new FillInfoEventHandler();

            ExternalEvent fillEvent = ExternalEvent.Create(fillInfoEventHandler);

            ResultWindow resultWindow = new ResultWindow(revitDocsExported,csvSavePath,uiapp,resetEvent,zoomToFitEvent,showEvent,showElementsEventHandler,fillEvent,fillInfoEventHandler);

            revalidateEventHandler = new ReValidateEventHandler();

            revalidateEvent = ExternalEvent.Create(revalidateEventHandler);

            resultWindow.Closed += ResultWindow_Closed;

            resultWindow.Show();
        }

        private void ResultWindow_Closed(object sender, EventArgs e)
        {
            ResultWindow resultWindow = sender as ResultWindow;

            if(resultWindow.reValidate)
            {
                IList<Document> newDocumentsToExport = new List<Document>();

                foreach (var doc in documentsToExport)
                {
                    if(docsIsExported.Contains(fileSavePath + "\\" + Path.GetFileNameWithoutExtension(doc.PathName) + ".ifc"))
                    {
                        newDocumentsToExport.Add(doc);
                    }
                }

                revalidateEventHandler.documentsToExport = newDocumentsToExport;

                revalidateEventHandler.ER_Name = ER_Name;

                revalidateEventHandler.mvdFilePath = mvdFilePath;

                revalidateEventHandler.fileSavePath = fileSavePath;

                revalidateEventHandler.ifcVersion = ifcVersion;

                revalidateEventHandler.MVD_Name = MVD_Name;

                revalidateEvent.Raise();
            }
        }

        private void ExportAsOfficialMVDs(UIApplication uiapp)
        {

            UIDocument uiDoc = uiapp.ActiveUIDocument;   

            IFCExportConfigurationsMap newConfigMap = new IFCExportConfigurationsMap();

            newConfigMap.AddBuiltInConfigurations();

            string configName = null;

            if (ifcVersion == IFCVersion.IFC4RV)
            {
                switch (ER_Name)
                {
                    case "Architectural Reference Exchange":
                        configName = $"{MVD_Name} [Architecture]";
                        break;
                    case "MEP Reference Exchange":
                        configName = $"{MVD_Name} [BuildingService]";
                        break;
                    case "Structural Reference Exchange":
                        configName = $"{MVD_Name} [Structural]";
                        break;
                }
            }
            else
            {
                configName = MVD_Name;

            }


            IFCExportConfiguration newConfig = new IFCExportConfiguration();

            newConfig = newConfigMap[configName];

            int count = 0;


            docsFailedToExport = new List<string>();

            foreach (Document doc in documentsToExport)
            {
                count++;

                string fileName = Path.GetFileNameWithoutExtension(doc.PathName) + ".ifc";

                string fullFileName = fileSavePath +"\\"+fileName;

                if (File.Exists(fullFileName))
                {
                    TaskDialog messageDialog = new TaskDialog("File Exist!");

                    messageDialog.MainContent = @"File with the same name already exist in the target directory, overwirte?";

                    messageDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                    TaskDialogResult taskDialogResult = messageDialog.Show();

                    if (taskDialogResult == TaskDialogResult.Cancel)
                    {
                        docsFailedToExport.Add(Path.GetFileName(doc.PathName));
                        continue;
                    }
                }

                IFCExportOptions exportOptions = new IFCExportOptions();

                Transaction newTransaction = new Transaction(doc, "Export IFC");

                newTransaction.Start();

                ElementId activeViewId = GenerateActiveViewIdFromDocument(doc);

                newConfig.ActiveViewId = newConfig.UseActiveViewGeometry ? activeViewId : ElementId.InvalidElementId;

                newConfig.UpdateOptions(exportOptions, activeViewId);

               // OnPassCurrentNumb(count);

                bool result = doc.Export(fileSavePath, fileName, exportOptions);

                newTransaction.Commit();

                if (result == false)
                {
                    docsFailedToExport.Add(Path.GetFileName(doc.PathName));
                }

            }

            string title = "Export Complete";

            string message = null;

            string textMessages = null;

            if (docsFailedToExport.Count > 0)
            {
                foreach (string docName in docsFailedToExport)
                {
                    textMessages += docName;
                    textMessages += "\n";
                }

                message = $"{docsFailedToExport.Count} out of {documentsToExport.Count} file(s) are failed to export:";


                if (docsFailedToExport.Count == documentsToExport.Count)
                {
                    title = "Export failed!";
                }
                else
                {
                    message += @"Click Ok to show exported ifc files!";
                }
            }
            else
            {
                message = $"Total of {documentsToExport.Count} file(s) are all export successfully!";

                textMessages += @"Click Ok to show exported ifc files!";
            }

            TaskDialog newDialog = new TaskDialog(title);

            newDialog.MainInstruction = message;

            newDialog.MainContent = textMessages.ToString();

            newDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

            TaskDialogResult dialogResult = newDialog.Show();

            if (docsFailedToExport.Count != documentsToExport.Count && dialogResult == TaskDialogResult.Ok)
            {
                Process.Start("explorer.exe", fileSavePath);
            }

            //clientPipe.Close(); 
        }

/*        private void ThreadStartingPoint()
        {
            exportStatusBar newWindow = new exportStatusBar(documentsToExport.Count);
            myWindow = newWindow;
            newWindow.Show();
            System.Windows.Threading.Dispatcher.Run();
             
            //newWindow.InitializeServer();
        }*/


        private ElementId GenerateActiveViewIdFromDocument(Document doc)
        {
            try
            {
                Autodesk.Revit.DB.View activeView = doc.ActiveView;
                ElementId activeViewId = (activeView == null) ? ElementId.InvalidElementId : activeView.Id;
                return activeViewId;
            }
            catch
            {
                return ElementId.InvalidElementId;
            }
        }

    }


}
