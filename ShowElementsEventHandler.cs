using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using Color = Autodesk.Revit.DB.Color;

namespace Cust_IFC_Exporter
{
    [Transaction(TransactionMode.Manual)]
    public class ShowElementsEventHandler : IExternalEventHandler
    {

        public List<string> elementsGUIDs_Pass;

        public List<string> elementsGUIDs_Fail;

        public List<string> elementsGUIDs_Warning;

        List<ElementId> selectedEleID_Pass = new List<ElementId>();

        List<ElementId> selectedEleID_Warning = new List<ElementId>();

        List<ElementId> selectedEleID_Fail = new List<ElementId>();

        //public bool mode = false;

        //public ModeEnum mode;   

        public System.Windows.Media.Color argbColor_Pass;

        public System.Windows.Media.Color argbColor_Warning;

        public System.Windows.Media.Color argbColor_Fail;

        //public System.Windows.Media.Color argbColor_DoesNotApply;

        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;

            Document document = uidoc.Document;

            View3D default3DView = GetDefault3DView(document);

            if(elementsGUIDs_Fail.Count==0 && elementsGUIDs_Pass.Count==0 && elementsGUIDs_Warning.Count==0)
            {
                return;
            }

            selectedEleID_Pass.Clear();

            selectedEleID_Warning.Clear();

            selectedEleID_Fail.Clear();

            using (Transaction transaction = new Transaction(document, "Highlight selected elements"))
            {
                transaction.Start();

                try
                {
                    List<ElementId>  eleID =FilteredElementByInstance(document);

                    if (eleID.Count > 0)
                    {
                        default3DView.IsolateElementsTemporary(eleID);

                        default3DView.ConvertTemporaryHideIsolateToPermanent();

                        default3DView.DisplayStyle = DisplayStyle.FlatColors;

                        //Now override the graphic setting (highlight them) for all selected elements

                        FillPatternElement fillPatternElement;

                        fillPatternElement = FillPatternElement.GetFillPatternElementByName(document, FillPatternTarget.Drafting, @"<Solid fill>");

                        if (selectedEleID_Pass.Count > 0)
                        {
                            OverrideGraphicSettings overrideSettings_Pass = new OverrideGraphicSettings();

                            Color highlightColor_Pass = GetRGBColor(argbColor_Pass);

                            overrideSettings_Pass.SetSurfaceForegroundPatternId(fillPatternElement.Id);

                            overrideSettings_Pass.SetSurfaceForegroundPatternColor(highlightColor_Pass);

                            foreach (ElementId elementId in selectedEleID_Pass)
                            {
                                default3DView.SetElementOverrides(elementId, overrideSettings_Pass);
                            }
                        }

                        if (selectedEleID_Fail.Count() > 0)
                        {
                            OverrideGraphicSettings overrideSettings_Fail = new OverrideGraphicSettings();

                            Color highlightColor_Fail = GetRGBColor(argbColor_Fail);

                            overrideSettings_Fail.SetSurfaceForegroundPatternId(fillPatternElement.Id);

                            overrideSettings_Fail.SetSurfaceForegroundPatternColor(highlightColor_Fail);

                            foreach (ElementId elementId in selectedEleID_Fail)
                            {
                                default3DView.SetElementOverrides(elementId, overrideSettings_Fail);
                            }
                        }

                        if (selectedEleID_Warning.Count() > 0)
                        {

                            OverrideGraphicSettings overrideSettings_Warning = new OverrideGraphicSettings();

                            Color highlightColor_Warning = GetRGBColor(argbColor_Warning);

                            overrideSettings_Warning.SetSurfaceForegroundPatternId(fillPatternElement.Id);

                            overrideSettings_Warning.SetSurfaceForegroundPatternColor(highlightColor_Warning);

                            foreach (ElementId elementId in selectedEleID_Warning)
                            {
                                default3DView.SetElementOverrides(elementId, overrideSettings_Warning);
                            }
                        }

                        transaction.Commit();
                    }
                    else
                    {
                        transaction.RollBack();
                    }
 
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    transaction.RollBack();
                }
            }
        }

        private List<ElementId> FilterElemntByMode(Document document, bool mode)
        {
            if (mode)
            {
                return FilteredElementByType(document);
            }
            else
            {
                return FilteredElementByInstance(document);
            }

        }

        private List<ElementId> FilteredElementByType(Document document)
        {
            List<ElementId> selectedEleTypeID_Pass=new List<ElementId>();

            List<ElementId> selectedEleTypeID_Fail = new List<ElementId>();

            List<ElementId> selectedEleTypeID_Warning = new List<ElementId>();

            FilteredElementCollector allEleCollector = new FilteredElementCollector(document);

            ElementCategoryFilter gridFilter = new ElementCategoryFilter(BuiltInCategory.OST_Grids, true);

            ElementCategoryFilter elevationFilter = new ElementCategoryFilter(BuiltInCategory.OST_Levels, true);

            LogicalAndFilter Filter = new LogicalAndFilter(gridFilter, elevationFilter);

            ICollection<ElementId> allElementIds = allEleCollector.WherePasses(Filter).WhereElementIsElementType().ToElementIds();

            // Loop through all elements
            foreach (ElementId elementId in allElementIds)
            {
                Element element = document.GetElement(elementId);

                //Collect elements need to be highlighted in the same loop
                var para = element.LookupParameter("Type IfcGUID");

                string IfcGUID = null;

                if (para != null)
                {
                    IfcGUID = para.AsString();
                }

                if (IfcGUID != null)
                {
                    if (elementsGUIDs_Pass.Contains(IfcGUID))
                    {
                        selectedEleTypeID_Pass.Add(elementId);
                    }
                    else if (elementsGUIDs_Fail.Contains(IfcGUID))
                    {
                        selectedEleTypeID_Fail.Add(elementId);
                    }
                    else if (elementsGUIDs_Warning.Contains(IfcGUID))
                    {
                        selectedEleTypeID_Warning.Add(elementId);
                    }
                }
            }

            foreach (var id in selectedEleTypeID_Pass)
            {
                Element typeElement= document.GetElement(id);

                List<ElementId> dependentsELemnts = typeElement.GetDependentElements(null).ToList();

                foreach(var eleID in dependentsELemnts)
                {
                    Element ele = document.GetElement(eleID);

                    if (ele.GetTypeId() == id)
                    {
                        selectedEleID_Pass.Add(eleID);
                    }
                }
            }

            foreach (var id in selectedEleTypeID_Fail)
            {
                Element typeElement = document.GetElement(id);

                List<ElementId> dependentsELemnts = typeElement.GetDependentElements(null).ToList();

                foreach (var eleID in dependentsELemnts)
                {
                    Element ele = document.GetElement(eleID);

                    if (ele.GetTypeId() == id)
                    {
                        selectedEleID_Fail.Add(eleID);
                    }
                }
            }

            foreach (var id in selectedEleTypeID_Warning)
            {
                Element typeElement = document.GetElement(id);

                List<ElementId> dependentsELemnts = typeElement.GetDependentElements(null).ToList();

                foreach (var eleID in dependentsELemnts)
                {
                    Element ele = document.GetElement(eleID);

                    if (ele.GetTypeId() == id)
                    {
                        selectedEleID_Warning.Add(eleID);
                    }
                }
            }
            List<ElementId> eleIDs = selectedEleID_Pass.Concat(selectedEleID_Fail).Concat(selectedEleID_Warning).ToList();

            return eleIDs;
        }

        private List<ElementId> FilteredElementByInstance(Document document)
        {
            FilteredElementCollector allEleCollector = new FilteredElementCollector(document);

            ElementCategoryFilter gridFilter = new ElementCategoryFilter(BuiltInCategory.OST_Grids, true);

            ElementCategoryFilter elevationFilter = new ElementCategoryFilter(BuiltInCategory.OST_Levels, true);

            LogicalAndFilter Filter = new LogicalAndFilter(gridFilter, elevationFilter);

            ICollection<ElementId> allElementIds = allEleCollector.WherePasses(Filter).WhereElementIsNotElementType().ToElementIds();

            // Loop through all elements
            foreach (ElementId elementId in allElementIds)
            {
                Element element = document.GetElement(elementId);

                //Collect elements need to be highlighted in the same loop
                var para = element.LookupParameter("IfcGUID");

                string IfcGUID = null;

                if (para != null)
                {
                    IfcGUID = para.AsString();
                }

                if (IfcGUID != null)
                {
                    if (elementsGUIDs_Pass.Contains(IfcGUID))
                    {
                        selectedEleID_Pass.Add(elementId);
                    }
                    else if (elementsGUIDs_Fail.Contains(IfcGUID))
                    {
                        selectedEleID_Fail.Add(elementId);
                    }
                    else if (elementsGUIDs_Warning.Contains(IfcGUID))
                    {
                        selectedEleID_Warning.Add(elementId);
                    }
                }
            }

            List<ElementId> eleID = selectedEleID_Pass.Concat(selectedEleID_Fail).Concat(selectedEleID_Warning).ToList();

            return eleID;
        }

        private Color GetRGBColor(System.Windows.Media.Color argbColor)
        {
            if (argbColor.A == 255)
                return new Color(argbColor.R,argbColor.G,argbColor.B);

            System.Windows.Media.Color background = System.Windows.Media.Colors.White;
            var alpha = argbColor.A / 255.0;
            var diff = 1.0 - alpha;
            return new Color(
                (byte)(argbColor.R * alpha + background.R * diff),
                (byte)(argbColor.G * alpha + background.G * diff),
                (byte)(argbColor.B * alpha + background.B * diff));

        }

        private View3D GetDefault3DView(Document document)
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);
            ICollection<Element> views = collector.OfClass(typeof(View3D)).ToElements();

            foreach (Element view in views)
            {
                View3D view3D = view as View3D;
                if (view3D != null && view3D.IsTemplate == false && view3D.Name == @"{3D}")
                {
                    // "3D" is the default name for the default 3D view in Revit
                    return view3D;
                }
            }

            return null;
        }

        public string GetName()
        {
            return "Show Elements Eevnt";
        }
    }
}
