using Autodesk.Revit.DB;
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
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for ChooseER.xaml
    /// </summary>
    public partial class ChooseER : ChildWindow
    {
        List<string> ER_Name=new List<string>();
        public string SelectedER_Name { get; set; }

        public IFCVersion appliedIFCSchema { get; set; }

        public string realMVDName { get;set; }
        public ChooseER(string filePath, bool exportAsOfficialMVDs)
        {
            InitializeComponent();

            if (exportAsOfficialMVDs)
            {
                ER_Name.Add("Architecture");
                ER_Name.Add("Structural");
                ER_Name.Add("BuildingService");     
            }
            else
            {
               
                XmlDocument doc = new XmlDocument();

                doc.Load(filePath);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("mvd", "http://buildingsmart-tech.org/mvd/XML/1.1");
               // nsmgr.AddNamespace("bk", "random");
               // nsmgr.AddNamespace("bk", "random");

                XmlNode root = doc.DocumentElement;


                XmlNodeList nodes = root.SelectNodes("mvd:Views/mvd:ModelView[1]/mvd:ExchangeRequirements/mvd:ExchangeRequirement", nsmgr);

                //XmlNodeList nodes = viewNodes.SelectNodes("Views/ModelView[1]/ExchangeRequirements/ExchangeRequirement");

                foreach(XmlNode node in nodes)
                {
                    ER_Name.Add(node.Attributes["name"].Value);
                }
                string mvdIFCVersion = null;

                XmlNode mvdNode = root.SelectSingleNode("mvd:Views/mvd:ModelView[1]", nsmgr);

                mvdIFCVersion = mvdNode.Attributes["applicableSchema"].Value;

                realMVDName = mvdNode.Attributes["name"].Value;

                switch (mvdIFCVersion)
                {
                    case "IFC2X2":
                        appliedIFCSchema = IFCVersion.IFC2x2;
                        break;
                    case "IFC2X3":
                        appliedIFCSchema = IFCVersion.IFC2x3;
                        break;
                    case "IFC4":
                        appliedIFCSchema = IFCVersion.IFC4;
                        break;
                    default:
                        appliedIFCSchema = IFCVersion.Default;
                        break;
                }
            }

            ER_Names.ItemsSource = ER_Name;

        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {

            SelectedER_Name = ER_Names.SelectedItem.ToString();
            
            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
