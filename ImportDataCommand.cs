using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;
using Xbim.IO.Step21;
using static Autodesk.Revit.DB.SpecTypeId;

namespace Cust_IFC_Exporter
{
    [Transaction(TransactionMode.Manual)]
    class ImportDataCommand : IExternalCommand
    {
        SQLDBConnect connect;

        List<RowItem> rows=new List<RowItem>();

        int count = 0;

        static readonly char[] prohibitedCharacters = { '{', '}', '[', ']', '|', ';', '<', '>', '?', '`', '~', ':','-'};

        static string ReplaceProhibitedCharacters(string name)
        {
            foreach (char prohibitedChar in prohibitedCharacters)
            {
                name = name.Replace(prohibitedChar, ' ');
            }
            return name;
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;

            Document doc = uiapp.ActiveUIDocument.Document;

            DatabaseLogin databaseLogin = new DatabaseLogin();

            bool? loginDialogResult = databaseLogin.ShowDialog();

            if (loginDialogResult.HasValue && loginDialogResult.Value)
            {
                connect = databaseLogin.connect;

                AddKeySchedule addKeySchedule = new AddKeySchedule(connect,rows);

                bool? addScheduleDialogResult = addKeySchedule.ShowDialog();

                if(addScheduleDialogResult.HasValue && addScheduleDialogResult.Value)
                {
                    //organise the rows into understandable sql command

                    Dictionary<string, List<string>> data=new Dictionary<string, List<string>>();

                    bool generateAverageData = addKeySchedule.generateAverageData;

                    foreach (RowItem row in rows)
                    {
                        connect.LoadData(data,row, generateAverageData);

                    }

                    if (data.Count > 0 && data.FirstOrDefault().Value.Count>0)
                    {

                        foreach(var pair in data)
                        {
                            if (pair.Key == "ReadyMixConcrete")
                            {
                                GenerateConcreteMaterials(doc,pair.Value);
                            }
                            else if (pair.Key == "Timber")
                            {
                                GenerateTimberMaterials(doc,pair.Value);
                            }
                            else
                            {
                                GenerateGeneralMaterials(doc,pair.Key,pair.Value);
                            }
                        }

                        if(count>1)
                        {
                            MessageBox.Show($"{count} new materials have beeen added!");
                        }
                        else if(count==1)
                        {
                            MessageBox.Show($"{count} new material has beeen added!");
                        }
                        else
                        {
                            MessageBox.Show($"Failed! {count} new materials is added!");
                        }                      

                    }
                    else
                    {
                        MessageBox.Show("Error! No data record satisfies the filters requirements!");                  
                    }
                }

            }

            return Result.Succeeded;
        }

        private void GenerateConcreteMaterials(Document doc, List<string> value)
        {
            // Create a new material
            using (Transaction transaction = new Transaction(doc, "Add New Material"))
            {
                transaction.Start();

                foreach (var record in value)
                {
                    count++;

                    string[] recordValues = record.Split('\t');

                    //Create the material

                    ElementId materialId = Material.Create(doc, count.ToString()+"_"+ ReplaceProhibitedCharacters(recordValues[2]));

                    Material material = doc.GetElement(materialId) as Material;

                    material.MaterialCategory = "ReadyMixConcrete";

                    material.MaterialClass = "ReadyMixConcrete";

                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    manufacturer.Set(ReplaceProhibitedCharacters(recordValues[4]));

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    if (recordValues[2].StartsWith("No."))
                    {
                        var comment = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        comment.Set(recordValues[5]);

                        model.Set("No Specific Model");

                    } 
                    else
                    {
                        model.Set(ReplaceProhibitedCharacters(recordValues[5]));
                    }

                   
                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    mark.Set(recordValues[0]);

                    //Create a new property set that can be used by this material
                    StructuralAsset strucAsset = new StructuralAsset($"{count}_Property_" + recordValues[0], StructuralAssetClass.Concrete); ;
                    strucAsset.Behavior = StructuralBehavior.Isotropic;
                    strucAsset.Density = recordValues[7].ToDouble() / 35.315;

                    if (double.TryParse(recordValues[6], out double result))
                    {
                        strucAsset.ConcreteCompression = result * 304800;
                    }
                    

                    //Assign the property set to the material.
                    PropertySetElement pse = PropertySetElement.Create(doc, strucAsset);

                    material.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pse.Id);

                }

                transaction.Commit();
            }

        }



        private void GenerateTimberMaterials(Document doc, List<string> value)
        {
            // Create a new material
            using (Transaction transaction = new Transaction(doc, "Add New Material"))
            {
                transaction.Start();

                foreach (var record in value)
                {
                    count++;
                    string[] recordValues = record.Split('\t');

                    //Create the material
                    ElementId materialId = Material.Create(doc, count.ToString() + "_" + ReplaceProhibitedCharacters(recordValues[2]));
                    Material material = doc.GetElement(materialId) as Material;

                    material.MaterialCategory = "Timber";

                    material.MaterialClass = "Timber";

                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    manufacturer.Set(ReplaceProhibitedCharacters(recordValues[4]));

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    if (recordValues[2].StartsWith("No."))
                    {
                        var comment = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        comment.Set(recordValues[5]);

                        model.Set("No Specific Model");

                    }
                    else
                    {
                        model.Set(ReplaceProhibitedCharacters(recordValues[5]));
                    }

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    mark.Set(recordValues[0]);

                    //Create a new property set that can be used by this material
                    StructuralAsset strucAsset = new StructuralAsset($"{count}_Property_" + recordValues[0], StructuralAssetClass.Wood);
                    strucAsset.Behavior = StructuralBehavior.Isotropic;
                    strucAsset.Density = recordValues[7].ToDouble()/35.315;
                    
                    //strucAsset.MinimumTensileStrength = recordValues[6].ToDouble()*304800;

                    //strucAsset.MinimumYieldStress = recordValues[6].ToDouble()*304800;

                    //Assign the property set to the material.
                    PropertySetElement pse = PropertySetElement.Create(doc, strucAsset);
                    material.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pse.Id);
                }

                transaction.Commit();
            }
        }

        private void GenerateGeneralMaterials(Document doc, string key, List<string> value)
        {
            // Create a new material
            using (Transaction transaction = new Transaction(doc, "Add New Material"))
            {
                transaction.Start();

                foreach (var record in value)
                {
                    count++;
                    string[] recordValues = record.Split('\t');

                    //Create the material
                    ElementId materialId = Material.Create(doc, count.ToString() + "_" + ReplaceProhibitedCharacters(recordValues[2]));
                    Material material = doc.GetElement(materialId) as Material;

                    material.MaterialCategory = key;

                    material.MaterialClass = key;

                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    manufacturer.Set(ReplaceProhibitedCharacters(recordValues[4]));

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    if (recordValues[2].StartsWith("No."))
                    {
                        var comment = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        comment.Set(recordValues[5]);

                        model.Set("No Specific Model");

                    }
                    else
                    {
                        model.Set(ReplaceProhibitedCharacters(recordValues[5]));
                    }

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    mark.Set(recordValues[0]);

                    //Create a new property set that can be used by this material
                    StructuralAsset strucAsset = new StructuralAsset($"{count}_Property_" + recordValues[0], StructuralAssetClass.Generic);
                    
                    //strucAsset.Behavior = StructuralBehavior.Isotropic;

                    //Assign the property set to the material.
                    PropertySetElement pse = PropertySetElement.Create(doc, strucAsset);

                    material.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pse.Id);
                }

                transaction.Commit();
            }
        }
        private void testFunc()
        {
            TreeNode<string> root = new TreeNode<string>("Nations");

            TreeNode<string> china = new TreeNode<string>("China");

            TreeNode<string> usa = new TreeNode<string>("USA");

            TreeNode<string> canada = new TreeNode<string>("Canada");

            TreeNode<string> california = new TreeNode<string>("California");

            TreeNode<string> texas = new TreeNode<string>("Texas");

            TreeNode<string> newyork = new TreeNode<string>("New York");

            usa.Children.Add(california);

            usa.Children.Add(texas);

            usa.Children.Add(newyork);

            california.ParentNode = usa;

            texas.ParentNode = usa;

            newyork.ParentNode = usa;

            china.ParentNode = root;

            usa.ParentNode = root;

            canada.ParentNode = root;

            root.Children.Add(china);

            root.Children.Add(usa);

            root.Children.Add(canada);

            TreeNode<string> beijing = new TreeNode<string>("Beijing");

            TreeNode<string> shanghai = new TreeNode<string>("Shanghai");

            TreeNode<string> guangdong = new TreeNode<string>("Guang Dong");

            TreeNode<string> henan = new TreeNode<string>("He Nan");

            china.Children.Add(beijing);

            china.Children.Add(shanghai);

            china.Children.Add(guangdong);

            china.Children.Add(henan);

            henan.ParentNode = china;

            beijing.ParentNode = china;

            guangdong.ParentNode = china;

            shanghai.ParentNode = china;

            TreeNode<string> anyang = new TreeNode<string>("An Yang");

            TreeNode<string> nanyang = new TreeNode<string>("Nan Yang");

            TreeNode<string> xinxiang = new TreeNode<string>("Xin Xiang");

            TreeNode<string> zhengzhou = new TreeNode<string>("Zheng Zhou");

            henan.Children.Add(anyang);

            henan.Children.Add(nanyang);

            henan.Children.Add(xinxiang);

            henan.Children.Add(zhengzhou);

            anyang.ParentNode = henan;

            nanyang.ParentNode = henan;

            xinxiang.ParentNode = henan;

            zhengzhou.ParentNode = henan;

            ObservableCollection<TreeNode<string>> nodes = new ObservableCollection<TreeNode<string>>();

            nodes.Add(root);

            SelectTreeWindow selectTreeWindow = new SelectTreeWindow(nodes);

            selectTreeWindow.ShowDialog();
        }

    }
}
