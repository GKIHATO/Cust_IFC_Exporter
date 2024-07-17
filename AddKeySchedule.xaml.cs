using Autodesk.Revit.UI;
using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for AddKeySchedule.xaml
    /// </summary>
    public partial class AddKeySchedule : ChildWindow
    {
        SQLDBConnect connection;

        public bool generateAverageData = false;

        public List<RowItem> rowItems;

        public AddKeySchedule(SQLDBConnect connect, List<RowItem> items)
        {
            InitializeComponent();

            FilterTable.ItemsSource = items;

            rowItems = items;

            connection = connect;
        }

        private void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            NewItem newItem = new NewItem(connection);

            bool? dialogResult= newItem.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                RowItem newRow = new RowItem();

                newRow.MaterialType = newItem.materialType.Text;

                newRow.materialTypeList = newItem.materialTypeList;

                newRow.selectedTypes = newItem.selectedMaterialTypes;

                if (newItem.producedLocations.Text != "")
                {
                    newRow.Region = newItem.producedLocations.Text;
                }
                else
                {
                    newRow.Region = "No Limits";
                }

                newRow.regionList = newItem.regionList;

                newRow.selectedCities = newItem.selectedCities;

                if(newItem.selectedManufacturers.Count>0)
                {
                    newRow.Manufacturer = newItem.manufacturers.Text;
                }
                else
                {
                    newRow.Manufacturer = "No Limits";
                }

                newRow.manufacturerList = newItem.manufacturerList;

                newRow.selectedManufacturers = newItem.selectedManufacturers;

/*                List<string> selectedMaterialTypes = newItem.selectedMaterialTypes;

                List<string> selectedCities = newItem.selectedCities;

                List<string> selectedManufacturers = newItem.selectedManufacturers;

                string materialTypes;

                string regions;

                string manufacturers;*/

                newRow.DateRange= newItem.startDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd") + " - " + newItem.endDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");                

/*                if (selectedMaterialTypes.Count>1)
                {
                    materialTypes = string.Join(",", selectedMaterialTypes);
                }
                else
                {
                    materialTypes = selectedMaterialTypes[0];
                }
                
                if(selectedRegions.Count>1)
                {
                    regions= string.Join(",", selectedRegions);
                }
                else if(selectedRegions.Count == 1)
                {
                    regions = selectedRegions[0];
                }
                else
                {
                    regions = "No Limits";
                }

                if(selectedManufacturers.Count>1)
                {
                    manufacturers = string.Join(",", selectedManufacturers);
                }
                else if(selectedManufacturers.Count == 1)
                {
                    manufacturers = selectedManufacturers[0];
                }
                else
                {
                    manufacturers = "No Limits";
                }*/

                //RowItem newRow = new RowItem() { MaterialType = materialTypes, Region = regions, Manufacturer = manufacturers, DateRange = DateRange, materialTypeList=newItem.materialTypeList,manufacturerList=newItem.manufacturerList,regionList=newItem.regionList };

                rowItems.Add(newRow);

                FilterTable.Items.Refresh();
                
            }
        }

        private void ModifyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (FilterTable.SelectedItem == null)
            {
                return;
            }

            RowItem rowItem = FilterTable.SelectedItem as RowItem;
/*
            List<string> selectedMaterialTypes = rowItem.MaterialType.Split(',').ToList();

            List<string> selectedRegions;*/

            if (rowItem.Region == "No Limits")
            {
                rowItem.Region = "";
            }

            if(rowItem.Manufacturer=="No Limits")
            {
                rowItem.Manufacturer = "";
            }

           // List<string> selectedManufacturers;

/*            if (rowItem.Manufacturer == "No Limits")
            {
                selectedManufacturers = new List<string>();
            }
            else
            {
                selectedManufacturers = rowItem.Manufacturer.Split(',').ToList();
            }*/

            //string[] dates = rowItem.DateRange.Split(new[] { " - " }, StringSplitOptions.None);

            NewItem newItem = new NewItem(connection, rowItem);

            bool? dialogResult = newItem.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                /*selectedMaterialTypes = newItem.selectedMaterialTypes;

                selectedCities = newItem.selectedCities;

                selectedManufacturers = newItem.selectedManufacturers;

                string materialTypes;

                string regions;

                string manufacturers;*/

                if (newItem.producedLocations.Text != "")
                {
                    rowItem.Region = newItem.producedLocations.Text;
                }
                else
                {
                    rowItem.Region = "No Limits";
                }

                if (newItem.selectedManufacturers.Count>0)
                {
                    rowItem.Manufacturer = newItem.manufacturers.Text;
                }
                else
                {
                    rowItem.Manufacturer = "No Limits";
                }

                rowItem.DateRange = newItem.startDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd") + " - " + newItem.endDate.SelectedDate.Value.Date.ToString("yyyy-MM-dd");

                /*

                if (selectedMaterialTypes.Count > 1)
                {
                    materialTypes = string.Join(",", selectedMaterialTypes);
                }
                else
                {
                    materialTypes = selectedMaterialTypes[0];
                }

                if (selectedRegions.Count > 1)
                {
                    regions = string.Join(",", selectedRegions);
                }
                else if (selectedRegions.Count == 1)
                {
                    regions = selectedRegions[0];
                }
                else
                {
                    regions = "No Limits";
                }

                if (selectedManufacturers.Count > 1)
                {
                    manufacturers = string.Join(",", selectedManufacturers);
                }
                else if (selectedManufacturers.Count == 1)
                {
                    manufacturers = selectedManufacturers[0];
                }
                else
                {
                    manufacturers = "No Limits";
                }*/

                /*                rowItem.MaterialType = materialTypes;

                                rowItem.Region = regions;

                                rowItem.Manufacturer = manufacturers;

                                rowItem.DateRange = DateRange;

                                rowItem.materialTypeList = newItem.materialTypeList;

                                rowItem.manufacturerList = newItem.manufacturerList;

                                rowItem.regionList = newItem.regionList;*/

                //rowItem.lastChange = newItem.anyChange;

                FilterTable.Items.Refresh();
            }
        }

        private void DeleteFilter_Click(object sender, RoutedEventArgs e)
        {
            if (FilterTable.SelectedItem == null)
            {
                return;
            }

            var items=FilterTable.ItemsSource as List<RowItem>;
            
            if (items != null)
            {
                items.Remove(FilterTable.SelectedItem as RowItem);

                FilterTable.Items.Refresh();
            }

        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            if (rowItems.Count == 0)
            {
                MessageBox.Show("No data is selected!");

                return;                   
            }

            if (useAverage.IsChecked.Value)
            {
                generateAverageData = true;               
            }

            DialogResult = true;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }

    public class RowItem
    { 
        public string MaterialType { get; set; }

        public List<string> selectedTypes { get; set; }

        public string Region { get; set; }

        public List<string> selectedCities { get; set; }

        public string Manufacturer { get; set; }

        public List<string> selectedManufacturers { get; set; }

        public string DateRange { get; set; }

        //public anyChange lastChange { get; set; }

        public ObservableCollection<TreeNode<string>> materialTypeList { get; set; }

        public ObservableCollection<TreeNode<string>> manufacturerList { get; set; }

        public ObservableCollection<TreeNode<string>> regionList { get; set; }
    }

}
