using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.UI.Windows;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for IFCExport_2.xaml
    /// </summary>
    public partial class IFCExport_2 : ChildWindow
    {
        Document doc_1;
        public IFCExport_2(Document doc)
        {
            InitializeComponent();

            doc_1= doc;
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc_1).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType();

            string fileName = Path.GetFileNameWithoutExtension(doc_1.PathName) + ".ifc";

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            IFCExportOptions ExportOptions=new IFCExportOptions();

            using (Transaction transaction = new Transaction(doc_1, "Add Parameter"))
            {
                transaction.Start();

                doc_1.Export(filePath, fileName, ExportOptions);

                transaction.Commit();
            }

            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
