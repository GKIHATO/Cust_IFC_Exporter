using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIM.IFC.Export.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cust_IFC_Exporter
{
    public class ReValidateEventHandler : IExternalEventHandler
    {
        public string fileSavePath = null;

        public IList<Document> documentsToExport = null;

        public string ER_Name = null;

        public string MVD_Name = null;

        public IFCVersion ifcVersion;

        public string mvdFilePath = null;

        List<string> docsFailedToExport;

        List<string> docsIsExported;

        List<string> revitDocsExported;

        ReValidateEventHandler revalidateEventHandler;

        ExternalEvent revalidateEvent;

        public void Execute(UIApplication uiapp)
        {
            UIDocument uiDoc = uiapp.ActiveUIDocument;

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

            string resourceFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

            string mappingTablePath = Path.Combine(resourceFolderPath, "IFC Mapping Table_Parameters.txt");

            string familyMappingFilePath = Path.Combine(resourceFolderPath, "IFC Export Class Mapping Table.txt");

            newConfig.ExportUserDefinedParameterMappingFileName = mappingTablePath;

            int count = 0;

            docsFailedToExport = new List<string>();

            docsIsExported = new List<string>();

            revitDocsExported = new List<string>();

            foreach (Document doc in documentsToExport)
            {
                count++;

                string fileName = Path.GetFileNameWithoutExtension(doc.PathName) + ".ifc";

                string fullFileName = fileSavePath + "\\" + fileName;

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
                    ValidateStatusBar newStatusBar = new ValidateStatusBar(docsIsExported, ER_Name, mvdFilePath, fileSavePath);

                    bool? validateResult = newStatusBar.ShowDialog();

                    if (validateResult.HasValue && validateResult.Value)
                    {
                        ShowValidateResultInRevit(newStatusBar.csvFileNames, uiapp);
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

        private void ShowValidateResultInRevit(List<string> csvSavePath, UIApplication uiapp)
        {

            ExternalEvent resetEvent = ExternalEvent.Create(new ResetViewEventHandler());

            ExternalEvent zoomToFitEvent = ExternalEvent.Create(new ZoomToFit());

            ShowElementsEventHandler showElementsEventHandler = new ShowElementsEventHandler();

            ExternalEvent showEvent = ExternalEvent.Create(showElementsEventHandler);

            FillInfoEventHandler fillInfoEventHandler = new FillInfoEventHandler();

            ExternalEvent fillEvent = ExternalEvent.Create(fillInfoEventHandler);

            ResultWindow resultWindow = new ResultWindow(revitDocsExported, csvSavePath, uiapp, resetEvent, zoomToFitEvent, showEvent, showElementsEventHandler, fillEvent, fillInfoEventHandler);

            revalidateEventHandler = new ReValidateEventHandler();

            revalidateEvent = ExternalEvent.Create(revalidateEventHandler);

            resultWindow.Closed += ResultWindow_Closed;

            resultWindow.Show();
        }

        private void ResultWindow_Closed(object sender, EventArgs e)
        {
            ResultWindow resultWindow = sender as ResultWindow;

            if (resultWindow.reValidate)
            {
                IList<Document> newDocumentsToExport = new List<Document>();

                foreach (var doc in documentsToExport)
                {
                    if (docsIsExported.Contains(fileSavePath + "\\" + Path.GetFileNameWithoutExtension(doc.PathName) + ".ifc"))
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

        public string GetName()
        {
            return "Fill Info Event";
        }
    }
}
