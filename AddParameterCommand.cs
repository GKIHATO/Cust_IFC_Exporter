using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace Cust_IFC_Exporter
{
    [Transaction(TransactionMode.Manual)]
    class AddParameterCommand: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        { 

            UIApplication uiapp = commandData.Application;

            Document doc = uiapp.ActiveUIDocument.Document;

            string resourceFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");  

            string classMappingTablePath = Path.Combine(resourceFolderPath, "IFC Export Class Mapping Table.txt");

            string IfcSharedParameterFilePath = Path.Combine(resourceFolderPath, "IFC Shared Parameters.txt");

            string availaleCategoryPath = Path.Combine(resourceFolderPath, "All Categories_Real.txt");

            string[] addParameterFilePath = GenerateTXTFile(resourceFolderPath);

            var application = uiapp.Application;

            application.SharedParametersFilename = IfcSharedParameterFilePath;

            DefinitionFile definitionFile = application.OpenSharedParameterFile();

            DefinitionGroups definitionGroups = definitionFile.Groups;

            BuiltInParameterGroup ifcParameterGroup = BuiltInParameterGroup.PG_IFC;

            Categories categories = doc.Settings.Categories;

            //generate a path of desktop

/*                        using (StreamWriter writer = new StreamWriter(test_Path))
                        {
                            foreach (Category cate in categories)
                            {
                                string line = cate.Name + "\t" + cate.BuiltInCategory;

                                writer.WriteLine(line);
                            }
                        }*//*

            DefinitionGroup IfcPsetGroup = definitionGroups.get_Item("Revit IFCExporter Parameters");

            Definition pDef = IfcPsetGroup.Definitions.get_Item("IfcName");

            if(doc.ParameterBindings.Contains(pDef))
            {
                InstanceBinding binding = doc.ParameterBindings.get_Item(pDef) as InstanceBinding;

                using (StreamWriter writer = new StreamWriter(test_Path))
                {
                    foreach (Category cate in binding.Categories)
                    {
                        string line = cate.Name + "\t" + cate.BuiltInCategory;

                        writer.WriteLine(line);
                    }
                }                
            }*/


            if (File.Exists(addParameterFilePath[0]))
            {
                string[] allLines = File.ReadAllLines(addParameterFilePath[0]);

                List<string> allIfcNames = new List<string>();

                Dictionary<string, List<string>> pNames = new Dictionary<string, List<string>>();

                foreach (var line in allLines)
                {
                    //separate string by tab

                    string[] values = line.Split('\t');

                    List<string> ifcNames = new List<string>();

                    for (int i = 1; i < values.Length; i++)
                    {
                        ifcNames.Add(values[i]);

                        if (!allIfcNames.Contains(values[i]))
                        {
                            allIfcNames.Add(values[i]);
                        }
                    }

                    pNames.Add(values[0], ifcNames);
                }

                Dictionary<string, CategorySet> categorySets = GetAllCategory(categories, classMappingTablePath, availaleCategoryPath, allIfcNames);

                Transaction transaction = new Transaction(doc, "Add Parameter_Instance");

                transaction.Start();

                foreach (var name in pNames.Keys)
                {
                    string[] pName = name.Split('.');

                    string PName = pName[1];

                    string PsetName = pName[0];

                    DefinitionGroup IfcPsetGroup = definitionGroups.get_Item(PsetName);

                    Definition pDef = IfcPsetGroup.Definitions.get_Item(name);

                    CategorySet categorySet = GetCategorySetForOne(pNames[name], categorySets);
                    
                    if(categorySet.IsEmpty)
                    {
                        continue;
                    }

                    InstanceBinding instanceBinding = application.Create.NewInstanceBinding(categorySet);

                    doc.ParameterBindings.Insert(pDef, instanceBinding, ifcParameterGroup);
                }

                transaction.Commit();

                TaskDialog newDialog = new TaskDialog("Message!");

                newDialog.MainInstruction = "Success! Instance Binding!";

                newDialog.Show();
            }


            if (File.Exists(addParameterFilePath[1]))
            {
                string[] allLines = File.ReadAllLines(addParameterFilePath[1]);

                List<string> allIfcNames = new List<string>();

                Dictionary<string, List<string>> pNames = new Dictionary<string, List<string>>();

                foreach (var line in allLines)
                {
                    //separate string by tab

                    string[] values = line.Split('\t');

                    List<string> ifcNames = new List<string>();

                    for (int i = 1; i < values.Length; i++)
                    {
                        ifcNames.Add(values[i]);

                        if (!allIfcNames.Contains(values[i]))
                        {
                            allIfcNames.Add(values[i]);
                        }
                    }

                    pNames.Add(values[0], ifcNames);
                }

                Dictionary<string, CategorySet> categorySets = GetAllCategory(categories, classMappingTablePath, availaleCategoryPath, allIfcNames);

                Transaction transaction = new Transaction(doc, "Add Parameter_Type");

                transaction.Start();

                foreach (var name in pNames.Keys)
                {
                    string[] pName = name.Split('.');

                    string PName = pName[1];

                    string PsetName = pName[0];

                    DefinitionGroup IfcPsetGroup = definitionGroups.get_Item(PsetName);

                    string nameType= name + "[Type]";

                    Definition pDef = IfcPsetGroup.Definitions.get_Item(nameType);

                    CategorySet categorySet = GetCategorySetForOne(pNames[name], categorySets);

                    if (categorySet.IsEmpty)
                    {
                        continue;
                    }

                   TypeBinding typeBinding = application.Create.NewTypeBinding(categorySet);

                    doc.ParameterBindings.Insert(pDef, typeBinding, ifcParameterGroup);
                }

                transaction.Commit();

                TaskDialog newDialog = new TaskDialog("Success!");

                newDialog.MainInstruction = "Success! Type Binding!";

                newDialog.Show();
            }
            return Result.Succeeded;
        }

        private CategorySet GetCategorySetForOne(List<string> applicableIfcEntityList, Dictionary<string, CategorySet> categorySets)
        {
            CategorySet categorySetCombined = new CategorySet();

            foreach (var ifcEntity in applicableIfcEntityList)
            {
                if (categorySets.ContainsKey(ifcEntity))
                {
                    if (!categorySets[ifcEntity].IsEmpty)
                    {
                        foreach (Category category in categorySets[ifcEntity])
                        {
                            categorySetCombined.Insert(category);
                        }
                    }
                }
            }

            return categorySetCombined;
        }

        private string[] GenerateTXTFile(string fileFolderPath)
        {
            string filePath = Path.Combine(fileFolderPath,"IFC Mapping Table_Raw.txt");

            string mappingTablePath = Path.Combine(fileFolderPath, "IFC Mapping Table_Parameters.txt"); 

            string sharedParameterPath_Instance = Path.Combine(fileFolderPath, "IFC shared parameter_ToAdd_Instance.txt");

            string sharedParameterPath_Type = Path.Combine(fileFolderPath, "IFC shared parameter_ToAdd_Type.txt");

            try
            {
                string[] allLines = File.ReadAllLines(filePath);

                string pattern_Begin = @"^Ifc";

                string pattern_End = @"Type$";

                string record = null;

                Dictionary<string, List<string>> sharedParameter_Instance = new Dictionary<string, List<string>>();

                Dictionary<string, List<string>> sharedParameter_Type = new Dictionary<string, List<string>>();

                Regex regex_Begin = new Regex(pattern_Begin);

                Regex regex_End= new Regex(pattern_End);

                Dictionary<string, List<string>> mappingTable = new Dictionary<string, List<string>>();

                foreach (string line in allLines)
                {
                    if (line != "")
                    {
                        if (regex_Begin.IsMatch(line))
                        {
                            record = line;
                        }
                        else
                        {
                            string[] values = line.Split('\t');

                            string ifcPName = values[0] + @"." + values[1];

                            if (values[2] == ifcPName)
                            {
                                if(regex_End.IsMatch(record))
                                {
                                    if (sharedParameter_Type.ContainsKey(ifcPName))
                                    {
                                        string recordWithoutPostFix = record.Replace("Type", "");
                                        sharedParameter_Type[ifcPName].Add(recordWithoutPostFix);
                                    }
                                    else
                                    {
                                        sharedParameter_Type.Add(ifcPName, new List<string>());
                                        sharedParameter_Type[ifcPName].Add(ifcPName);
                                        string recordWithoutPostFix = record.Replace("Type", "");
                                        sharedParameter_Type[ifcPName].Add(recordWithoutPostFix);
                                    }
                                }
                                else
                                {
                                    if (sharedParameter_Instance.ContainsKey(ifcPName))
                                    {
                                        sharedParameter_Instance[ifcPName].Add(record);
                                    }
                                    else
                                    {
                                        sharedParameter_Instance.Add(ifcPName, new List<string>());
                                        sharedParameter_Instance[ifcPName].Add(ifcPName);
                                        sharedParameter_Instance[ifcPName].Add(record);
                                    }
                                }
                                
                            }
                            else
                            {
                                if (!mappingTable.ContainsKey(ifcPName))
                                {
                                    List<string> strings = new List<string>();

                                    for (int i = 0; i < 3; i++)
                                    {
                                        strings.Add(values[i]);
                                    }

                                    mappingTable.Add(ifcPName, strings);
                                }
                            }
                        }
                    }
                }

                using (StreamWriter writer = new StreamWriter(mappingTablePath))
                {
                    foreach (var value in mappingTable.Values)
                    {
                        string line = string.Join("\t", value);

                        writer.WriteLine(line);
                    }
                }

                using (StreamWriter writer = new StreamWriter(sharedParameterPath_Instance))
                {
                    foreach (var value in sharedParameter_Instance.Values)
                    {
                        string line = string.Join("\t", value);

                        writer.WriteLine(line);
                    }
                }

                using (StreamWriter writer = new StreamWriter(sharedParameterPath_Type))
                {
                    foreach (var value in sharedParameter_Type.Values)
                    {
                        string line = string.Join("\t", value);

                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException e)
            {
                TaskDialog newDialog=new TaskDialog("Error");
                
                newDialog.MainInstruction = "Error, file is empty or corrupted!";

                newDialog.Show();
            }

            string[] path = { sharedParameterPath_Instance, sharedParameterPath_Type };

            return path;
        }

        private Dictionary<string, CategorySet> GetAllCategory(Categories categories, string classMappingTablePath, string availaleCategoryPath, List<string> IFCNames)
        {
            Dictionary<string, CategorySet> categorySets = new Dictionary<string, CategorySet>();

            List<List<string>> records = new List<List<string>>();

            if (File.Exists(classMappingTablePath))
            {
                string[] allLines = File.ReadAllLines(classMappingTablePath);

                foreach (var line in allLines)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    List<string> record = new List<string>();

                    record = line.Split('\t').ToList();

                    records.Add(record);
                }
            }

            List<string> availableCategories = new List<string>();

            if (File.Exists(availaleCategoryPath))
            {
                string[] allLines = File.ReadAllLines(availaleCategoryPath);

                foreach (var line in allLines)
                {
                    string[] values=line.Split('\t');
                    availableCategories.Add(values[1]);
                }
            }

            List<string> errors=new List<string>();

            foreach (var IFCName in IFCNames)
            {
                CategorySet ifcCategorySet = new CategorySet();

                if (IFCName == "IfcBuilding" || IFCName == "IfcSite")
                {
                    Category category = categories.get_Item("Project Information");

                    ifcCategorySet.Insert(category);
                }
                else
                {
                    foreach (var record in records)
                    {
                        if (record[2] == IFCName)
                        {
                            try
                            {
                                Category category = categories.get_Item(record[0]);

                                if (record[1] != "")
                                {
                                    Category subCategory = category.SubCategories.get_Item(record[1]);

                                    if (availableCategories.Contains(subCategory.BuiltInCategory.ToString()))
                                    {
                                        ifcCategorySet.Insert(subCategory);
                                    }

                                }
                                else
                                {
                                    if (availableCategories.Contains(category.BuiltInCategory.ToString()))
                                    {
                                        ifcCategorySet.Insert(category);
                                    }
                                }
                            }
                            catch
                            {
                                string error = record[0] + "\\" + record[1];

                                errors.Add(error);
                            }

                        }
                    }
                }

                categorySets.Add(IFCName, ifcCategorySet);
            }

/*            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string test_Path = Path.Combine(desktopPath, "Missing Categories.txt");

            using (StreamWriter writer = new StreamWriter(test_Path))
            {
                foreach (string line in errors)
                {
                    writer.WriteLine(line);
                }
            }
*/


            return categorySets;

        }
    }
}
