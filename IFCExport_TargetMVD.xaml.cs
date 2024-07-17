using Autodesk.Revit.DB;
using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for IFCExport_TargetMVD.xaml
    /// </summary>
    public partial class IFCExport_TargetMVD : ChildWindow
    {
        public bool? exportAsOffcialMVDs = null;

        public IFCVersion ifcVersion;

        public string MVD_Name = null;

        public string mvdFilePath = null;

        List<mvd> officialMVDs = new List<mvd>();

        public string ER_Name= null;

        public IFCExport_TargetMVD()
        {
            InitializeComponent();

            officialMVDs.Add(new mvd { ifcVersion = IFCVersion.IFC2x3CV2, MVD_FullName = "IFC 2X3 CoordinationView 2.0", officialMVD = true, IsChecked = false }); ;
            officialMVDs.Add(new mvd { ifcVersion = IFCVersion.IFC2x3, MVD_FullName = "IFC 2X3 CoordinationView", officialMVD = true, IsChecked = false });
            officialMVDs.Add(new mvd { ifcVersion = IFCVersion.IFCCOBIE, MVD_FullName = "IFC 2X3 GSA Concept Design BIM 2010", officialMVD = true, IsChecked = false });
            officialMVDs.Add(new mvd { ifcVersion = IFCVersion.IFC2x3BFM, MVD_FullName = "IFC 2X3 Basic FM Handover View", officialMVD = true, IsChecked = false });
            officialMVDs.Add(new mvd { ifcVersion = IFCVersion.IFC2x2, MVD_FullName = "IFC 2X2 CoordinationView 2.0", officialMVD = true, IsChecked = false });
            officialMVDs.Add(new mvd { ifcVersion = IFCVersion.IFC2x3FM, MVD_FullName = "IFC 2X3 COBie 2.4 Design Deliverable View", officialMVD = true, IsChecked = false });
            officialMVDs.Add(new mvd { ifcVersion = IFCVersion.IFC4RV, MVD_FullName = "IFC4 Reference View", officialMVD = true, IsChecked = false });
            officialMVDs.Add(new mvd { ifcVersion = IFCVersion.IFC4DTV, MVD_FullName = "IFC4 Design Transfer View", officialMVD = true, IsChecked = false });

            defaultMVDs.ItemsSource = officialMVDs;

            lastCheckedItem = null;

        }

        private mvd GetMVDbyName(string Name)
        {
            foreach (mvd item in officialMVDs)
            {
                if (Name == item.MVD_FullName)
                {
                    return item;
                }
            }
            return new mvd { MVD_FullName = Name };
        }

        private mvd lastCheckedItem { get; set; }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog newDialog = new OpenFileDialog();

            newDialog.Filter = "mvdXML file | *.mvdXML";

            newDialog.DefaultExt = "mvdXML";

            newDialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();

            bool? dialogResult = newDialog.ShowDialog();

            if (dialogResult == true)
            {
                mvd item = defaultMVDs.SelectedItem as mvd;

                if (item != null)
                {
                    item.IsChecked = false;
                }

                fileLocation.Text = newDialog.FileName;

                //MVD_Name = System.IO.Path.GetFileNameWithoutExtension(newDialog.FileName);

                mvdFilePath = newDialog.FileName;

                exportAsOffcialMVDs = false;

                //Needs to be determined by its mvdfile, set default for current stage

                //ifcVersion = IFCVersion.Default;
            }
            else
            {   //Click cancel will empty previous selection

                fileLocation.Text = null;

                mvdFilePath = null;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (exportAsOffcialMVDs==false || ifcVersion==IFCVersion.IFC4RV)
            {
                //Open a dialog to define ER

                ChooseER newWindow = new ChooseER(mvdFilePath, (bool)exportAsOffcialMVDs);

                newWindow.Owner = this;

                bool? dialogResult = newWindow.ShowDialog();

                if(dialogResult.HasValue&&dialogResult.Value)
                {
                    ER_Name = newWindow.SelectedER_Name;

                    if (exportAsOffcialMVDs == false)
                    {
                        ifcVersion = newWindow.appliedIFCSchema;

                        MVD_Name = newWindow.realMVDName;
                    }

                    DialogResult = true;
                }
                else
                {
                     MessageBox.Show("Please select at least one exchane requirement model!");
                }
            }
            else
            {
                ER_Name = "NotDefined";

                DialogResult = true;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

            CheckBox checkedItem = sender as CheckBox;

            string checkedItemName = checkedItem.Content.ToString();

            mvd currentSelectedMVD = defaultMVDs.SelectedItem as mvd;

            mvd checkedMVD = GetMVDbyName(checkedItemName);            
            
            if (lastCheckedItem != null && lastCheckedItem!= checkedMVD)
            {
                lastCheckedItem.IsChecked = false;
            }
            
            lastCheckedItem = checkedMVD;

            if (currentSelectedMVD == null || currentSelectedMVD.MVD_FullName != checkedItemName)
            {
                defaultMVDs.SelectedItem = checkedMVD;
            }

            if (fileLocation.Text != null)
            {
                fileLocation.Text = null;
            }

            exportAsOffcialMVDs = true;

            ifcVersion = checkedMVD.ifcVersion;

            MVD_Name = checkedMVD.MVD_FullName;

            // Can add default mvdXML path of the official MVDs
            mvdFilePath = null;

        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            exportAsOffcialMVDs = null;

            ifcVersion = IFCVersion.Default;

            MVD_Name = null;

            defaultMVDs.SelectedItem = null;
        }
    }

    public class mvd : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string MVD_FullName { get; set; }

        public IFCVersion ifcVersion { get; set; }

        public bool officialMVD { get; set; }

        private bool _IsChecked = false;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; OnPropertyChanged(); }
        }

        protected void OnPropertyChanged( string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }

}
