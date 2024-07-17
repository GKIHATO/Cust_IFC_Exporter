using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cust_IFC_Exporter
{
    [Transaction(TransactionMode.Manual)]
    class AddNew3DViewCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Find the default 3D view template
            ViewFamilyType viewFamilyType3D = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(v => v.ViewFamily == ViewFamily.ThreeDimensional && v.CanBeCopied);

            View3D default3DViewInstance;

            if (viewFamilyType3D != null)
            {
                // Create an instance of the default 3D view from the view template
                using (Transaction transaction = new Transaction(doc, "Create 3D View"))
                {
                    transaction.Start();

                    default3DViewInstance = View3D.CreateIsometric(doc, viewFamilyType3D.Id);

                    // Optionally set the name of the new view
                    default3DViewInstance.Name = "My 3D View";

                    transaction.Commit();
                }

                // Activate the newly created 3D view instance
                uiDoc.ActiveView = default3DViewInstance;

                // Center the view within the Revit window
                UIView uiView = uiDoc.GetOpenUIViews().FirstOrDefault(v => v.ViewId == default3DViewInstance.Id);
                if (uiView != null)
                {
                    uiView.ZoomToFit();
                }
            }
            else
            {
                TaskDialog.Show("Default 3D View", "No suitable 3D view template found.");
            }
            return Result.Succeeded;
        }
    }
}
