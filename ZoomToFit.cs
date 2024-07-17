using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cust_IFC_Exporter
{
    internal class ZoomToFit : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument updatedUIDoc = app.ActiveUIDocument;

            View3D default3DView=updatedUIDoc.ActiveView as View3D;

            UIView uiView = updatedUIDoc.GetOpenUIViews().FirstOrDefault(v => v.ViewId == default3DView.Id);

            uiView.ZoomToFit();


            
        }

        public string GetName()
        {
            return "Reset The Camera";
        }
    }
}
