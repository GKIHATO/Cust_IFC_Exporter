using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Cust_IFC_Exporter
{
    [Transaction(TransactionMode.Manual)]
    internal class ResetViewEventHandler : IExternalEventHandler
    {

        View3D default3DView = null;
        public void Execute(UIApplication app)
        {
            //Now acquire the default 3D view and center it

            UIDocument updatedUIDoc = app.ActiveUIDocument;

            Document document = updatedUIDoc.Document;

            default3DView = GetDefault3DView(document);

            using (Transaction transaction = new Transaction(document, "Reset View"))
            {
                transaction.Start();

                try
                {
                    if (default3DView != null)
                    {
                        updatedUIDoc.ActiveView = default3DView;

                        //Now show hidden elements if there any

                        //Get all elements in the document
                        FilteredElementCollector gridCollector = new FilteredElementCollector(document);

                        ICollection<ElementId> gridsElementIds = gridCollector.OfCategory(BuiltInCategory.OST_Grids).WhereElementIsNotElementType().ToElementIds();

                        FilteredElementCollector elevationCollector = new FilteredElementCollector(document);

                        ICollection<ElementId> elevationElementIds = elevationCollector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElementIds();

                        FilteredElementCollector allEleCollector = new FilteredElementCollector(document);

                        ElementCategoryFilter gridFilter = new ElementCategoryFilter(BuiltInCategory.OST_Grids,true);
                        
                        ElementCategoryFilter elevationFilter = new ElementCategoryFilter(BuiltInCategory.OST_Levels, true);

                        LogicalAndFilter Filter = new LogicalAndFilter(gridFilter, elevationFilter);

                        ICollection<ElementId> allElementIds = allEleCollector.WherePasses(Filter).WhereElementIsNotElementType().ToElementIds();

                        List<ElementId> hiddenEleID = new List<ElementId>();

 /*                       FillPatternElement fillPatternElement;

                        fillPatternElement = FillPatternElement.GetFillPatternElementByName(document, FillPatternTarget.Drafting, @"<No pattern>");

                        OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();

                        overrideSettings.SetSurfaceForegroundPatternId(fillPatternElement.Id);*/

                        // Loop through all elements
                        foreach (ElementId elementId in allElementIds)
                        {
                            Element element = document.GetElement(elementId);

                            //Clear the graphic override settings
                            default3DView.SetElementOverrides(elementId, new OverrideGraphicSettings());

                            // Check if the element is hidden in the current view
                            if (element.IsHidden(default3DView))
                            {
                                // Add to the list if it is hidden
                                hiddenEleID.Add(elementId);
                            }
                        }
                        //Unhide all elements

                        if (hiddenEleID.Count() > 0)
                        {
                            default3DView.UnhideElements(hiddenEleID);
                        }

                        //Hide Grid
                        if(gridsElementIds != null && gridsElementIds.Count() > 0)
                        {
                            default3DView.HideElements(gridsElementIds);

                        }

                        //Hide Elevation marks
                        if (elevationElementIds != null && elevationElementIds.Count() > 0)
                        {
                            
                            default3DView.HideElements(elevationElementIds);
                        }

                        //Now turn off section box if it was opened

                        if (default3DView.IsSectionBoxActive)
                        {
                            default3DView.IsSectionBoxActive = false;
                        }

                        //Now change the graphic display setting to shading
                        default3DView.DisplayStyle = DisplayStyle.Shading;

                        //Reset the camera 
/*
                        UIView uiView = updatedUIDoc.GetOpenUIViews().FirstOrDefault(v => v.ViewId == default3DView.Id);

                        uiView.ZoomToFit();*/
                    }
                    else
                    {
                        MessageBox.Show("Empty View!");
                    }
                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    transaction.RollBack();
                }

            }
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
            return "Reset View Event";
        }
    }
}
