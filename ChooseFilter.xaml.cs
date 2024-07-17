using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Xbim.Ifc4.HvacDomain;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for ChooseFilter.xaml
    /// </summary>
    public partial class ChooseFilter : ChildWindow
    {
        public List<List<Filter>> filters;

        public List<bool> filterApplied = new List<bool> { false, false, false, false, false };

        public string Operator = null;

        List<ValidateResult> rawData;

        List<List<string>> easyAccessData;

        List<ValidateResultsByElement> resultsByElement;
             
        public ChooseFilter(List<ValidateResult> data, List<List<Filter>> filtersSelected, List<bool> filterApply, string operatorSelected, List<ValidateResultsByElement> ResultsByElement)
        {
            InitializeComponent();

            rawData = data;

            filters = filtersSelected;

            filterApplied = filterApply;

            Operator = operatorSelected;

            resultsByElement = ResultsByElement;

            InitializeAllFilters();

            GetEasyAccessData();
            
        }

        private void GetEasyAccessData()
        {

            easyAccessData = new List<List<string>>();

           
            foreach (var record in rawData)
            {
                List<string> line = new List<string>();

                line.Add(record.Id);
                line.Add(record.IfcType);
                line.Add(record.ConceptRootName);
                line.Add(record.ConceptName);
                line.Add(record.Result);

                easyAccessData.Add(line);
            }
        }

        private void InitializeAllFilters()
        {/*
            filters = new List<List<Filter>>();

            List<Filter> IfcInstanceFilters = new List<Filter>();

            IfcInstanceFilters.Add(new Filter("Select All", true));

            List<Filter> IfcTypeFilters = new List<Filter>();

            IfcTypeFilters.Add(new Filter("Select All", true));

            List<Filter> ConceptRootFilters = new List<Filter>();

            ConceptRootFilters.Add(new Filter("Select All", true));

            List<Filter> ConceptFilters = new List<Filter>();

            ConceptFilters.Add(new Filter("Select All", true));

            List<Filter> ResultFilters = new List<Filter>();

            ResultFilters.Add(new Filter("Select All", true));

            List<string> IfcIds = new List<string>();

            List<string> IfcTypes = new List<string>();

            List<string> ConceptRoots = new List<string>();

            List<string> Concepts = new List<string>();

            List<string> Results = new List<string>();

            foreach (var record in rawData)
            {

                IfcIds.Add(record.Id);
                IfcTypes.Add(record.IfcType);
                ConceptRoots.Add(record.ConceptRootName);
                Concepts.Add(record.ConceptName);
                Results.Add(record.Result);
            }

            IfcIds = IfcIds.Distinct().ToList();
            IfcTypes = IfcTypes.Distinct().ToList();
            ConceptRoots = ConceptRoots.Distinct().ToList();
            Concepts = Concepts.Distinct().ToList();
            Results = Results.Distinct().ToList();

            foreach (string item in IfcIds)
            {
                IfcInstanceFilters.Add(new Filter(item, true));
            }

            foreach (string item in IfcTypes)
            {
                IfcTypeFilters.Add(new Filter(item, true));
            }

            foreach (string item in ConceptRoots)
            {
                ConceptRootFilters.Add(new Filter(item, true));
            }

            foreach (string item in Concepts)
            {
                ConceptFilters.Add(new Filter(item, true));
            }

            foreach (string item in Results)
            {
                ResultFilters.Add(new Filter(item, true));
            }

            filters.Add(IfcInstanceFilters);
            filters.Add(IfcTypeFilters);
            filters.Add(ConceptRootFilters);
            filters.Add(ConceptFilters);
            filters.Add(ResultFilters);*/

            IfcIdFilter.ItemsSource = filters[0];
            IfcTypeFilter.ItemsSource = filters[1];
            ConceptRootFilter.ItemsSource = filters[2];
            ConceptFilter.ItemsSource = filters[3];
            ResultFilter.ItemsSource = filters[4];

            SolidColorBrush brush = new SolidColorBrush(Colors.LightYellow);

            if (filterApplied[0]==true)
            {
                Ele_Apply.Background = brush;
            }
            if (filterApplied[1] == true)
            {
                Type_Apply.Background = brush;
            }
            if (filterApplied[2] == true)
            {
                Root_Apply.Background = brush;
            }
            if (filterApplied[3] == true)
            {
                Concept_Apply.Background = brush;
            }
            if (filterApplied[4] == true)
            {
                Result_Apply.Background = brush;
            }

            if (Operator == "or")
            {
                @operator.SelectedIndex = 1;
            }
            else if (Operator == "and")
            {
                @operator.SelectedIndex = 0;
            }

        }

        private void operator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(@operator.SelectedIndex==1)
            {
                Operator = "or";
            }
            else
            {
                Operator = "and";
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ele in resultsByElement)
            {
                ele.Hide = true;
            }

            foreach (var record in rawData)
            {
                record.Hide = true;
            }

            foreach(var filter in filters)
            {
                filter[0].IsSelected = true;
            }
        }

        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            string buttonName = button.Name;

            SolidColorBrush brush = new SolidColorBrush(Colors.LightYellow);

            button.Background = brush;

            switch(buttonName)
            {
                case "Ele_Apply":
                    filterApplied[0] = true;
                    break;
                case "Type_Apply":
                    filterApplied[1] = true;
                    break;
                case "Root_Apply":
                    filterApplied[2] = true;
                    break;
                case "Concept_Apply":
                    filterApplied[3] = true;
                    break;
                case "Result_Apply":
                    filterApplied[4] = true;
                    break;
                default:
                    MessageBox.Show("Error!");
                    break;
            }

            ApplyFilters();
            
            
        }

        private void ApplyFilters()
        {
            List<List<string>> sumFilterNames = new List<List<string>>();

            for (int i = 0; i < filterApplied.Count; i++)
            {
                List<string> filterNames = new List<string>();

                if (filterApplied[i])
                {
                    filterNames = GetFilterNames(GetActiveFilters(filters[i]));
                }

                sumFilterNames.Add(filterNames);
            }

            if (Operator == "or")
            {
                for (int lineNum = 0; lineNum < easyAccessData.Count; lineNum++)
                {
                    for (int filterNum = 0; filterNum < filters.Count; filterNum++)
                    {
                        if (BeingFiltered(sumFilterNames[filterNum], filterNum, lineNum))
                        {
                            rawData[lineNum].Hide = true;
                            break;
                        }
                        else
                        {
                            rawData[lineNum].Hide = false;
                        }
                    }
                }
            }

            else
            {
                for (int lineNum = 0; lineNum < easyAccessData.Count; lineNum++)
                {
                    for (int filterNum = 0; filterNum < filters.Count; filterNum++)
                    {
                        if (!BeingFiltered(sumFilterNames[filterNum], filterNum, lineNum))
                        {
                            rawData[lineNum].Hide = false;
                            break;
                        }
                        else
                        {
                            rawData[lineNum].Hide = true;
                        }
                    }
                }

                /*                foreach (var record in rawData)
                                {
                                    if (PassFilter(record)
                                        sumFilterNames[0].Contains(record.Id) &&
                                        sumFilterNames[1].Contains(record.IfcType) &&
                                        sumFilterNames[2].Contains(record.ConceptRootName) &&
                                        sumFilterNames[3].Contains(record.ConceptName) &&
                                        sumFilterNames[4].Contains(record.Result))
                                    {
                                        record.Hide = true;
                                    }
                                    else
                                    {
                                        record.Hide = false;
                                    }
                                }*/

                UpdateFilters();
            }

            UpdateExpander();
        }

        private void UpdateExpander()
        {
            for (int i=0;i<resultsByElement.Count;i++)
            {
/*                if (filters[0][i].IsSelected==false)
                {
                    resultsByElement[i].Hide = false;
                    continue;
                }*/

                if (AllFalse(resultsByElement[i]))
                {
                    resultsByElement[i].Hide = false; //If all elements are hidden, hide the expander as well
                }
                else
                {
                    resultsByElement[i].Hide = true; //If all elements are shown, show the expander as well
                }
            }
        }


/*        //Check if all record of an element is shown
        private bool AllTrue(ValidateResultsByElement ele)
        {
            foreach (var record in ele.Results) //loop through all records of an element
            {
                if(record.Hide==false) //If one of the element is hidden
                {
                    return false; //Then not all element is shown, return false
                }
            }

            return true; //If all elements are shown, return true
        }*/

        //Check if all record of an element is hidden
        private bool AllFalse(ValidateResultsByElement ele)
        {
            foreach (var record in ele.Results) //loop through all records of an element
            {
                if (record.Hide == true) //If one of the element is shown
                {
                    return false; //Then not all element is hidden, return false
                }
            }

            return true; //If all elements are hidden, return true
        }

        private void UpdateFilters()
        {
            List<List<string>> activeData = new List<List<string>>();

            activeData.Add(new List<string>());
            activeData.Add(new List<string>());
            activeData.Add(new List<string>());
            activeData.Add(new List<string>());
            activeData.Add(new List<string>());

            for (int i=0;i<rawData.Count;i++)
            {
                if (rawData[i].Hide==true)
                {
                    for(int j=0;j<activeData.Count;j++)
                    {
                        activeData[j].Add(easyAccessData[i][j]);
                    }
                }        
            }

            for (int i = 0; i < activeData.Count; i++)
            {
                activeData[i]=activeData[i].Distinct().ToList();

                for (int j = 1; j < filters[i].Count; j++)
                {
                    if (activeData[i].Contains(filters[i][j].Name))
                    {
                        filters[i][j].IsSelected = true;
                    }
                    else
                    {
                        filters[i][j].IsSelected = false;
                    }
                }
            }
        }

        private bool BeingFiltered(List<string> filter,int filterNum, int lineNum)
        {
            if (filterApplied[filterNum])
            {
                return filter.Contains(easyAccessData[lineNum][filterNum]);
            }
            else
            {
                return true; //True means pass, false means fail
            }
        }

        private List<Filter> GetActiveFilters(List<Filter> filters)
        {
            List<Filter> activeFilter = new List<Filter>();

            for (int i = 1; i < filters.Count; i++)
            {
                if (filters[i].IsSelected == true)
                {
                    activeFilter.Add(filters[i]);
                }
            }

            return activeFilter;

        }

        private List<string> GetFilterNames(List<Filter> filters)
        {
            List<string> filterNames = new List<string>();

            foreach (Filter filter in filters)
            {
                filterNames.Add(filter.Name);
            }

            return filterNames;

        }

        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            string buttonName = button.Name;

            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(221, 221, 221));

            switch (buttonName)
            {
                case "Ele_Clear":
                    filterApplied[0] = false;
                    filters[0][0].IsSelected = true;
                    Ele_Apply.Background = brush;
                    break;
                case "Type_Clear":
                    filterApplied[1] = false;
                    filters[2][0].IsSelected = true;
                    Type_Apply.Background= brush;
                    break;
                case "Root_Clear":
                    filterApplied[2] = false;
                    filters[3][0].IsSelected = true;
                    Root_Apply.Background = brush;
                    break;
                case "Concept_Clear":
                    filterApplied[3] = false;
                    filters[3][0].IsSelected = true;
                    Concept_Apply.Background = brush;
                    break;
                case "Result_Clear":
                    filterApplied[4] = false;
                    filters[4][0].IsSelected = true;
                    Result_Apply.Background = brush;
                    break;
                default:
                    MessageBox.Show("Error!");
                    break;
            }

            ApplyFilters();

        }
        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() == "Select All")
            {
                for (int i = 1; i < filters[0].Count; i++)
                {
                    if (filters[0][i].IsSelected == false)
                    {
                        return;
                    }
                }

                for (int i = 1; i < filters[0].Count; i++)
                {
                    filters[0][i].IsSelected = false;
                }
            }
            else
            {
                filters[0][0].IsSelected = false;
            }
        }
        private void CheckBox_Unchecked_2(object sender, RoutedEventArgs e)
        {        
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() == "Select All")
            {
                for (int i = 1; i < filters[1].Count; i++)
                {
                    if (filters[1][i].IsSelected == false)
                    {
                        return;
                    }
                }
                for (int i = 1; i < filters[1].Count; i++)
                {

                    filters[1][i].IsSelected = false;
                }
            }
            else
            {
                filters[1][0].IsSelected = false;
            }
        }
        private void CheckBox_Unchecked_3(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() == "Select All")
            {
                for (int i = 1; i < filters[2].Count; i++)
                {
                    if (filters[2][i].IsSelected == false)
                    {
                        return;
                    }
                }
                for (int i = 1; i < filters[2].Count; i++)
                {

                    filters[2][i].IsSelected = false;
                }
            }
            else
            {
                filters[2][0].IsSelected = false;
            }
        }
        private void CheckBox_Unchecked_4(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() == "Select All")
            {
                for (int i = 1; i < filters[3].Count; i++)
                {
                    if (filters[3][i].IsSelected == false)
                    {
                        return;
                    }
                }

                for (int i = 1; i < filters[3].Count; i++)
                {

                    filters[3][i].IsSelected = false;
                }
            }
            else
            {
                filters[3][0].IsSelected = false;
            }
        }
        private void CheckBox_Unchecked_5(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() == "Select All")
            {
                for (int i = 1; i < filters[4].Count; i++)
                {
                    if (filters[4][i].IsSelected == false)
                    {
                        return;
                    }
                }

                for (int i = 1; i < filters[4].Count; i++)
                {

                    filters[4][i].IsSelected = false;
                }
            }
            else
            {
                filters[4][0].IsSelected = false;
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() != "Select All")
            {
                for (int i = 1; i < filters[0].Count(); i++)
                {
                    if (filters[0][i].IsSelected == false)
                    {
                        return;
                    }
                }

                filters[0][0].IsSelected = true;
            }
            else
            {
                for (int i = 1; i < filters[0].Count(); i++)
                {
                    filters[0][i].IsSelected = true;
                }
            }
        }

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() != "Select All")
            {
                for (int i = 1; i < filters[1].Count(); i++)
                {
                    if (filters[1][i].IsSelected == false)
                    {
                        return;
                    }
                }

                filters[1][0].IsSelected = true;
            }
            else
            {
                for (int i = 1; i < filters[1].Count(); i++)
                {
                    filters[1][i].IsSelected = true;
                }
            }
        }

        private void CheckBox_Checked_3(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() != "Select All")
            {
                for (int i = 1; i < filters[2].Count(); i++)
                {
                    if (filters[2][i].IsSelected == false)
                    {
                        return;
                    }
                }

                filters[2][0].IsSelected = true;
            }
            else
            {
                for (int i = 1; i < filters[2].Count(); i++)
                {
                    filters[2][i].IsSelected = true;
                }
            }
        }

        private void CheckBox_Checked_4(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() != "Select All")
            {
                for (int i = 1; i < filters[3].Count(); i++)
                {
                    if (filters[3][i].IsSelected == false)
                    {
                        return;
                    }
                }

                filters[3][0].IsSelected = true;
            }
            else
            {
                for (int i = 1; i < filters[3].Count(); i++)
                {
                    filters[3][i].IsSelected = true;
                }
            }
        }

        private void CheckBox_Checked_5(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() != "Select All")
            {
                for (int i = 1; i < filters[4].Count(); i++)
                {
                    if (filters[4][i].IsSelected == false)
                    {
                        return;
                    }
                }

                filters[4][0].IsSelected = true;
            }
            else
            {
                for (int i = 1; i < filters[4].Count(); i++)
                {
                    filters[4][i].IsSelected = true;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult= true;
        }

/*        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = Tab.Items.Count - 1; i >= 0; i--)
            {
                Tab.SelectedIndex = i;
            }
        }*/
    }
}
