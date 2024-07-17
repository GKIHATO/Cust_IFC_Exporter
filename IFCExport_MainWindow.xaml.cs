using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for IFCExport_MainWindow.xaml
    /// </summary>
    /// 

    public partial class IFCExport_MainWindow : ChildWindow
    {
        private UIApplication _uiapp;

        public string fileSavePath = null;

        static public IList<Document> documentsToExport = null;
        static public int docsExportCount { get; set; }

        public bool? exportAsOfficialMVDs = null;

        public string ER_Name = null;

        public string MVD_Name= null;

        public IFCVersion ifcVersion;

        public string mvdFilePath = null;

        public IFCExport_MainWindow(UIApplication uiapp)
        {
            InitializeComponent();
            documentsToExport = new List<Document>();
            _uiapp = uiapp;
        }

        private void selectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IFCExport_SelectFile selectFileWindow = new IFCExport_SelectFile(_uiapp);

            selectFileWindow.Owner = this;

            filesSelected.Text = null;

            filesSelected.Height = 25;

            docsExportCount = 0;

            documentsToExport = null;

            bool? fileSelected = selectFileWindow.ShowDialog();

            if (fileSelected.HasValue && fileSelected.Value)
            {
                if(docsExportCount>1)
                {
                    filesSelected.Height = docsExportCount * 18 + 7;

                    foreach (Document doc in documentsToExport)
                    {
                        filesSelected.Text += Path.GetFileName(doc.PathName)+"\n";
                    }
                }
                else
                {
                    filesSelected.Text = Path.GetFileName(documentsToExport[0].PathName);
                }
            }
            else
            {
                string message = "No file selected, please select at least one file to export!";

                MessageBox.Show(message);
            }

        }

        private void browseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FolderBrowserDialog saveFileDialog = new FolderBrowserDialog(); 

            saveFileDialog.RootFolder = Environment.SpecialFolder.Desktop;

            DialogResult dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult==System.Windows.Forms.DialogResult.OK)
            {
                filesSaveLocation.Text = saveFileDialog.SelectedPath;
                fileSavePath = saveFileDialog.SelectedPath;
            }
            else
            {
                string message = "Please select a file save path!";

                MessageBox.Show(message);
            }
        }

        private void chooseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IFCExport_TargetMVD chooseMVDWindow = new IFCExport_TargetMVD();

            chooseMVDWindow.Owner = this;

            bool? mvdChosen = chooseMVDWindow.ShowDialog();

            if( mvdChosen.HasValue && mvdChosen.Value)
            {
                selectedMVD.Text = chooseMVDWindow.MVD_Name;

                exportAsOfficialMVDs = chooseMVDWindow.exportAsOffcialMVDs;

                MVD_Name = chooseMVDWindow.MVD_Name;

                ER_Name = chooseMVDWindow.ER_Name;

                ifcVersion = chooseMVDWindow.ifcVersion;

                mvdFilePath = chooseMVDWindow.mvdFilePath;

            }
            else
            {
                string message = "Please select a target MVD!";

                MessageBox.Show(message);
            }
        }

        
        private void exportButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (docsExportCount != 0 && fileSavePath != null && exportAsOfficialMVDs.HasValue)
            {
                DialogResult = true;
            }
            else
            {
                string message = "Key information is missing!";

                MessageBox.Show(message);
            }

        }   

        private void cancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
