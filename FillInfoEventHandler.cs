using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Document = Autodesk.Revit.DB.Document;

namespace Cust_IFC_Exporter
{
    public class FillInfoEventHandler : IExternalEventHandler
    {
        public ValidateResultsByElement eleResult;

        public bool mode = false; //True Type, False Instance
        public void Execute(UIApplication app)
        {

            UIDocument uidoc = app.ActiveUIDocument;

            Document document = uidoc.Document;

            using (Transaction transaction = new Transaction(document, "Highlight selected element"))
            {
                transaction.Start();

                if (eleResult.Results[0].IfcType == "IfcProject" || eleResult.Results[0].IfcType == "IfcSite" || eleResult.Results[0].IfcType == "IfcBuilding")
                {
                    app.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.ProjectInformation));
                }
                else
                {
                    try
                    {
                        //Find Ele ID

                        ElementId selectId = FindSelectedID(document, mode);

                        Type type = document.GetElement(selectId).GetType();

                        if (selectId != null)
                        {
                            ElementInfoFill newWindow = new ElementInfoFill(document,eleResult, selectId, type.ToString());

                            bool? dialogResult = newWindow.ShowDialog();

                            if(dialogResult.HasValue && dialogResult.Value)
                            {
                                Dictionary<ElementId,Dictionary<string,bool>> failedParameters = new Dictionary<ElementId, Dictionary<string, bool>>();

                                Element ele= document.GetElement(selectId);

                                foreach (var row in newWindow.FailedPara)
                                {
                                    if(string.IsNullOrEmpty(row.Value))
                                    {
                                        continue;
                                    }

                                    Parameter parameter = ele.LookupParameter(row.RevitParameterName);

                                    if (parameter != null)
                                    {
                                        bool result = false;

                                        if (parameter.StorageType == StorageType.String)
                                        {
                                            result = parameter.Set(row.Value);
                                        }
                                        else if (parameter.StorageType == StorageType.Integer)
                                        {

                                            if (row.Value.ToLower() == "true")
                                            {
                                                result = parameter.Set(1);

                                            }
                                            else if (row.Value.ToLower() == "false")
                                            {
                                                result = parameter.Set(0);
                                            }
                                            else
                                            {
                                                result = parameter.Set(Convert.ToInt32(row.Value));
                                            }
                                        }
                                        else if (parameter.StorageType == StorageType.Double)
                                        {
                                            result = parameter.Set(Convert.ToDouble(row.Value));
                                        }
                                        else
                                        {
                                            result = parameter.SetValueString(row.Value);
                                        }

                                        if(!result)
                                        {
                                            if (failedParameters.ContainsKey(selectId))
                                            {
                                                failedParameters[selectId].Add(row.Value,result);
                                            }
                                            else
                                            {
                                                failedParameters.Add(selectId, new Dictionary<string, bool>() { { row.Value, result } });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error! Could not find the parameter!");
                                    }
                                }

                                foreach (var row in newWindow.WarningPara)
                                {
                                    if (string.IsNullOrEmpty(row.Value))
                                    {
                                        continue;
                                    }

                                    Parameter parameter = ele.LookupParameter(row.RevitParameterName);

                                    if (parameter != null)
                                    {
                                        bool result = false;

                                        if (parameter.StorageType == StorageType.String)
                                        {
                                            result = parameter.Set(row.Value);
                                        }
                                        else if (parameter.StorageType == StorageType.Integer)
                                        {
                                            if (row.Value.ToLower() == "true")
                                            {
                                                result = parameter.Set(1);

                                            }
                                            else if (row.Value.ToLower() == "false")
                                            {
                                                result = parameter.Set(0);
                                            }
                                            else
                                            {
                                                result = parameter.Set(Convert.ToInt32(row.Value));
                                            }
                                        }
                                        else if (parameter.StorageType == StorageType.Double)
                                        {
                                            result = parameter.Set(Convert.ToDouble(row.Value));
                                        }
                                        else
                                        {
                                            result = parameter.SetValueString(row.Value);
                                        }

                                        if (!result)
                                        {
                                            if (failedParameters.ContainsKey(selectId))
                                            {
                                                failedParameters[selectId].Add(row.Value, result);
                                            }
                                            else
                                            {
                                                failedParameters.Add(selectId, new Dictionary<string, bool>() { { row.Value, result } });
                                            }
                                        }

                                    }
                                    else
                                    {
                                        MessageBox.Show("Error! Could not find the parameter!");
                                    }
                                }

                                if(failedParameters.Count>0)
                                {
                                    string message = "The following parameters failed to be set:\n";

                                    foreach (var item in failedParameters)
                                    {
                                        message += "Element ID: " + item.Key.ToString() + "\n";

                                        foreach (var para in item.Value)
                                        {
                                            message += "Parameter: " + para.Key + "\n";
                                        }
                                    }

                                    MessageBox.Show(message);
                                }
                            }

                        }
                        else
                        {
                            MessageBox.Show("Error! Could not find the element!");
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        transaction.RollBack();
                    }
                }              
            }
        }

        private ElementId FindSelectedID(Document document, bool mode)
        {

            //Find Ele ID
            FilteredElementCollector allEleCollector = new FilteredElementCollector(document);

            ElementCategoryFilter gridFilter = new ElementCategoryFilter(BuiltInCategory.OST_Grids, true);

            ICollection<ElementId> allElementIds = allEleCollector.WherePasses(gridFilter).ToElementIds();

            string Ifc_GUID = eleResult.Results[0].Ifc_GUID;

            ElementId selectId = null;

            if (mode)
            {
                foreach (ElementId elementId in allElementIds)
                {
                    Element element = document.GetElement(elementId);

                    var para = element.LookupParameter("Type IfcGUID");// use Type IfcGUID for Type

                    if (para != null)
                    {
                        if (Ifc_GUID == para.AsString())
                        {
                            //category = element.Category;
                            //type = element.GetType();
                            selectId = elementId;
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (ElementId elementId in allElementIds)
                {
                    Element element = document.GetElement(elementId);

                    var para = element.LookupParameter("IfcGUID");// use Type IfcGUID for Type

                    if (para != null)
                    {
                        if (Ifc_GUID == para.AsString())
                        {
                            //category = element.Category;
                            //type = element.GetType();
                            selectId = elementId;
                            break;
                        }
                    }
                }
            }           
            return selectId;
            
        }

        private Color GetRGBColor(System.Windows.Media.Color argbColor)
        {
            if (argbColor.A == 255)
                return new Color(argbColor.R, argbColor.G, argbColor.B);

            System.Windows.Media.Color background = System.Windows.Media.Colors.White;
            var alpha = argbColor.A / 255.0;
            var diff = 1.0 - alpha;
            return new Color(
                (byte)(argbColor.R * alpha + background.R * diff),
                (byte)(argbColor.G * alpha + background.G * diff),
                (byte)(argbColor.B * alpha + background.B * diff));

        }

        public string GetName()
        {
            return "Fill Info Event";
        }
    }
}
