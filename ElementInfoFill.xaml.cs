using Autodesk.Revit.DB;
using Autodesk.UI.Windows;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static Autodesk.Revit.DB.SpecTypeId;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for ElementInfoFill.xaml
    /// </summary>
    public partial class ElementInfoFill : ChildWindow
    {
        /*string classFilePath = @"C:\ProgramData\Autodesk\RVT 2023\exportlayers-ifc-IAI.txt";

        string parameterFilePath = @"C:\ProgramData\Autodesk\RVT 2023\exportlayers-ifc-IAI.txt";*/

        string mappingTablePath = null;

        public List<ParameterRow> FailedPara;

        public List<ParameterRow> WarningPara;

        Document doc;

        ElementId eleId;
        public ElementInfoFill(Document Doc,  ValidateResultsByElement result, ElementId elementId, string eleType)
        {
            InitializeComponent();

            doc = Doc;

            eleId = elementId;

            string resourceFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

            mappingTablePath = Path.Combine(resourceFolderPath, "IFC Mapping Table_Parameters.txt");

            string info = $"Element Type: {eleType}\nElement ID: {elementId.ToString()}";

            this.Title = @"Data Detail";

            elementID.Content = info;

            FailedPara = PopulateData(result, "Fail");

            dataGrid_Fail.ItemsSource= FailedPara;

            WarningPara = PopulateData(result, "Warning");

            dataGrid_Warning.ItemsSource = WarningPara;

        }

        private List<ParameterRow> PopulateData(ValidateResultsByElement result, string Mode)
        {
            List<ParameterRow> data = new List<ParameterRow>();

            foreach (var record in result.Results)
            {
                if (record.Result == Mode && (record.ConceptName.Contains("Properties") || record.ConceptName.Contains("Quantities")))
                {
                    List<ParameterRow> rows = OrganizeData(record);

                    if (rows.Count > 0)
                    {
                        foreach (var line in rows)
                        {
                            data.Add(line);
                        }
                    }
                }
            }

            return data;
        }

        private List<ParameterRow> OrganizeData(ValidateResult record)
        {
            List<ParameterRow> rows = new List<ParameterRow>();

            List<string> substrings;
            if (record.Parameters.Contains(@"(and)"))
            {
                string[] separator = { "(and)" };
                substrings = record.Parameters.Split(separator, StringSplitOptions.None).ToList();
            }
            else if (record.Parameters.Contains(@"(or)"))
            {
                string[] separator = { "(or)" };
                substrings = record.Parameters.Split(separator, StringSplitOptions.None).ToList();
            }
            else if (record.Parameters.Contains(@"(nor)"))
            {
                string[] separator = { "(nor)" };
                substrings = record.Parameters.Split(separator, StringSplitOptions.None).ToList();
            }
            else
            {
                substrings = new List<string>();
                substrings.Add(record.Parameters);
            }

            string pattern = @"'([^']+)'";

            // Create a regular expression object and match the pattern in the input string
            Regex regex = new Regex(pattern);

            Element ele = doc.GetElement(eleId);

            foreach (var substring in substrings)
            {

                MatchCollection matches = regex.Matches(substring);

                ParameterRow row = new ParameterRow();

                row.PsetName = matches[0].Groups[1].Value;

                row.PName = matches[1].Groups[1].Value;

                string PFullName = row.PsetName + "." + row.PName;

                Parameter para = ele.LookupParameter(PFullName);

                if(para!=null)
                {
                    row.Value = para.AsValueString();
                }
                else
                {
                    row.Value = "";
                }

                row.RevitParameterName = FindRevitParameter(row.PsetName, row.PName);

                if (row.RevitParameterName == @"N/A")
                {
                    row.IsCustomizedParameter = "Yes";

                    row.RevitParameterName = PFullName;
                }
                else
                {
                    row.IsCustomizedParameter = "No";
                }

                rows.Add(row);
            }

            return rows;
        }

        private string FindRevitParameter(string psetName, string pName)
        { 
                string[] allLines = File.ReadAllLines(mappingTablePath);

                foreach (string line in allLines)
                {
                    string[] values = line.Split('\t');

                    if (values[0] == psetName && values[1].Replace("[Type]","") == pName)
                    {
                        return values[2];
                    }
                }

                return @"N/A";
           
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult= true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
    public class ParameterRow
    {
        public string PsetName { get; set; }

        public string PName { get; set; }

        public string Value { get; set; }

        public string RevitParameterName { get; set; }

        public string IsCustomizedParameter { get; set; }
    }

}

