using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.IO;


namespace Cust_IFC_Exporter
{
    public class revitPlugin:IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = RibbonPanel(application);

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            if (panel.AddItem(new PushButtonData("Add Shared Parameter", "Add Shared Parameter", thisAssemblyPath, "Cust_IFC_Exporter.AddParameterCommand")) is PushButton addParaButton)
            {
                addParaButton.ToolTip = "Add supplementary IFC shared parameters";

                Uri uriWrench = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "Fix.png"));
                BitmapImage bitmapWrench = new BitmapImage(uriWrench);
                addParaButton.LargeImage = bitmapWrench;
            }

            if (panel.AddItem(new PushButtonData("Import Data", "Import data from SQL", thisAssemblyPath, "Cust_IFC_Exporter.ImportDataCommand")) is PushButton importDataButton)
            {
                importDataButton.ToolTip = "Import data from SQL database and save them as new materials";

                Uri uriAdd = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "add.png"));
                BitmapImage bitmapAdd = new BitmapImage(uriAdd);
                importDataButton.LargeImage = bitmapAdd;
            }

            if (panel.AddItem(new PushButtonData("Populate Data", "Populate Data", thisAssemblyPath, "Cust_IFC_Exporter.PopulateDataCommand")) is PushButton populateDataButton)
            {
                populateDataButton.ToolTip = "Populate data from materials into type";

                Uri uriPopulateData = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "populate.png"));
                BitmapImage bitmapPopulateData = new BitmapImage(uriPopulateData);
                populateDataButton.LargeImage = bitmapPopulateData;
            }

            if (panel.AddItem(new PushButtonData("IFC exporter", "IFC exporter", thisAssemblyPath, "Cust_IFC_Exporter.Command")) is PushButton IfcExportButton)
            {
                IfcExportButton.ToolTip = "Export Revit file to LCA-specific IFC format (MVD)";

                Uri uriIFCExporter = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "IFC logo.png"));
                BitmapImage bitmapIFCExporter = new BitmapImage(uriIFCExporter);
                IfcExportButton.LargeImage = bitmapIFCExporter;
            }



            return Result.Succeeded;
        }

        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            {
                string tab="UQ Civil";
                RibbonPanel ribbonPanel = null;

                try
                {
                    a.CreateRibbonTab(tab);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                try
                {
                    a.CreateRibbonPanel(tab, "IFC exporter");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                List<RibbonPanel> panels = a.GetRibbonPanels(tab);
                foreach(RibbonPanel p in panels.Where(p=>p.Name=="IFC exporter"))
                {
                    ribbonPanel = p;
                }

                return ribbonPanel;
            }
        }
    }
}
