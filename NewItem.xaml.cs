using Autodesk.UI.Windows;
using Microsoft.Isam.Esent.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    /// Interaction logic for NewItem.xaml
    /// </summary>
    public partial class NewItem : ChildWindow
    {
        //public anyChange anyChange = anyChange.noChange;

        public ObservableCollection<TreeNode<string>> materialTypeList = new ObservableCollection<TreeNode<string>>();

        public ObservableCollection<TreeNode<string>> manufacturerList = new ObservableCollection<TreeNode<string>>();

        public ObservableCollection<TreeNode<string>> regionList = new ObservableCollection<TreeNode<string>>();

        public List<string> selectedMaterialTypes = new List<string>();

        //public Dictionary<string,Dictionary<string,List<string>>> selectedRegions = new Dictionary<string, Dictionary<string, List<string>>>();

        public List<string> selectedCities = new List<string>();

        public List<string> selectedManufacturers = new List<string>();

        SQLDBConnect connect;

        public NewItem(SQLDBConnect connection)
        {
            InitializeComponent();

            startDate.SelectedDate = DateTime.Now.Date;

            endDate.SelectedDate = DateTime.Now.Date;

            connect = connection;

            materialTypeList = connect.GetMaterialTypes(selectedManufacturers, selectedCities, startDate.SelectedDate.Value, endDate.SelectedDate.Value);

            manufacturerList = connect.GetManufacturerList(selectedMaterialTypes, selectedCities, startDate.SelectedDate.Value, endDate.SelectedDate.Value);

            regionList = connect.GetRegionList(selectedMaterialTypes, selectedManufacturers, startDate.SelectedDate.Value, endDate.SelectedDate.Value);

            /*List<string> materialTypeList = connect.GetMaterialType();

            materialTypes = GenerateFilter(materialTypeList);*/
        }

        public NewItem(SQLDBConnect connection, RowItem rowItem)
        {
            InitializeComponent();

            connect = connection;

            selectedMaterialTypes = rowItem.selectedTypes;

            selectedCities = rowItem.selectedCities;

            selectedManufacturers = rowItem.selectedManufacturers;

            //materialType.Text = rowItem.MaterialType;

            //manufacturers.Text = rowItem.Manufacturer;

            //producedLocations.Text = rowItem.Region;

            string[] dates = rowItem.DateRange.Split(new[] { " - " }, StringSplitOptions.None);

            //anyChange = lastChange;

            DateTime startDateParseValue;

            bool result_1 = DateTime.TryParse(dates[0], out startDateParseValue);

            if (result_1)
            {
                this.startDate.SelectedDate = startDateParseValue;
            }
            else
            {
                MessageBox.Show("Error! Can't parse the start date!");
            }

            DateTime endDateParseValue;

            bool result_2 = DateTime.TryParse(dates[1], out endDateParseValue);

            if (result_2)
            {
                this.endDate.SelectedDate = endDateParseValue;
            }
            else
            {
                MessageBox.Show("Error! Can't parse the end date!");
            }

            materialTypeList = rowItem.materialTypeList;

            showTypes();

            manufacturerList = rowItem.manufacturerList;

            showManufacturers();

            regionList = rowItem.regionList;

            shownRegions();
        }

        private void selectType_Click(object sender, RoutedEventArgs e)
        {
/*            if (anyChange != anyChange.materialTypeChanged)
            {
                UpdateTheOptions();
            }*/


            if (materialTypeList.Count == 0)
            {
                MessageBox.Show("Error: No satisfied records can be found! Please adjust the constraint and try again!");
                return;
            }

            SelectTreeWindow newSelectionWindow = new SelectTreeWindow(materialTypeList);

            bool? result = newSelectionWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                UpdateSelectedTypes();

                showTypes();

                //anyChange=anyChange.materialTypeChanged;

                UpdateTheOptions(1);
            }
        }

        private void UpdateSelectedTypes()
        {
            selectedMaterialTypes.Clear();

            foreach (var item in materialTypeList)
            {
                if (item.IsSelected == true && item.IsVisible == true)
                {
                    selectedMaterialTypes.Add(item.Data);
                }
            }
        }

        private void showTypes()
        {
            if (selectedMaterialTypes.Count > 1)
            {
                materialType.Text = string.Join(",", selectedMaterialTypes);

                selectManufacturers.IsEnabled = false;

                manufacturers.Text = "Can't choose manufacturer if mutiple categories are selected";

                manufacturers.IsEnabled = false;
            }
            else if (selectedMaterialTypes.Count == 1)
            {
                materialType.Text = string.Join(",", selectedMaterialTypes);

                selectManufacturers.IsEnabled = true;

                manufacturers.Text = "";

                manufacturers.IsEnabled = true;
            }
            else
            {
                materialType.Text = string.Empty;

                selectManufacturers.IsEnabled = true;

                manufacturers.Text = "";

                manufacturers.IsEnabled = true;
            }
        }

        private void selectManufacturer_Click(object sender, RoutedEventArgs e)
        {

            if (selectedMaterialTypes.Count > 1)
            {
                return;
            }

            /*            if (anyChange != anyChange.manufacturerChanged)
                        {
                            manufacturerList = connect.GetManufacturerList(selectedMaterialTypes, selectedRegions, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
                        }*/

            if (manufacturerList.Count == 0)
            {
                MessageBox.Show("Error: No satisfied records can be found! Please adjust the constraint and try again!");
                return;
            }

            SelectTreeWindow newSelectionWindow = new SelectTreeWindow(manufacturerList);

            bool? result = newSelectionWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                UpdateSelectedManufacturers();

                showManufacturers();

                UpdateTheOptions(2);

                //anyChange = anyChange.manufacturerChanged;
            }
        }

        private void UpdateSelectedManufacturers()
        {
            selectedManufacturers.Clear();

            foreach (var item in manufacturerList)
            {
                if (item.IsSelected == true && item.IsVisible == true)
                {
                    selectedManufacturers.Add(item.Data);
                }
            }
        }

        private void showManufacturers()
        {
            if (selectedManufacturers.Count > 0)
            {
                manufacturers.Text = string.Join(",", selectedManufacturers);
            }
            else
            {
                manufacturers.Text = string.Empty;
            }
        }

        private void selectRegion_Click(object sender, RoutedEventArgs e)
        {

            /*            if(anyChange!=anyChange.regionChanged)
                        {
                            regionList = connect.GetRegionList(selectedMaterialTypes, selectedManufacturers, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
                        }*/

            if (regionList.Count == 0)
            {
                MessageBox.Show("Error: No satisfied records can be found! Please adjust the constraint and try again!");
                return;
            }

            SelectTreeWindow newSelectionWindow = new SelectTreeWindow(regionList);

            bool? result = newSelectionWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                //selectedRegions.Clear();

                UpdateSelectedCities();

                shownRegions();

                UpdateTheOptions(3);

                //anyChange = anyChange.regionChanged;
            }
        }

        private void UpdateSelectedCities()
        {
            selectedCities.Clear();

            foreach (var country in regionList)
            {
                if (country.IsSelected != false)
                {
                    foreach (var state in country.Children)
                    {
                        if (state.IsSelected != false)
                        {
                            foreach (var city in state.Children)
                            {
                                if (city.IsSelected == true && city.IsVisible == true)
                                {
                                    selectedCities.Add(city.Data);
                                }
                            }
                        }
                    }
                }

            }
        }

        private void shownRegions()
        {
            if (selectedCities.Count > 0)
            {
                producedLocations.Text = GetRegionNames();

            }
            else
            {
                producedLocations.Text = string.Empty;
            }
        }

        private string GetRegionNames()
        {
            List<string> regionNames = new List<string>();

            foreach (var country in regionList)
            {
                if (country.IsSelected == true)
                {
                    regionNames.Add(country.Data+" [Whole]");

                }
                else if(country.IsSelected == null)
                { 
                    List<string> stateNames = new List<string>();

                    foreach (var state in country.Children)
                    {
                        if (state.IsSelected == true)
                        {
                            stateNames.Add(state.Data+ " (Whole)");
                        }
                        else if(state.IsSelected==null)
                        {
                            List<string> cityNames = new List<string>();

                            foreach (var city in state.Children)
                            {
                                if (city.IsSelected == true)
                                {
                                    cityNames.Add( city.Data);
                                }
                            }

                            string cityNamesJoined = string.Join(",", cityNames);

                            stateNames.Add(state.Data + " (" + cityNamesJoined + ")");
                        }
                    }

                    string stateNamesJoined = string.Join(",", stateNames);

                    regionNames.Add(country.Data + " [" + stateNamesJoined + "]");
                }
            }

            return string.Join(",", regionNames);
        }

        private void UpdateTheOptions(int Mode)
        {
            switch (Mode)
            {
                case 1:
                    {
                        var newManufacturerList = connect.GetManufacturerList(selectedMaterialTypes, selectedCities, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
                        var newRegionList = connect.GetRegionList(selectedMaterialTypes, selectedManufacturers, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
                        DisableUnWantedNodes(newManufacturerList,2);
                        DisableUnWantedNodes(newRegionList,3);
                    }
                        break;
                case 2:
                    {
                        var newRegionList = connect.GetRegionList(selectedMaterialTypes, selectedManufacturers, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
                        var newMaterialTypeList = connect.GetMaterialTypes(selectedManufacturers, selectedCities, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
                        DisableUnWantedNodes(newRegionList,3);
                        DisableUnWantedNodes(newMaterialTypeList,1);
                    }
                    break;
                case 3:
                    {
                        var newMaterialTypeList = connect.GetMaterialTypes(selectedManufacturers, selectedCities, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
                        var newManufacturerList = connect.GetManufacturerList(selectedMaterialTypes, selectedCities, startDate.SelectedDate.Value, endDate.SelectedDate.Value);
                        DisableUnWantedNodes(newMaterialTypeList,1);
                        DisableUnWantedNodes(newManufacturerList,2);
                    }
                    break;
                default:
                    break;
            }
        }

        private void DisableUnWantedNodes(ObservableCollection<TreeNode<string>> newList, int listNumber)
        {
            switch (listNumber)
            {
                case 1: 
                    {
                        List<string> newListNames = new List<string>();

                        foreach (var item in newList)
                        {
                            newListNames.Add(item.Data);
                        }

                        foreach(var item in materialTypeList)
                        { 
                            if(newListNames.Contains(item.Data))
                            {
                                item.IsVisible = true;
                            }
                            else
                            {
                                item.IsVisible = false;
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        List<string> newListNames = new List<string>();

                        foreach (var item in newList)
                        {
                            newListNames.Add(item.Data);
                        }

                        foreach (var item in manufacturerList)
                        {

                            if (newListNames.Contains(item.Data))
                            {
                                item.IsVisible = true;
                            }
                            else
                            {
                                item.IsVisible = false;
                            }
                        }
                    }
                    break;
                case 3:
                    {
                        List<string> newCityNameList = new List<string>();

                        List<string> newStateNameList = new List<string>();

                        List<string> newCountryNameList = new List<string>();

                        foreach (var country in newList)
                        {
                            newCountryNameList.Add(country.Data);

                            foreach (var state in country.Children)
                            {
                                newStateNameList.Add(state.Data);

                                foreach (var city in state.Children)
                                { 
                                    newCityNameList.Add(city.Data);
                                }
                            }
                        }

                        foreach (var country in regionList)
                        {
                            if(newCountryNameList.Contains(country.Data))
                            {
                                country.IsVisible = true;
                            }
                            else
                            {
                                country.IsVisible = false;
                            }

                            foreach(var state in country.Children)
                            {
                                if(newStateNameList.Contains(state.Data))
                                {
                                    state.IsVisible = true;
                                }
                                else
                                {
                                    state.IsVisible = false;
                                }

                                foreach(var city in state.Children)
                                {
                                    if(newCityNameList.Contains(city.Data))
                                    {
                                        city.IsVisible = true;
                                    }
                                    else
                                    {
                                        city.IsVisible = false;
                                    }
                                }

                            }
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        /*private IEnumerable<string> GetSelectedChildren(TreeNode<string> item)
        {
            List<string> selectedChildren = new List<string>();

            foreach(var child in item.Children)
            {
                if(child.Children.Count==0)
                {
                    if (child.IsSelected==true)
                    {
                        selectedChildren.Add(child.Data);
                    }
                }
                else
                {
                    selectedChildren.AddRange(GetSelectedChildren(child));
                }
            }

            return selectedChildren;
        }*/

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if(selectedMaterialTypes.Count==0)
            {
                MessageBox.Show("Error! Please select at least one material type!");
            }
            else
            {
                UpdateSelectedTypes();
                UpdateSelectedManufacturers();
                UpdateSelectedCities();
                DialogResult = true;
            }
           
        }



        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

/*        private List<Filter> GenerateFilter(List<string> list)
        {
            List<Filter> itemList = new List<Filter>();

            itemList.Add(new Filter("Select All", false));

            foreach (var item in list)
            {
                Filter newItem = new Filter(item, false);

                itemList.Add(newItem);
            }

            return itemList;
        }*/

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(startDate.SelectedDate>endDate.SelectedDate)
            {
                MessageBox.Show("Error! Start date can't be later than end date!");

                startDate.SelectedDate=endDate.SelectedDate;
            }
        }

        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startDate.SelectedDate > endDate.SelectedDate)
            {
                MessageBox.Show("Error! Start date can't be later than end date!");

                endDate.SelectedDate = startDate.SelectedDate;
            }
        }
    }

/*    public enum anyChange
    { 
        noChange=0,
        materialTypeChanged=1,
        manufacturerChanged=2,
        regionChanged=3,
        dateChanged=4,
    }*/

}
