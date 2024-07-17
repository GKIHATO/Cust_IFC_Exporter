using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
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
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.ComponentModel;
using ComboBox = System.Windows.Controls.ComboBox;
using Xbim.Common;
using System.Activities.Expressions;
using Color = System.Windows.Media.Color;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using System.Text.RegularExpressions;
using System.Drawing;
using Autodesk.Revit.DB.Structure;
using ListViewItem = System.Windows.Controls.ListViewItem;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : ChildWindow
    {

        List<string> _revitDocs;

        List<string> _fileSavePath;

        public UIApplication _uiapp;

        public bool reValidate = false;

        List<ValidateResult> backUp = null;

        ExternalEvent resetViewEvent;

        ExternalEvent zoomToFitEvent;

        ExternalEvent showEleEvent;

        ShowElementsEventHandler showEventHandler;

        ExternalEvent fillInfoEvent;

        FillInfoEventHandler fillInfoEventHandler;

        List<List<Filter>> filters;

        List<bool> filterApplied;

        List<string> excludeCategories;

        List<string> NeedSpecialCare;

        List<string> TypeEntities;

        List<ValidateResultsByElement> resultsByElements;

        List<ValidateResult> SelectedRecords;

        List<ValidateResultsByElement> SelectedResultsByElements;

        string Operator = "and";

        public ResultWindow(List<string> revitDocs, List<string> fileSavePath, UIApplication uiapp, ExternalEvent ResetViewEvent, ExternalEvent ZoomToFitEvent, ExternalEvent ShowEvent, ShowElementsEventHandler ShowELeEventHandler, ExternalEvent FillInfoEvent, FillInfoEventHandler FillInfoEventHandler)
        {
            InitializeComponent();

            _revitDocs = revitDocs;

            fileSelected.ItemsSource = _revitDocs;

            _uiapp = uiapp;

            _fileSavePath = fileSavePath;

            resetViewEvent = ResetViewEvent;

            zoomToFitEvent = ZoomToFitEvent;

            showEleEvent = ShowEvent;

            showEventHandler = ShowELeEventHandler;

            fillInfoEvent = FillInfoEvent;

            fillInfoEventHandler = FillInfoEventHandler;

        }

        private void SetUpExcludeCategories()
        {
            excludeCategories = new List<string>();

            NeedSpecialCare = new List<string>();

            TypeEntities = new List<string>();

            excludeCategories.Add("IfcGrid");

            excludeCategories.Add("IfcSpaceType");

            NeedSpecialCare.Add("IfcProject");

            NeedSpecialCare.Add("IfcSite");

            NeedSpecialCare.Add("IfcBuilding");

            NeedSpecialCare.Add("IfcBuildingStorey");

            NeedSpecialCare.Add("IfcSpace");

            NeedSpecialCare.Add("IfcStairFlight");  

            NeedSpecialCare.Add("IfcRampFlight");

            NeedSpecialCare.Add("IfcPlate");

            NeedSpecialCare.Add("IfcStairFlightType");

            NeedSpecialCare.Add("IfcRampFlightType");

            NeedSpecialCare.Add("IfcPlateType");

            NeedSpecialCare.Add("IfcOpeningElement");

            NeedSpecialCare.Add("IfcDistributionPort");

           /* excludeCategories.Add("IfcProject"); // don't have the element

            excludeCategories.Add("IfcSite"); // don't have the element

            excludeCategories.Add("IfcBuilding"); // don't have the element

            excludeCategories.Add("IfcBuildingStorey"); // Have the element but can't show it

            excludeCategories.Add("IfcSpace"); // has the element but can't show it

            excludeCategories.Add("IfcGrid");

            excludeCategories.Add("IfcStairFlight"); // has the element but can't show it. show the host element instead

            excludeCategories.Add("IfcRampFlight"); // has the element but can't show it. show the host element instead

            excludeCategories.Add("IfcPlate"); // has the element but can't show it. show the host element instead

            excludeCategories.Add("IfcOpeningElement"); //couln't find the element. need to show the host elemnt */

            string pattern = @"^Ifc\w+Type$";

            foreach (var IfcType in filters[1])
            {
                if (Regex.IsMatch(IfcType.Name, pattern))
                {
                    TypeEntities.Add(IfcType.Name);
                }
            }

            TypeEntities.Remove("IfcSpaceType");

            TypeEntities.Remove("IfcPlateType");

            TypeEntities.Remove("IfcStairFlightType");

            TypeEntities.Remove("IfcRampFlightType");
        }

        private void GetSelectedRecords()
        {
            SelectedRecords = new List<ValidateResult>();

            foreach (var record in backUp)
            {
                if (record.Hide == true)
                {
                    SelectedRecords.Add(record);
                }
            }
        }

        private void fileSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int itemIndex = fileSelected.SelectedIndex;

            string csvPath = _fileSavePath[itemIndex];

            using (StreamReader reader = new StreamReader(csvPath))

            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ResultMap>();

                var records = csv.GetRecords<ValidateResult>();

                backUp = records.ToList();

                SelectedRecords = backUp;

                resultsByElements = new List<ValidateResultsByElement>();

                List<string> eleList = new List<string>();

                foreach (var record in backUp)
                {
                    if (!eleList.Contains(record.Ifc_GUID))
                    {
                        eleList.Add(record.Ifc_GUID);

                        ValidateResultsByElement ele = new ValidateResultsByElement();

                        ele.Name = record.Id;

                        ele.Results = new List<ValidateResult>();

                        ele.Results.Add(record);

                        resultsByElements.Add(ele);
                    }
                    else
                    {
                        ValidateResultsByElement ele = resultsByElements.Find(x => x.Name == record.Id);

                        ele.Results.Add(record);
                    }
                }

                SelectedResultsByElements = resultsByElements;

                DataTable.ItemsSource = resultsByElements;

            }

            if (backUp != null)
            {
                InitializeFilters();

                SetUpExcludeCategories();
            }
            else
            {
                MessageBox.Show("Empty File!");
            }


            //Now set the selected file as the active document
            if (_uiapp.Application.Documents.Size > 1)
            {

                UIDocument uidoc = _uiapp.ActiveUIDocument;

                List<Document> openDocuments = _uiapp.Application.Documents.Cast<Document>().ToList();

                foreach (Document doc in openDocuments)
                {
                    if (doc.PathName == fileSelected.SelectedItem.ToString())
                    {
                        if (doc != uidoc.Document)
                        {
                            // If is not the active UI document, open it and set it as the active document
                            _uiapp.OpenAndActivateDocument(doc.PathName);

                            break;
                        }
                    }
                }
            }

            //Now reset the view
            resetViewEvent.Raise();
            zoomToFitEvent.Raise();

        }

        private void optionButton_Click(object sender, RoutedEventArgs e)
        {
            if (backUp == null)
            {
                MessageBox.Show("Error! Please select one file!");
            }
            else
            {
                ChooseFilter newWindow = new ChooseFilter(backUp, filters, filterApplied, Operator, resultsByElements);

                newWindow.Show();

                newWindow.Closing += NewWindow_Closing;
            }
        }

        private void NewWindow_Closing(object sender, CancelEventArgs e)
        {
            ChooseFilter newWindow = sender as ChooseFilter;

            if (newWindow != null)
            {
                filters = newWindow.filters;
                filterApplied = newWindow.filterApplied;
                Operator = newWindow.Operator;
                //resultsByElements = newWindow.resultsByElement;

                GetSelectedRecords();

                GetElementSelected();


            }
            else
            {
                MessageBox.Show("Error! Empty Window!");
            }

        }

        private void GetElementSelected()
        {
            SelectedResultsByElements = new List<ValidateResultsByElement>();

            foreach (var ele in resultsByElements)
            {
                if (ele.Hide == true)
                {
                    SelectedResultsByElements.Add(ele);
                }
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
        private void InitializeFilters()
        {

            filters = new List<List<Filter>>();

            List<Filter> IfcInstanceFilters = new List<Filter>();

            IfcInstanceFilters.Add(new Filter("Select All", true));

            List<Filter> IfcTypeFilters = new List<Filter>();

            IfcTypeFilters.Add(new Filter("Select All", true)); ;

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

            foreach (var record in backUp)
            {

                //string id = record.Id.Insert(record.Id.IndexOf(@"_"),@"_");
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
            filters.Add(ResultFilters);

            filterApplied = new List<bool> { false, false, false, false, false };
        }

        private void failedButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRecords.Count > 0)
            {
                SolidColorBrush failedBrush = FailedColor.Background as SolidColorBrush;

                SolidColorBrush excludedBrush = new SolidColorBrush(Color.FromRgb(146, 146, 146));

                Color color = failedBrush.Color;

                List<string> failedElementID = new List<string>();

                foreach (var record in SelectedRecords)
                {
                    if (record.Result == "Fail")
                    {
                        record.Hide = true;

                        if (excludeCategories.Contains(record.IfcType) || NeedSpecialCare.Contains(record.IfcType) || TypeEntities.Contains(record.IfcType))
                        {
                            record.Background = failedBrush;

                            ValidateResultsByElement ele = SelectedResultsByElements.FindAll(x => x.Name == record.Id).FirstOrDefault();

                            ele.Background = excludedBrush;
                        }
                        else
                        {
                            failedElementID.Add(record.Ifc_GUID);
                        }
                    }
                    else
                    {
                        record.Hide = false;
                    }
                }

                UpdateExpander();

                resetViewEvent.Raise();

                if (failedElementID.Count > 0)
                {
                    failedElementID = failedElementID.Distinct().ToList();
                    showEventHandler.argbColor_Fail = color;
                    showEventHandler.elementsGUIDs_Fail = failedElementID;
                    showEventHandler.elementsGUIDs_Pass = new List<string>();
                    showEventHandler.elementsGUIDs_Warning = new List<string>();
                    showEleEvent.Raise();
                }
                else
                {

                    MessageBox.Show("No Elements to show!");
                    foreach (var ele in SelectedResultsByElements)
                    {
                        ele.Hide = false;
                    }
                }

                zoomToFitEvent.Raise();
            }
            else
            {
                MessageBox.Show("Please select a file!");
            }
        }

        private void UpdateExpander()
        {
            for (int i = 0; i < SelectedResultsByElements.Count; i++)
            {

                if (AllFalse(SelectedResultsByElements[i]))
                {
                    SelectedResultsByElements[i].Hide = false; //If all elements are hidden, hide the expander as well
                }
                else
                {
                    SelectedResultsByElements[i].Hide = true; //If all elements are shown, show the expander as well
                }
            }
        }

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

        private void warningButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRecords.Count > 0)
            {
                SolidColorBrush warningBrush = WarningColor.Background as SolidColorBrush;

                SolidColorBrush excludedBrush = new SolidColorBrush(Color.FromRgb(146, 146, 146));

                Color color = warningBrush.Color;

                List<string> warningElementID = new List<string>();

                foreach (var record in SelectedRecords)
                {
                    if (record.Result == "Warning")
                    {
                        record.Hide = true;

                        if (excludeCategories.Contains(record.IfcType) || NeedSpecialCare.Contains(record.IfcType) || TypeEntities.Contains(record.IfcType))
                        {
                            record.Background = warningBrush;

                            ValidateResultsByElement ele = SelectedResultsByElements.FindAll(x => x.Name == record.Id).FirstOrDefault();

                            ele.Background = excludedBrush;
                        }
                        else
                        {
                            warningElementID.Add(record.Ifc_GUID);

                        }
                    }
                    else
                    {
                        record.Hide = false;
                    }
                }
                UpdateExpander();


                resetViewEvent.Raise();

                if (warningElementID.Count > 0)
                {
                    warningElementID = warningElementID.Distinct().ToList();
                    showEventHandler.argbColor_Warning = color;
                    showEventHandler.elementsGUIDs_Fail = new List<string>();
                    showEventHandler.elementsGUIDs_Pass = new List<string>();
                    showEventHandler.elementsGUIDs_Warning = warningElementID;

                    showEleEvent.Raise();
                }
                else
                {
                    MessageBox.Show("No Elements to show!");

                    foreach (var ele in SelectedResultsByElements)
                    {
                        ele.Hide = false;
                    }
                }

                zoomToFitEvent.Raise();
            }
            else
            {
                MessageBox.Show("Please select a file!");
            }
        }

        private void passButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRecords.Count > 0)
            {
                SolidColorBrush passBrush = PassColor.Background as SolidColorBrush;

                Color passColor = passBrush.Color;

                SolidColorBrush warningBrush = WarningColor.Background as SolidColorBrush;

                SolidColorBrush notApplyBrush = NotApplyColor.Background as SolidColorBrush;

                SolidColorBrush excludedBrush = new SolidColorBrush(Color.FromRgb(146, 146, 146));

                List<string> passElementID = new List<string>();

                foreach (var ele in SelectedResultsByElements)
                {
                    passElementID.Add(ele.Results[0].Ifc_GUID);
                }
       
                foreach (var record in SelectedRecords)
                {
                    if (record.Result != "Fail")
                    {
                        record.Hide = true;

                        switch (record.Result)
                        {
                            case "Pass":
                                record.Background = passBrush;
                                break;
                            case "Warning":
                                record.Background = warningBrush;
                                break;
                            case "DoesNotApply":
                                record.Background = notApplyBrush;
                                break;
                        }

                    }
                    else
                    {
                        record.Hide = false;

                        passElementID.Remove(record.Ifc_GUID);
                    }


                    UpdateExpander();

                    foreach (var ele in SelectedResultsByElements)
                    {
                        if (excludeCategories.Contains(ele.Results[0].IfcType) || NeedSpecialCare.Contains(ele.Results[0].IfcType)|| TypeEntities.Contains(ele.Results[0].IfcType))
                        {
                            ele.Background = excludedBrush;
                        }
                    }
                }

                resetViewEvent.Raise();

                if (passElementID.Count > 0)
                {
                    passElementID = passElementID.Distinct().ToList();
                    showEventHandler.argbColor_Pass = passColor;
                    showEventHandler.elementsGUIDs_Pass = passElementID;
                    showEventHandler.elementsGUIDs_Fail = new List<string>();
                    showEventHandler.elementsGUIDs_Warning = new List<string>();
                    showEleEvent.Raise();
                }
                else
                {
                    MessageBox.Show("No Elements to show!");

                    foreach (var ele in SelectedResultsByElements)
                    {
                        ele.Hide = false;
                    }
                }

                zoomToFitEvent.Raise();
            }
            else
            {
                MessageBox.Show("Please select a file!");
            }
        }

        private void showElementsButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRecords.Count > 0)
            {
                SolidColorBrush passBrush = PassColor.Background as SolidColorBrush;

                Color passColor = passBrush.Color;

                SolidColorBrush failBrush = FailedColor.Background as SolidColorBrush;

                Color failColor = failBrush.Color;

                SolidColorBrush warningBrush = WarningColor.Background as SolidColorBrush;

                SolidColorBrush notApplyBrush = NotApplyColor.Background as SolidColorBrush;

                SolidColorBrush excludedBrush = new SolidColorBrush(Color.FromRgb(146, 146, 146));

                List<string> failedElementID = new List<string>();

                List<string> passElementID = new List<string>();

                foreach (var ele in SelectedResultsByElements)
                {
                    passElementID.Add(ele.Results[0].Ifc_GUID);
                }

                foreach (var record in SelectedRecords)
                {
                    record.Hide = true;

                    switch (record.Result)
                    {
                        case "Pass":
                            record.Background = passBrush;
                            break;
                        case "Warning":
                            record.Background = warningBrush;
                            break;
                        case "Fail":
                            record.Background = failBrush;
                            failedElementID.Add(record.Ifc_GUID);
                            break;
                        case "DoesNotApply":
                            record.Background = notApplyBrush;
                            break;
                    }
                }

                failedElementID = failedElementID.Distinct().ToList();

                passElementID = passElementID.Except(failedElementID).ToList();

                foreach (var ele in SelectedResultsByElements)
                {
                    ele.Hide = true;

                    if (excludeCategories.Contains(ele.Results[0].IfcType) || NeedSpecialCare.Contains(ele.Results[0].IfcType) || TypeEntities.Contains(ele.Results[0].IfcType))
                    {
                        ele.Background = excludedBrush;
                    }
                }

                //UpdateExpander();

                resetViewEvent.Raise();

                if (failedElementID.Count > 0 || passElementID.Count > 0)
                {
                    showEventHandler.argbColor_Pass = passColor;
                    showEventHandler.argbColor_Fail = failColor;
                    showEventHandler.elementsGUIDs_Pass = passElementID;
                    showEventHandler.elementsGUIDs_Fail = failedElementID;
                    showEventHandler.elementsGUIDs_Warning = new List<string>();

                    showEleEvent.Raise();
                }
                else
                {

                    MessageBox.Show("No Elements to show!");

                    foreach (var ele in SelectedResultsByElements)
                    {
                        ele.Hide = false;
                    }
                }

                zoomToFitEvent.Raise();
            }
            else
            {
                MessageBox.Show("Please select a file!");
            }
        }

        private void ColorSelect_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            ColorDialog colorDialog = new ColorDialog();

            DialogResult result = colorDialog.ShowDialog();

            // Show the color dialog
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Set the color of the Rectangle
                System.Drawing.Color selectedColor = colorDialog.Color;

                button.Background = new SolidColorBrush(Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B));
            }
        }

        private void resetLegendButton_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush failedBrush = new SolidColorBrush(Color.FromRgb(241, 145, 145));
            FailedColor.Background = failedBrush;

            SolidColorBrush warningBrush = new SolidColorBrush(Color.FromRgb(247, 204, 127));
            WarningColor.Background = warningBrush;

            SolidColorBrush passBrush = new SolidColorBrush(Color.FromRgb(142, 253, 159));
            PassColor.Background = passBrush;

            SolidColorBrush NotApplyBrush = new SolidColorBrush(Color.FromRgb(119, 171, 255));
            NotApplyColor.Background = NotApplyBrush;

            SolidColorBrush SelectBrush = new SolidColorBrush(Colors.MediumPurple);
            SelectColor.Background = SelectBrush;
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            if (backUp != null)
            {

                foreach (var ele in SelectedResultsByElements)
                {
                    ele.Hide = true;

                    ele.Background = null;
                }

                foreach (var record in SelectedRecords)
                {
                    record.Hide = true;

                    record.Background = null;
                }

                resetViewEvent.Raise();

                zoomToFitEvent.Raise();

            }
            else
            {
                MessageBox.Show("Please select a file!");
            }
        }

        private void EleMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string Id = null;

            string IfcGUID = null;

            string IfcType = null;  

            Expander expander = sender as Expander;

            if (expander != null)
            {
                TextBlock textBlock = expander.Header as TextBlock;

                if (textBlock != null)
                {
                    Id = textBlock.Text;

                    foreach (var ele in SelectedResultsByElements)
                    {
                        if (ele.Name == Id)
                        {
                            IfcType= ele.Results[0].IfcType;
                            IfcGUID = ele.Results[0].Ifc_GUID;
                            break;
                        }
                    }
                }
            }
            else
            {
                ListViewItem listItem = sender as ListViewItem;

                if (listItem != null)
                {
                    ValidateResult record = listItem.Content as ValidateResult;

                    if (record != null)
                    {
                        IfcType = record.IfcType;
                        IfcGUID = record.Ifc_GUID;
                        Id = record.Id;
                    }
                }
            
            }

            if(IfcGUID != null || !excludeCategories.Contains(IfcType))
            {
                resetViewEvent.Raise();

                ValidateResultsByElement eleResult = resultsByElements.Find(x => x.Name == Id);

                SolidColorBrush brush = SelectColor.Background as SolidColorBrush;

                showEventHandler.argbColor_Pass = brush.Color;

                showEventHandler.elementsGUIDs_Pass = new List<string>();

                showEventHandler.elementsGUIDs_Fail = new List<string>();

                showEventHandler.elementsGUIDs_Warning = new List<string>();

                fillInfoEventHandler.eleResult = eleResult;

                if (TypeEntities.Contains(IfcType))
                {
                    //The Ifc entity is a type entity, and it can be corrrespond to one or more Ifc elements
                    //Then highlight the corresponding elements an show a  table to fill the Type info

                    List<string> findTypedElementsGUIDs = GetElementGUIDsByType(IfcGUID);

                    showEventHandler.elementsGUIDs_Pass.AddRange(findTypedElementsGUIDs);

                    //showEventHandler.mode = false;

                    fillInfoEventHandler.mode = true;

                }          
                else if (NeedSpecialCare.Contains(IfcType))
                {
                    switch (IfcType)
                    {
                        case "IfcProject":
                        case "IfcSite":
                        case "IfcBuilding":
                            {
                                fillInfoEventHandler.mode = false;

                                fillInfoEvent.Raise();

                                return;
                            }
                        case "IfcBuidlingStorey":
                        case "IfcSpace":
                            {
                                fillInfoEventHandler.mode = false;

                                fillInfoEvent.Raise();

                                return;
                            }
                        case "IfcStairFlight":
                        case "IfcRampFlight":
                        case "IfcPlate":
                            {
                                string HostElementGUID = GetHostElementGUID(IfcGUID);

                                showEventHandler.elementsGUIDs_Pass.Add(HostElementGUID);

                                //showEventHandler.mode = false;

                                fillInfoEventHandler.mode = false;
                            }
                            break;
                        case "IfcOpeningElement":
                            {
                                string HostElementGUID = GetFilledElementGUIDForOpeningEle(IfcGUID);

                                showEventHandler.elementsGUIDs_Pass.Add(HostElementGUID);

                                //showEventHandler.mode = false;

                                fillInfoEventHandler.mode = false;
                            }
                            break;
                        case "IfcDistributionPort":
                            {
                                string HostElementGUID = GetHostElementGUIDForDistributionPort(IfcGUID);

                                showEventHandler.elementsGUIDs_Pass.Add(HostElementGUID);

                                //showEventHandler.mode = false;

                                fillInfoEventHandler.mode = false;
                            }
                            break;
                        case "IfcStairFlightType":
                        case "IfcRampFlightType":
                        case "IfcPlateType":
                            {
                                List<string> HostELementGUIDs = GetHostElementGUIDsByType(IfcGUID);

                                showEventHandler.elementsGUIDs_Pass.AddRange(HostELementGUIDs);

                                //showEventHandler.mode = false;

                                fillInfoEventHandler.mode = false;

                            }
                            break;
                    }
                }
                else
                {
                    //The Ifc entity is an instance entity, and it can only be corrrespond to one Ifc element
                    //Then hightlight the element and show a table to fill the instance info

                    IfcGUID=GetStairGUIDIfLandingSlab(IfcGUID);

                    showEventHandler.elementsGUIDs_Pass.Add(IfcGUID);

                    //showEventHandler.mode= false;

                    fillInfoEventHandler.mode = false;
                }

                showEleEvent.Raise();

                zoomToFitEvent.Raise();

                fillInfoEvent.Raise();
            }
            else
            {
                    //The type that are not be able to show or find in revit, information can't be filled directly
                    MessageBox.Show("Can't find the element in Revit! Need to fill info directly through other windows!");

            }
        }

        private string GetStairGUIDIfLandingSlab(string ifcGUID)
        {
            string theHostElementGUID = null;

            int itemIndex = fileSelected.SelectedIndex;

            string fileSavePath = Path.GetDirectoryName(_fileSavePath[itemIndex]) + "\\" + Path.GetFileNameWithoutExtension(_revitDocs[itemIndex]) + ".ifc";

            if (File.Exists(fileSavePath))
            {
                using (var model = IfcStore.Open(fileSavePath))
                {
                    var theIfcEle = model.Instances.Where<IIfcObject>(x => x.GlobalId == ifcGUID).FirstOrDefault();

                    if(theIfcEle.ExpressType.Name=="IfcSlab" && theIfcEle.Decomposes.Count()>0)
                    {
                        var hostEle = theIfcEle.Decomposes.FirstOrDefault().RelatingObject;

                        theHostElementGUID = hostEle.GlobalId;
                    }
                    else
                    {
                        theHostElementGUID = ifcGUID;
                    }

                }
            }
            else
            {
                MessageBox.Show("Can't find the Exported IFC file!");
            }
            return theHostElementGUID;
        }

        private string GetHostElementGUIDForDistributionPort(string ifcGUID)
        {

            string theHostElementGUID = null;

            int itemIndex = fileSelected.SelectedIndex;

            string fileSavePath = Path.GetDirectoryName(_fileSavePath[itemIndex]) + "\\" + Path.GetFileNameWithoutExtension(_revitDocs[itemIndex]) + ".ifc";

            if (File.Exists(fileSavePath))
            {
                using (var model = IfcStore.Open(fileSavePath))
                {
                    var theIfcEle = model.Instances.Where<IIfcDistributionPort>(x => x.GlobalId == ifcGUID).FirstOrDefault();

                    var hostEle=theIfcEle.Nests.FirstOrDefault().RelatingObject;

                    theHostElementGUID = hostEle.GlobalId;
                }
            }
            else
            {
                MessageBox.Show("Can't find the Exported IFC file!");
            }
            return theHostElementGUID;
        }

        private List<string> GetElementGUIDsByType(string ifcGUID)
        {
            List<string> ElementGUIDs = new List<string>();

            int itemIndex = fileSelected.SelectedIndex;

            string fileSavePath = Path.GetDirectoryName(_fileSavePath[itemIndex]) + "\\" + Path.GetFileNameWithoutExtension(_revitDocs[itemIndex]) + ".ifc";

            if (File.Exists(fileSavePath))
            {
                using (var model = IfcStore.Open(fileSavePath))
                {

                    var theIfcEle = model.Instances.Where<IIfcTypeObject>(x => x.GlobalId == ifcGUID).FirstOrDefault();

                    if (theIfcEle != null)
                    {
                        var ifcRelTypes = theIfcEle.Types.FirstOrDefault();

                        var instances = ifcRelTypes.RelatedObjects.ToList();

                        foreach (var item in instances)
                        {
                            if(item.ExpressType.Name=="IfcSlab" && item.Decomposes.Count()>0)
                            {
                                var hostEle = item.Decomposes.FirstOrDefault().RelatingObject;

                                ElementGUIDs.Add(hostEle.GlobalId);
                            }

                            ElementGUIDs.Add(item.GlobalId);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Can't find the Exported IFC file!");
            }

            ElementGUIDs = ElementGUIDs.Distinct().ToList();

            return ElementGUIDs;
        }

        private string GetFilledElementGUIDForOpeningEle(string ifcGUID)
        {
            string theFilledElementGUID = null;

            int itemIndex = fileSelected.SelectedIndex;

            string fileSavePath = Path.GetDirectoryName(_fileSavePath[itemIndex]) + "\\" + Path.GetFileNameWithoutExtension(_revitDocs[itemIndex]) + ".ifc";

            if (File.Exists(fileSavePath))
            {
                using (var model = IfcStore.Open(fileSavePath))
                {
                    var theIfcEle = model.Instances.Where<IIfcOpeningElement>(x => x.GlobalId == ifcGUID).FirstOrDefault();

                    if (theIfcEle != null)
                    {
                        var FilledElement = theIfcEle.HasFillings;

                        if(FilledElement.Count()>0)
                        {
                            var ele= FilledElement.FirstOrDefault().RelatedBuildingElement;

                            theFilledElementGUID = ele.GlobalId;
                        }
                        else
                        {
                            var openingEleTag=theIfcEle.Tag;

                            var hostEle = model.Instances.Where<IIfcBuildingElement>(x => x.Tag== openingEleTag).FirstOrDefault();

                            theFilledElementGUID = hostEle.GlobalId;
                        }
                        
                    }
                }
            }
            else
            {
                MessageBox.Show("Can't find the Exported IFC file!");
            }
            return theFilledElementGUID;
        }

        private List<string> GetHostElementGUIDsByType(string ifcGUID)
        {
            List<string> HostElementGUIDs = new List<string>();

            int itemIndex = fileSelected.SelectedIndex;

            string fileSavePath =Path.GetDirectoryName(_fileSavePath[itemIndex])+"\\"+ Path.GetFileNameWithoutExtension(_revitDocs[itemIndex])+".ifc";

            if (File.Exists(fileSavePath))
                {
                    using (var model = IfcStore.Open(fileSavePath))
                    {

                    var theIfcEle = model.Instances.Where<IIfcTypeObject>(x => x.GlobalId == ifcGUID).FirstOrDefault();

                        if(theIfcEle!=null)
                        {
                            var ifcRelTypes = theIfcEle.Types.FirstOrDefault();

                            var instances = ifcRelTypes.RelatedObjects.ToList();

                            foreach(var item in instances)
                            {
                                var HostElement = item.Decomposes.FirstOrDefault().RelatingObject;

                                HostElementGUIDs.Add(HostElement.GlobalId);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Can't find the Exported IFC file!");
                }  

            return HostElementGUIDs;
        }

        private string GetHostElementGUID(string ifcGUID)
        {
            string theHostElementGUID = null;

            int itemIndex = fileSelected.SelectedIndex;

            string fileSavePath = Path.GetDirectoryName(_fileSavePath[itemIndex]) + "\\" + Path.GetFileNameWithoutExtension(_revitDocs[itemIndex]) + ".ifc";

            if (File.Exists(fileSavePath))
                {
                    using (var model = IfcStore.Open(fileSavePath))
                    {
                        var theIfcEle = model.Instances.Where<IIfcObject>(x => x.GlobalId == ifcGUID).FirstOrDefault();

                        if (theIfcEle != null)
                        {
                            var HostElement = theIfcEle.Decomposes.FirstOrDefault().RelatingObject;

                            theHostElementGUID = HostElement.GlobalId;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Can't find the Exported IFC file!");
                }
            return theHostElementGUID;
        }

        private void reValidateButton_Click(object sender, RoutedEventArgs e)
        {
            reValidate = true;

            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
