using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.IFC;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Revit.IFC.Export.Exporter.PropertySet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xbim.IO.Step21;

namespace Cust_IFC_Exporter
{
    [Transaction(TransactionMode.Manual)]
    class PopulateDataCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;

            Document doc = uiapp.ActiveUIDocument.Document;

            Transaction transaction = new Transaction(doc, "Populate Data");

            transaction.Start();

            //SetStructuralConnections(doc);

            SetWalls(doc);

            //SetCurtainPanels(doc);

            SetFloors(doc);

            SetCeilings(doc);

            SetRoofs(doc);

            SetGutters(doc);

            SetStairs(doc);

            SetRamps(doc);

            SetRailing(doc);

            SetArchitecturalColumns(doc);

            SetStructuralColumns(doc);

            SetBeam(doc);

            SetMullions(doc);

            SetFoundation(doc);

            SetTank(doc);

            SetWindows(doc);

            SetDoors(doc);

            SetGenericModels(doc);

            SetPlumbingFixture(doc);

            SetFurniture(doc);

            SetCommunicationDevices(doc);

            transaction.Commit();

            //need to set beam

            //need to set foundation

            return Result.Succeeded;
        }

        private void SetCommunicationDevices(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_CommunicationDevices)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                string manufacturer_string = null;

                if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                {
                    manufacturer_string = manufacturer_Type.AsString();
                }
                else
                {
                    manufacturer_string = "XXX";
                }

                string model_string = null;

                if (!string.IsNullOrEmpty(model_Type.AsString()))
                {
                    model_string = model_Type.AsString();
                }
                else
                {
                    model_string = "XXX";
                }

                if (Guid.TryParse(mark_Type.AsString(), out Guid result))
                {
                    mark_Type.Set(mark_Type.AsString() + "-" + "XXX"+"-0");
                }
                else
                {
                    var category = "Furniture";

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX"+"-0";

                    mark_Type.Set(comments_string);
                }

            }
        }//L

        private void SetFurniture(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Furniture)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                string manufacturer_string = null;

                if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                {
                    manufacturer_string = manufacturer_Type.AsString();
                }
                else
                {
                    manufacturer_string = "XXX";
                }

                string model_string = null;

                if (!string.IsNullOrEmpty(model_Type.AsString()))
                {
                    model_string = model_Type.AsString();
                }
                else
                {
                    model_string = "XXX";
                }

                if (Guid.TryParse(mark_Type.AsString(), out Guid result))
                {
                    mark_Type.Set(mark_Type.AsString() + "-" + "XXX" + "-0");

                }
                else
                {

                    var category = "Furniture";

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-0";

                    mark_Type.Set(comments_string);
                }

            }
        }//L

        private void SetPlumbingFixture(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_PlumbingFixtures)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                string manufacturer_string = null;

                if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                {
                    manufacturer_string = manufacturer_Type.AsString();
                }
                else
                {
                    manufacturer_string = "XXX";
                }

                string model_string = null;

                if (!string.IsNullOrEmpty(model_Type.AsString()))
                {
                    model_string = model_Type.AsString();
                }
                else
                {
                    model_string = "XXX";
                }

                if (Guid.TryParse(mark_Type.AsString(), out Guid result))
                {
                    mark_Type.Set(mark_Type.AsString() + "-" + "XXX" + "-0");

                }
                else
                {

                    var category = "PlumbingFixture";

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-0";

                    mark_Type.Set(comments_string);
                }

            }
        }//L

        private void SetGenericModels(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_GenericModel)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                string manufacturer_string = null;

                if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                {
                    manufacturer_string = manufacturer_Type.AsString();
                }
                else
                {
                    manufacturer_string = "XXX";
                }

                string model_string = null;

                if (!string.IsNullOrEmpty(model_Type.AsString()))
                {
                    model_string = model_Type.AsString();
                }
                else
                {
                    model_string = "XXX";
                }

                if (Guid.TryParse(mark_Type.AsString(), out Guid result))
                {
                    mark_Type.Set(mark_Type.AsString() + "-" + "XXX" + "-0");

                }
                else
                {
                    var category = "Brick"; //can't find the material for the generic model, set as brick for the case study

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-0";

                    mark_Type.Set(comments_string);
                }

            }
        }//Has Issues

        private void SetDoors(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Doors)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                string manufacturer_string = null;

                if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                {
                    manufacturer_string = manufacturer_Type.AsString();
                }
                else
                {
                    manufacturer_string = "XXX";
                }

                string model_string = null;

                if (!string.IsNullOrEmpty(model_Type.AsString()))
                {
                    model_string = model_Type.AsString();
                }
                else
                {
                    model_string = "XXX";
                }

                if (Guid.TryParse(mark_Type.AsString(), out Guid result))
                {
                    mark_Type.Set(mark_Type.AsString() + "-" + "XXX" + "-0");

                }
                else
                {

                    var category = "Door";

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-0";

                    mark_Type.Set(comments_string);
                }

            }
        }//L

        private void SetWindows(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Windows)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);
               
                string manufacturer_string = null;

                if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                {
                    manufacturer_string = manufacturer_Type.AsString();
                }
                else
                {
                    manufacturer_string = "XXX";
                }

                string model_string = null;

                if (!string.IsNullOrEmpty(model_Type.AsString()))
                {
                    model_string = model_Type.AsString();
                }
                else
                {
                    model_string = "XXX";
                }

                if (Guid.TryParse(mark_Type.AsString(), out Guid result))
                {
                    mark_Type.Set(mark_Type.AsString() + "-" + "XXX"+"-0");

                }
                else
                {

                    var category = "Window";

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-0";

                    mark_Type.Set(comments_string);
                }

            }
        }//L

        private void SetGutters(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Gutter)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                var para = ele.LookupParameter("Material");

                if (para == null)
                {
                    continue;
                }

                ElementId materialID = para.AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if (material != null)
                {
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_string = manufacturer_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_Type.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_string = model_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();

                        model_Type.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }
                   
                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_Type.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if(string.IsNullOrEmpty(description_Type.AsString()) && !string.IsNullOrEmpty(description.AsString()))
                        {
                            description_Type.Set(description.AsString());
                        }
                    }
                    else
                    {
                        var category = "Gutter";

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && !string.IsNullOrEmpty(year.AsString()))
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && !string.IsNullOrEmpty(region.AsString()))
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_Type.Set(comments_string);
                    }
                }
            }
        }//L

        private void SetTank(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_MechanicalEquipment)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                var para = ele.LookupParameter("Tank Material");

                if (para == null)
                {
                    continue;
                }

                ElementId materialID = para.AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if (material != null)
                {
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_string = manufacturer_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_Type.Set(manufacturer.AsString());
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_string = model_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();

                        model_Type.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }
                 
                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_Type.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if (string.IsNullOrEmpty(description_Type.AsString()) && !string.IsNullOrEmpty(description.AsString()))
                        {
                            description_Type.Set(description.AsString());
                        }
                    }
                    else
                    {                    
                        
                        var category = "Tank";

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && !string.IsNullOrEmpty(year.AsString()))
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && !string.IsNullOrEmpty(region.AsString()))
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_Type.Set(comments_string);
                    }
                }
            }
        }//L

        private void SetFoundation(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructuralFoundation)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                /*FamilySymbol familySymbol = ele as FamilySymbol;

                if(familySymbol == null)
                {
                    continue;
                }

                List<ElementId> familyInstances= familySymbol.GetDependentElements(new ElementClassFilter(typeof(FamilyInstance))).ToList();

                List<ElementId> materialIDs = new List<ElementId>();

                foreach(var instance in familyInstances)
                {
                    FamilyInstance familyInstance = doc.GetElement(instance) as FamilyInstance;

                    if(familyInstance == null)
                    {
                        continue;
                    }

                    var para = familyInstance.LookupParameter("Structural Material");

                    if (para == null)
                    {
                        continue;
                    }

                    ElementId materialID = para.AsElementId();

                    materialIDs.Add(materialID);
                }
         

                if(materialIDs.Distinct().Count()==1)
                {
                    Material material = doc.GetElement(materialIDs[0]) as Material;

                    if (material != null)
                    {

                        var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                        Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                        var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        description_Type.Set(description.AsString());

                        manufacturer_Type.Set(manufacturer.AsString());

                        model_Type.Set(model.AsString());

                        mark_Type.Set(mark.AsString());
                    }
                }*/

                var para = ele.LookupParameter("Structural Material");

                if (para == null)
                {
                    continue;
                }

                ElementId materialID = para.AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if (material != null)
                {
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_string = manufacturer_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_Type.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_string = model_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();

                        model_Type.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }
                    
                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_Type.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if (string.IsNullOrEmpty(description_Type.AsString()) && !string.IsNullOrEmpty(description.AsString()))
                        {
                            description_Type.Set(description.AsString());
                        }
                    }
                    else
                    {
                        var category = material.MaterialClass;

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && !string.IsNullOrEmpty(year.AsString()))
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && !string.IsNullOrEmpty(region.AsString()))
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_Type.Set(comments_string);
                    }
                }
            }
        }//L

/*        private void SetBeam(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                FamilySymbol familySymbol = ele as FamilySymbol;

                if (familySymbol == null)
                {
                    continue;
                }

                List<ElementId> familyInstances = familySymbol.GetDependentElements(new ElementClassFilter(typeof(FamilyInstance))).ToList();

                List<ElementId> materialIDs = new List<ElementId>();

                foreach (var instance in familyInstances)
                {
                    FamilyInstance familyInstance = doc.GetElement(instance) as FamilyInstance;

                    if (familyInstance == null)
                    {
                        continue;
                    }

                    var para = familyInstance.LookupParameter("Structural Material");

                    if (para == null)
                    {
                        continue;
                    }

                    ElementId materialID = para.AsElementId();

                    materialIDs.Add(materialID);
                }         

                if (materialIDs.Distinct().Count() > 1)
                {
                    Material material = doc.GetElement(materialIDs[0]) as Material;

                    if (material != null)
                    {
                        var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                        Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                        var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);



                        description_Type.Set(description.AsString());

                        manufacturer_Type.Set(manufacturer.AsString());

                        model_Type.Set(model.AsString());

                        mark_Type.Set(mark.AsString());
                    }
                }
                else if(materialIDs.Distinct().Count() ==1)
                {
                    Material material = doc.GetElement(materialIDs[0]) as Material;

                    if (material != null)
                    {
                        var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                        Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                        var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        var category = material.MaterialClass;

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && year.AsString() != "")
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && region.AsString() != "")
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && regionLevel.AsString() != "" )
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string manufacturer_string = null;

                        if (manufacturer_Type.AsString() != "")
                        {
                            manufacturer_string = manufacturer_Type.AsString();
                        }
                        else if (manufacturer.AsString() != "")
                        {
                            manufacturer_string = manufacturer.AsString();
                        }
                        else
                        {
                            manufacturer_string = "XXX";
                        }

                        string model_string = null;

                        if (model_Type.AsString() != "")
                        {
                            model_string = model_Type.AsString();
                        }
                        else if (model.AsString() != "")
                        {
                            model_string = model.AsString();
                        }
                        else
                        {
                            model_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer.AsString() + "-" + model.AsString() + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX";

                        if (Guid.TryParse(mark.AsString(), out Guid result))
                        {
                            mark_Type.Set(comments_string);
                        }
                        else
                        {
                            mark_Type.Set(mark.AsString() + "-" + "XXX");

                            manufacturer_Type.Set(manufacturer.AsString());

                            model_Type.Set(model.AsString());

                            description_Type.Set(description.AsString());
                        }
                    }
                }
            }
        }*/

        private void SetBeam(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                var para = ele.LookupParameter("Structural Material");

                if (para == null)
                {
                    continue;
                }

                ElementId materialID = para.AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if (material != null)
                {
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_string = manufacturer_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_Type.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_string = model_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();

                        model_Type.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }
                    
                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_Type.Set(mark.AsString() + "-" + "XXX"+"-"+density.ToString());

                        if(string.IsNullOrEmpty(description_Type.AsString()) && !string.IsNullOrEmpty(description.AsString()))
                        {
                            description_Type.Set(description.AsString());
                        }
                    }
                    else
                    {                        
                        var category = material.MaterialClass;

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && !string.IsNullOrEmpty(year.AsString()))
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && !string.IsNullOrEmpty(region.AsString()))
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_Type.Set(comments_string);
                    }
                }
            }
        }//L

        private void SetMullions(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_CurtainWallMullions)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                var para = ele.LookupParameter("Material");

                if (para == null)
                {
                    continue;
                }

                ElementId materialID = para.AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if (material != null)
                {
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_string = manufacturer_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_Type.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_string = model_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();

                        model_Type.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }

                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_Type.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if (string.IsNullOrEmpty(description_Type.AsString()) && !string.IsNullOrEmpty(description.AsString()))
                        {
                            description_Type.Set(description.AsString());
                        }
                    }
                    else
                    {
                        var category = material.MaterialClass;

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && !string.IsNullOrEmpty(year.AsString()))
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && !string.IsNullOrEmpty(region.AsString()))
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_Type.Set(comments_string);
                    }
                }
            }
        }

        private void SetArchitecturalColumns(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Columns)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                var para = ele.LookupParameter("Material");

                if (para == null)
                {
                    continue;
                }

                ElementId materialID = para.AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if (material != null)
                {
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_string = manufacturer_Type.AsString();
                    }
                    else if(!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_Type.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_string = model_Type.AsString();
                    }
                    else if(!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();

                        model_Type.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }
                    
                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_Type.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if (string.IsNullOrEmpty(description_Type.AsString()) && !string.IsNullOrEmpty(description.AsString()))
                        {
                            description_Type.Set(description.AsString());
                        }
                    }
                    else
                    {
                        var category = material.MaterialClass;

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && !string.IsNullOrEmpty(year.AsString()))
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && !string.IsNullOrEmpty(region.AsString()))
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_Type.Set(comments_string);
                    }
                }
            }
        }//L

        private void SetStructuralColumns(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns)).WhereElementIsElementType();

            foreach (var ele in collector)
            {
                var para = ele.LookupParameter("Structural Material");

                if (para == null)
                {
                    continue;
                }

                ElementId materialID = para.AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if (material != null)
                {
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if(!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_string = manufacturer_Type.AsString();
                    }
                    else if(!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_Type.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_string = model_Type.AsString();
                    }
                    else if(!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();
                        
                        model_Type.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }
                   
                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {                       
                        mark_Type.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if (string.IsNullOrEmpty(description_Type.AsString()) && !string.IsNullOrEmpty(description.AsString()))
                        {
                            description_Type.Set(description.AsString());
                        }
                    }
                    else
                    {
                        var category = material.MaterialClass;

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && !string.IsNullOrEmpty(year.AsString()))
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && !string.IsNullOrEmpty(region.AsString()))
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_Type.Set(comments_string);
                    }
                }
            }
        }//L

        private void SetRamps(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Ramps)).WhereElementIsElementType();

            foreach(var ele in collector)
            {
                var para = ele.LookupParameter("Ramp Material");

                if (para == null)
                {
                    continue;
                }

                ElementId materialID = para.AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if(material!=null)
                {
                    
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_string = manufacturer_Type.AsString();

                           
                    }
                    else if(!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_Type.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_string = model_Type.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();

                        model_Type.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }
                    
                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_Type.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if(string.IsNullOrEmpty(description_Type.AsString()) && !string.IsNullOrEmpty(description.AsString()))
                        {
                            description_Type.Set(description.AsString());
                        }
                    }
                    else
                    {
                        var category = material.MaterialClass;

                        var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year != null && !string.IsNullOrEmpty(year.AsString()))
                        {
                            year_string = year.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region != null && !string.IsNullOrEmpty(region.AsString()))
                        {
                            region_string = region.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                        {
                            regionLevel_string = regionLevel.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_Type.Set(comments_string);
                    }
                }
            }
        }//L

        private void SetStairs(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(StairsType)));

            foreach (var ele in collector)
            {
                StairsType stairType = ele as StairsType;

                if (stairType == null)
                {
                    continue;
                }
                
                StairsRunType stairsRunType = doc.GetElement(stairType.LookupParameter("Run Type").AsElementId()) as StairsRunType;
                
                ElementId runMaterialID= stairsRunType.MaterialId;

                Material runMaterial = doc.GetElement(runMaterialID) as Material;

                string commentsString_Run = null;

                string descriptionString_Run = null;

                string manufacturerString_Run = null;

                string modelString_Run = null;

                string yearString_Run = null;

                string regionString_Run = null;

                string regionLevelString_Run = null;

                string manufacturerString_Landing = null;

                string modelString_Landing = null;

                string yearString_Landing = null;

                string regionString_Landing = null;

                string regionLevelString_Landing = null;

                string commentsString_Landing = null;   

                string descriptionString_Landing = null;

                if (runMaterial != null)
                {
                    var manufacturer_RunMaterial =runMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_RunType = stairsRunType.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model_RunMaterial = runMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_RunType = stairsRunType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark_RunMaterial = runMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_RunType = stairsRunType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description_RunMaterial = runMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_RunType = stairsRunType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);                

                    var region_RunType = stairsRunType.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    var regionLevel_RunType = stairsRunType.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    var year_RunType = stairsRunType.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    PropertySetElement propertySetElement = doc.GetElement(runMaterial.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    if (year_RunType != null && !string.IsNullOrEmpty(year_RunType.AsString()))
                    {
                        yearString_Run = year_RunType.AsString();
                    }
                    else
                    {
                        yearString_Run = "XXX";
                    }

                    if (region_RunType != null && !string.IsNullOrEmpty(region_RunType.AsString()))
                    {
                        regionString_Run = region_RunType.AsString();
                    }
                    else
                    {
                        regionString_Run = "XXX";
                    }

                    if (regionLevel_RunType != null && !string.IsNullOrEmpty(regionLevel_RunType.AsString()))
                    {
                        regionLevelString_Run = regionLevel_RunType.AsString();
                    }
                    else
                    {
                        regionLevelString_Run = "XXX";
                    }

                    if (!string.IsNullOrEmpty(manufacturer_RunType.AsString()))
                    {
                        manufacturerString_Run = manufacturer_RunType.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer_RunMaterial.AsString()))
                    {
                        manufacturerString_Run = manufacturer_RunMaterial.AsString();

                        manufacturer_RunType.Set(manufacturerString_Run);
                    }
                    else
                    {
                        manufacturerString_Run = "XXX";
                    }

                    if (!string.IsNullOrEmpty(model_RunType.AsString()))
                    {
                        modelString_Run = model_RunType.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model_RunMaterial.AsString()))
                    {
                        modelString_Run = model_RunMaterial.AsString();

                        model_RunType.Set(modelString_Run);
                    }
                    else
                    {
                        modelString_Run = "XXX";
                    }
                
                    if (Guid.TryParse(mark_RunMaterial.AsString(), out Guid result))
                    {                  
                        commentsString_Run=mark_RunMaterial.AsString() + "-" + "XXX" + "-" + density.ToString();

                        if(string.IsNullOrEmpty(description_RunType.AsString()) && !string.IsNullOrEmpty(description_RunMaterial.AsString()))
                        {
                            description_RunType.Set(description_RunMaterial.AsString());
                        }                      
                    }
                    else
                    {
                        var category_RunMaterial = runMaterial.MaterialClass;                    
                        
                        commentsString_Run=category_RunMaterial + "-" + manufacturerString_Run + "-" + modelString_Run + "-" + yearString_Run + "-" + regionLevelString_Run + "-" + regionString_Run + "-" + "XXX" + "-" + density.ToString();                    

                    }

                    mark_RunType.Set(commentsString_Run);

                    descriptionString_Run = description_RunType.AsString();

                    /* description_Type.Set(description.AsString());

                     manufacturer_Type.Set(manufacturer.AsString());

                     model_Type.Set(model.AsString());

                     mark_Type.Set(mark.AsString());

                     manufacturer_string = manufacturer.AsString();  

                     model_string = model.AsString();

                     mark_string = mark.AsString();

                     description_string = description.AsString();*/
                }

                StairsLandingType stairsLandingType = doc.GetElement(stairType.LookupParameter("Landing Type").AsElementId()) as StairsLandingType;

                Parameter materialPara = stairsLandingType.LookupParameter("Monolithic Material");

                ElementId landingMaterialID=ElementId.InvalidElementId;

                if (materialPara!=null)
                {
                   landingMaterialID = materialPara.AsElementId();
                }

                Material landingMaterial = doc.GetElement(landingMaterialID) as Material;

                if (landingMaterial != null)
                {
                    var manufacturer_LandingMaterial = landingMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_LandingType = stairsLandingType.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model_LandingMaterial = landingMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_LandingType = stairsLandingType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark_LandingMaterial = landingMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_LandingType = stairsLandingType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description_LandingMaterial = landingMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_LandingType = stairsLandingType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    var region_LandingType = stairsLandingType.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    var regionLevel_LandingType = stairsLandingType.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    var year_LandingType = stairsLandingType.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    PropertySetElement propertySetElement = doc.GetElement(landingMaterial.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    if (year_LandingType != null && !string.IsNullOrEmpty(year_LandingType.AsString()))
                    {
                        yearString_Landing = year_LandingType.AsString();
                    }
                    else
                    {
                        yearString_Landing = "XXX";
                    }

                    if (region_LandingType != null && !string.IsNullOrEmpty(region_LandingType.AsString()))
                    {
                        regionString_Landing = region_LandingType.AsString();
                    }
                    else
                    {
                        regionString_Landing = "XXX";
                    }

                    if (regionLevel_LandingType != null && !string.IsNullOrEmpty(regionLevel_LandingType.AsString()))
                    {
                        regionLevelString_Landing = regionLevel_LandingType.AsString();
                    }
                    else
                    {
                        regionLevelString_Landing = "XXX";
                    }

                    if (!string.IsNullOrEmpty(manufacturer_LandingType.AsString()))
                    {
                        manufacturerString_Landing = manufacturer_LandingType.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer_LandingMaterial.AsString()))
                    {
                        manufacturerString_Landing = manufacturer_LandingMaterial.AsString();

                        manufacturer_LandingType.Set(manufacturerString_Run);
                    }
                    else
                    {
                        manufacturerString_Landing = "XXX";
                    }

                    if (!string.IsNullOrEmpty(model_LandingType.AsString()))
                    {
                        modelString_Landing = model_LandingType.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model_LandingMaterial.AsString()))
                    {
                        modelString_Landing = model_LandingMaterial.AsString();

                        model_LandingType.Set(modelString_Run);
                    }
                    else
                    {
                        modelString_Landing = "XXX";
                    }

                    if (Guid.TryParse(mark_LandingMaterial.AsString(), out Guid result))
                    {
                        commentsString_Landing = mark_LandingMaterial.AsString() + "-" + "XXX" + "-" + density.ToString();

                        if (string.IsNullOrEmpty(description_LandingType.AsString()) && !string.IsNullOrEmpty(description_LandingMaterial.AsString()))
                        {
                            description_LandingType.Set(description_LandingMaterial.AsString());
                        }
                    }
                    else
                    {
                        var category_LandingMaterial =landingMaterial.MaterialClass;

                        commentsString_Landing = category_LandingMaterial + "-" + manufacturerString_Landing + "-" + modelString_Landing + "-" + yearString_Landing + "-" + regionLevelString_Landing + "-" + regionString_Landing + "-" + "XXX" + "-" + density.ToString();

                    }

                    mark_LandingType.Set(commentsString_Landing);

                    descriptionString_Landing = description_LandingType.AsString();

                }

                var year_StairType = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                var region_StairType = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                var regionLevel_StairType = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                var manufacturer_StairType = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                var model_StairType = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                var mark_StairType = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                var description_StairType = ele.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                if (landingMaterialID==runMaterialID)
                {
                    if(string.IsNullOrEmpty(model_StairType.AsString()) && !(modelString_Run=="XXX" && modelString_Landing=="XXX"))
                    {
                        if(modelString_Landing == modelString_Run)
                        {
                            model_StairType.Set(modelString_Run);
                        }
                        else
                        {
                            model_StairType.Set(modelString_Run+";"+modelString_Landing);
                        }
                       
                    }
                    if(string.IsNullOrEmpty(manufacturer_StairType.AsString()) && !(manufacturerString_Run == "XXX" && manufacturerString_Landing == "XXX"))
                    {
                        if (manufacturerString_Landing == manufacturerString_Run)
                        {
                            manufacturer_StairType.Set(manufacturerString_Run);

                        }
                        else
                        {
                            manufacturer_StairType.Set(manufacturerString_Run + ";" + manufacturerString_Landing);
                        }
                    }

                    if(year_StairType!=null && string.IsNullOrEmpty(year_StairType.AsString()) && !(yearString_Run == "XXX" && yearString_Landing=="XXX"))
                    {
                        if (yearString_Landing == yearString_Run )
                        {
                            year_StairType.Set(yearString_Run);

                        }
                        else
                        {
                            year_StairType.Set(yearString_Run + ";" + yearString_Landing);
                        }
                    }

                    if (region_StairType != null && string.IsNullOrEmpty(region_StairType.AsString()) &&!(regionString_Run == "XXX" &&regionString_Landing=="XXX"))
                    {
                        if (regionString_Landing == regionString_Run )
                        {
                            region_StairType.Set(regionString_Run);

                        }
                        else
                        {
                            region_StairType.Set(regionString_Run + ";" + regionString_Landing);
                        }
                    }

                    if (regionLevel_StairType != null && string.IsNullOrEmpty(regionLevel_StairType.AsString()) &&!(regionLevelString_Run == "XXX"&& regionLevelString_Landing=="XXX"))
                    {
                        if (regionLevelString_Landing == regionLevelString_Run)
                        {
                            regionLevel_StairType.Set(regionLevelString_Run);

                        }
                        else
                        {
                            regionLevel_StairType.Set(regionLevelString_Run + ";" + regionLevelString_Landing);
                        }
                    }

                    if(commentsString_Landing==commentsString_Run)
                    {
                        mark_StairType.Set(commentsString_Run);                        
                    }
                    else
                    {
                        mark_StairType.Set(commentsString_Run + ";" + commentsString_Landing);               
                    }

                    if(string.IsNullOrEmpty(description_StairType.AsString()) && !string.IsNullOrEmpty(descriptionString_Run) && !string.IsNullOrEmpty(descriptionString_Landing))
                    {
                        if(descriptionString_Landing==descriptionString_Run)
                        {
                            description_StairType.Set(descriptionString_Run);
                        }
                        else
                        {
                            description_StairType.Set(descriptionString_Run + ";" + descriptionString_Landing);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(model_StairType.AsString()) && !(modelString_Run == "XXX" && modelString_Landing == "XXX"))
                    {
                        model_StairType.Set(modelString_Run + ";" + modelString_Landing);
                    }

                    if (string.IsNullOrEmpty(manufacturer_StairType.AsString()) && !(manufacturerString_Run == "XXX" && manufacturerString_Landing == "XXX"))
                    {
                        manufacturer_StairType.Set(manufacturerString_Run + ";" + manufacturerString_Landing);
                    }

                    if (year_StairType != null && string.IsNullOrEmpty(year_StairType.AsString()) && !(yearString_Run == "XXX" && yearString_Landing == "XXX"))
                    {
                        year_StairType.Set(yearString_Run + ";" + yearString_Landing);
                    }

                    if (region_StairType != null && string.IsNullOrEmpty(region_StairType.AsString()) && !(regionString_Run == "XXX" && regionString_Landing == "XXX"))
                    {
                        region_StairType.Set(regionString_Run + ";" + regionString_Landing);
                    }

                    if (regionLevel_StairType != null && string.IsNullOrEmpty(regionLevel_StairType.AsString()) && !(regionLevelString_Run == "XXX" && regionLevelString_Landing == "XXX"))
                    {
                        regionLevel_StairType.Set(regionLevelString_Run + ";" + regionLevelString_Landing);
                    }

                    if (commentsString_Landing == commentsString_Run)
                    {
                        mark_StairType.Set(commentsString_Run + ";" + commentsString_Landing);
                    }

                    if (string.IsNullOrEmpty(description_StairType.AsString()) && !string.IsNullOrEmpty(descriptionString_Run) && !string.IsNullOrEmpty(descriptionString_Landing))
                    {
                        description_StairType.Set(descriptionString_Run + ";" + descriptionString_Landing);
                    }
                }
            }
        }//L

        private void SetRailing(Document doc)
        {
            FilteredElementCollector railingTypes = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(Railing))).WhereElementIsElementType();

            foreach (var ele in railingTypes)
            {
                RailingType railingType = ele as RailingType;

                if (railingType == null)
                {
                    continue;
                }

                TopRailType topRailType = doc.GetElement(railingType.TopRailType) as TopRailType;

                ElementId railingMaterialID = topRailType.LookupParameter("Material").AsElementId();

                Material railingMaterial = doc.GetElement(railingMaterialID) as Material;

                if (railingMaterial != null)
                {
                    var manufacturer = railingMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_railType = railingType.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_topRailType = topRailType.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = railingMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_railType = railingType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_topRailType = topRailType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = railingMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_railType = railingType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    Parameter mark_topRailType = topRailType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = railingMaterial.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_railType = railingType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_topRailType = topRailType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(railingMaterial.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_railType.AsString()))
                    {
                        manufacturer_string = manufacturer_railType.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer_topRailType.AsString()))
                    {
                        manufacturer_string = manufacturer_topRailType.AsString();

                        manufacturer_railType.Set(manufacturer_string);
                    }
                    else if(!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_railType.Set(manufacturer_string);

                        manufacturer_topRailType.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_railType.AsString()))
                    {
                        model_string = model_railType.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model_topRailType.AsString()))
                    {
                        model_string = model_topRailType.AsString();

                        model_railType.Set(model_string);
                    }
                    else if (!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string= model.AsString();

                        model_railType.Set(model_string);

                        model_topRailType.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }

                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_railType.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        mark_topRailType.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if (string.IsNullOrEmpty(description_railType.AsString()))
                        {
                            if (!string.IsNullOrEmpty(description_topRailType.AsString()))
                            {
                                description_railType.Set(description_topRailType.AsString());
                            }
                            else if (!string.IsNullOrEmpty(description.AsString()))
                            {
                                description_railType.Set(description.AsString());

                                description_topRailType.Set(description.AsString());
                            }                    
                        }
                    }
                    else
                    {
                        var category = railingMaterial.MaterialClass;

                        var year_Rail = railingType.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if(year_Rail!=null && !string.IsNullOrEmpty(year_Rail.AsString()))
                        {
                            year_string = year_Rail.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region_Rail = railingType.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if(region_Rail!=null && !string.IsNullOrEmpty(region_Rail.AsString()))
                        {
                            region_string = region_Rail.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel_Rail = railingType.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel_Rail != null && !string.IsNullOrEmpty(regionLevel_Rail.AsString()))
                        {
                            regionLevel_string = regionLevel_Rail.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_railType.Set(comments_string);

                        mark_topRailType.Set(comments_string);
                    }
                }
            }
        }//L
        
        private void SetWalls(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(WallType)));

            foreach (var ele in collector)
            {
                WallType wallType = ele as WallType;

                if(wallType==null)
                {
                    continue;
                }

                if (wallType.Kind == WallKind.Curtain)
                {
                    SetCurtainWalls(doc, wallType);
                }

                var structure=wallType.GetCompoundStructure();

                if(structure!=null)
                {
                    var layers = structure.GetLayers();

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    List<string> comments_Strings = new List<string>();

                    List<string> manufacturer_Strings= new List<string>();

                    List<string> model_Strings = new List<string>();

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]"); 

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    foreach (var layer in layers)
                    {
                        var material = doc.GetElement(layer.MaterialId) as Material;

                        if (material != null)
                        {
                            var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                            var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                            var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

      //                      var density= material.get_Parameter(BuiltInParameter.PHY_MATERIAL_PARAM_STRUCTURAL_DENSITY);

                            //StructuralAsset materialAssest= doc.GetElement(material.StructuralAssetId) as StructuralAsset;

                            PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                            double density = 0;

                            if(propertySetElement!=null)
                            {
                                StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                                density=structuralAsset.Density* 35.315;
                            }

                            string manufacturer_string = null;

                            if(!string.IsNullOrEmpty(manufacturer.AsString()))
                            {
                                manufacturer_string = manufacturer.AsString();
                            }
                            else
                            {
                                manufacturer_string = "XXX";
                            }

                            manufacturer_Strings.Add(manufacturer_string);

                            string model_string = null;

                            if(!string.IsNullOrEmpty(model.AsString()))
                            {
                                model_string = model.AsString();
                                
                            }
                            else
                            {
                                model_string = "XXX";
                                
                            }

                            model_Strings.Add(model_string);                         

                            if (Guid.TryParse(mark.AsString(), out Guid result))
                            {
                                comments_Strings.Add(mark.AsString() + "-" + (layer.Width * 304.8).ToString() + "-" + density.ToString());
                            }
                            else
                            {
                                var category = material.MaterialClass;

                                string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + (layer.Width * 304.8).ToString() + "-" + density.ToString();

                                comments_Strings.Add(comments_string);
                            }
                        }
                    }

                    string commentString_Type = string.Join(";", comments_Strings);

                    string manufacturerString_Type = string.Join(";", manufacturer_Strings.Distinct());

                    string modelString_Type = string.Join(";", model_Strings.Distinct());

                    if (string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_Type.Set(manufacturerString_Type);
                    }

                    if (string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_Type.Set(modelString_Type);
                    }
                    mark_Type.Set(commentString_Type);    
                }

            }
        }//L

        private void SetFloors(Document doc)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(FloorType)));

            foreach (var ele in collector)
            {
                FloorType floorType = ele as FloorType;

                if (floorType == null)
                {
                    continue;
                }

                var structure = floorType.GetCompoundStructure();

                if (structure != null)
                {
                    var layers = structure.GetLayers();

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    List<string> comments_Strings = new List<string>();

                    List<string> manufacturer_Strings = new List<string>();

                    List<string> model_Strings = new List<string>();

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    foreach (var layer in layers)
                    {
                        var material = doc.GetElement(layer.MaterialId) as Material;

                        if (material != null)
                        {
                            var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                            var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                            var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                            PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                            double density = 0;

                            if (propertySetElement != null)
                            {
                                StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                                density = structuralAsset.Density * 35.315;
                            }

                            string manufacturer_string = null;

                            if (!string.IsNullOrEmpty(manufacturer.AsString()))
                            {
                                manufacturer_string = manufacturer.AsString();
                            }
                            else
                            {
                                manufacturer_string = "XXX";
                            }

                            manufacturer_Strings.Add(manufacturer_string);

                            string model_string = null;

                            if (!string.IsNullOrEmpty(model.AsString()))
                            {
                                model_string = model.AsString();

                            }
                            else
                            {
                                model_string = "XXX";

                            }

                            model_Strings.Add(model_string);

                            if (Guid.TryParse(mark.AsString(), out Guid result))
                            {
                                comments_Strings.Add(mark.AsString() + "-" + (layer.Width * 304.8).ToString()+"-" + density.ToString());
                            }
                            else
                            {
                                var category = material.MaterialClass;

                                string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + (layer.Width * 304.8).ToString() + "-" + density.ToString();

                                comments_Strings.Add(comments_string);
                            }
                        }
                    }

                    string commentString_Type = string.Join(";", comments_Strings);

                    string manufacturerString_Type = string.Join(";", manufacturer_Strings.Distinct());

                    string modelString_Type = string.Join(";", model_Strings.Distinct());

                    if (string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_Type.Set(manufacturerString_Type);
                    }

                    if (string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_Type.Set(modelString_Type);
                    }
                    mark_Type.Set(commentString_Type);
                }
            }
        }//L

        private void SetCeilings(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(CeilingType)));

            foreach (var ele in collector)
            {
                CeilingType ceilingType = ele as CeilingType;

                if (ceilingType == null)
                {
                    continue;
                }

                var structure = ceilingType.GetCompoundStructure();

                if (structure != null)
                {
                    var layers = structure.GetLayers();

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    List<string> comments_Strings = new List<string>();

                    List<string> manufacturer_Strings = new List<string>();

                    List<string> model_Strings = new List<string>();

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string year_string = null;

                    if (year != null && !string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    foreach (var layer in layers)
                    {
                        var material = doc.GetElement(layer.MaterialId) as Material;

                        if (material != null)
                        {
                            var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                            var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                            var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                            PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                            double density = 0;

                            if (propertySetElement != null)
                            {
                                StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                                density = structuralAsset.Density * 35.315;
                            }

                            string manufacturer_string = null;

                            if (!string.IsNullOrEmpty(manufacturer.AsString()))
                            {
                                manufacturer_string = manufacturer.AsString();
                            }
                            else
                            {
                                manufacturer_string = "XXX";
                            }

                            manufacturer_Strings.Add(manufacturer_string);

                            string model_string = null;

                            if (!string.IsNullOrEmpty(model.AsString()))
                            {
                                model_string = model.AsString();

                            }
                            else
                            {
                                model_string = "XXX";

                            }

                            model_Strings.Add(model_string);

                            if (Guid.TryParse(mark.AsString(), out Guid result))
                            {
                                comments_Strings.Add(mark.AsString() + "-" + (layer.Width * 304.8).ToString() + "-" + density.ToString());
                            }
                            else
                            {
                                var category = material.MaterialClass;

                                string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + (layer.Width * 304.8).ToString() + "-" + density.ToString();

                                comments_Strings.Add(comments_string);
                            }
                        }
                    }

                    string commentString_Type = string.Join(";", comments_Strings);

                    string manufacturerString_Type = string.Join(";", manufacturer_Strings.Distinct());

                    string modelString_Type = string.Join(";", model_Strings.Distinct());

                    if (string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_Type.Set(manufacturerString_Type);
                    }

                    if (string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_Type.Set(modelString_Type);
                    }

                    mark_Type.Set(commentString_Type);
                }

            }
        }//L

        private void SetRoofs(Document doc)
        {

            FilteredElementCollector collector = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(RoofType)));

            foreach (var ele in collector)
            {
                RoofType roofType = ele as RoofType;

                if (roofType == null)
                {
                    continue;
                }

                var structure = roofType.GetCompoundStructure();

                if (structure != null)
                {
                    var layers = structure.GetLayers();

                    Parameter manufacturer_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter model_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter mark_Type = ele.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    List<string> comments_Strings = new List<string>();

                    List<string> manufacturer_Strings = new List<string>();

                    List<string> model_Strings = new List<string>();

                    var year = ele.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                    var region = ele.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                    var regionLevel = ele.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                    string year_string = null;

                    if (year != null &&!string.IsNullOrEmpty(year.AsString()))
                    {
                        year_string = year.AsString();
                    }
                    else
                    {
                        year_string = "XXX";
                    }

                    string region_string = null;

                    if (region != null && !string.IsNullOrEmpty(region.AsString()))
                    {
                        region_string = region.AsString();
                    }
                    else
                    {
                        region_string = "XXX";
                    }

                    string regionLevel_string = null;

                    if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                    {
                        regionLevel_string = regionLevel.AsString();
                    }
                    else
                    {
                        regionLevel_string = "XXX";
                    }

                    foreach (var layer in layers)
                    {
                        var material = doc.GetElement(layer.MaterialId) as Material;

                        if (material != null)
                        {
                            var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                            var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                            var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                            PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                            double density = 0;

                            if (propertySetElement != null)
                            {
                                StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                                density = structuralAsset.Density * 35.315;
                            }

                            string manufacturer_string = null;

                            if (!string.IsNullOrEmpty(manufacturer.AsString()))
                            {
                                manufacturer_string = manufacturer.AsString();
                            }
                            else
                            {
                                manufacturer_string = "XXX";
                            }

                            manufacturer_Strings.Add(manufacturer_string);

                            string model_string = null;

                            if (!string.IsNullOrEmpty(model.AsString()))
                            {
                                model_string = model.AsString();

                            }
                            else
                            {
                                model_string = "XXX";

                            }

                            model_Strings.Add(model_string);

                            if (Guid.TryParse(mark.AsString(), out Guid result))
                            {
                                comments_Strings.Add(mark.AsString() + "-" + (layer.Width*304.8).ToString() + "-" + density.ToString());
                            }
                            else
                            {
                                var category = material.MaterialClass;

                                string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + (layer.Width * 304.8).ToString() + "-" + density.ToString();

                                comments_Strings.Add(comments_string);
                            }
                        }
                    }

                    string commentString_Type = string.Join(";", comments_Strings);

                    string manufacturerString_Type = string.Join(";", manufacturer_Strings.Distinct());

                    string modelString_Type = string.Join(";", model_Strings.Distinct());

                    if (string.IsNullOrEmpty(manufacturer_Type.AsString()))
                    {
                        manufacturer_Type.Set(manufacturerString_Type);
                    }

                    if (string.IsNullOrEmpty(model_Type.AsString()))
                    {
                        model_Type.Set(modelString_Type);
                    }

                    mark_Type.Set(commentString_Type);
                }
            }
        }//L

        private void SetCurtainWalls(Document doc, WallType wallType)
        {
            Parameter panel = wallType.LookupParameter("Curtain Panel");
            
            ElementId panelID = panel.AsElementId();

            PanelType panelType = doc.GetElement(panelID) as PanelType;

            if (panelType != null)
            {
                ElementId materialID = panelType.LookupParameter("Material").AsElementId();

                Material material = doc.GetElement(materialID) as Material;

                if(material != null)
                {
                    var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_WallType = wallType.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    Parameter manufacturer_PanelType= panelType.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                    var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_WallType = wallType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    Parameter model_PanelType = panelType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                    var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                    Parameter mark_WallType = wallType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    Parameter mark_PanelType = panelType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                    var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_WallType = wallType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    Parameter description_PanelType = panelType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                    PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                    double density = 0;

                    if (propertySetElement != null)
                    {
                        StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                        density = structuralAsset.Density * 35.315;
                    }

                    string manufacturer_string = null;

                    if (!string.IsNullOrEmpty(manufacturer_WallType.AsString()))
                    {
                        manufacturer_string = manufacturer_WallType.AsString();
                    }
                    else if (!string.IsNullOrEmpty(manufacturer_PanelType.AsString()))
                    {
                        manufacturer_string = manufacturer_PanelType.AsString();

                        manufacturer_WallType.Set(manufacturer_string);
                    }
                    else if (!string.IsNullOrEmpty(manufacturer.AsString()))
                    {
                        manufacturer_string = manufacturer.AsString();

                        manufacturer_WallType.Set(manufacturer_string);

                        manufacturer_PanelType.Set(manufacturer_string);
                    }
                    else
                    {
                        manufacturer_string = "XXX";
                    }

                    string model_string = null;

                    if (!string.IsNullOrEmpty(model_WallType.AsString()))
                    {
                        model_string = model_WallType.AsString();
                    }
                    else if (!string.IsNullOrEmpty(model_PanelType.AsString()))
                    {
                        model_string = model_PanelType.AsString();

                        model_WallType.Set(model_string);
                    }
                    else if (!string.IsNullOrEmpty(model.AsString()))
                    {
                        model_string = model.AsString();

                        model_WallType.Set(model_string);

                        model_PanelType.Set(model_string);
                    }
                    else
                    {
                        model_string = "XXX";
                    }

                    if (Guid.TryParse(mark.AsString(), out Guid result))
                    {
                        mark_WallType.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        mark_PanelType.Set(mark.AsString() + "-" + "XXX" + "-" + density.ToString());

                        if (description_WallType.AsString() == "")
                        {
                            if (!string.IsNullOrEmpty(description_PanelType.AsString()))
                            {
                                description_WallType.Set(description_PanelType.AsString());
                            }
                            else if (!string.IsNullOrEmpty(description.AsString()))
                            {
                                description_WallType.Set(description.AsString());

                                description_PanelType.Set(description.AsString());
                            }
                        }
                    }
                    else
                    {
                        var category = material.MaterialClass;

                        var year_Wall = wallType.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                        string year_string = null;

                        if (year_Wall != null && !string.IsNullOrEmpty(year_Wall.AsString()))
                        {
                            year_string = year_Wall.AsString();
                        }
                        else
                        {
                            year_string = "XXX";
                        }

                        var region_Wall = wallType.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                        string region_string = null;

                        if (region_Wall != null && !string.IsNullOrEmpty(region_Wall.AsString()))
                        {
                            region_string = region_Wall.AsString();
                        }
                        else
                        {
                            region_string = "XXX";
                        }

                        var regionLevel_Wall = wallType.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                        string regionLevel_string = null;

                        if (regionLevel_Wall != null && !string.IsNullOrEmpty(regionLevel_Wall.AsString()))
                        {
                            regionLevel_string = regionLevel_Wall.AsString();
                        }
                        else
                        {
                            regionLevel_string = "XXX";
                        }

                        string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                        mark_WallType.Set(comments_string);

                        mark_PanelType.Set(comments_string);
                    }
                }
            }
        }//L

        private void SetCurtainPanels(Document doc)
        {
            var collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_CurtainWallPanels)).WhereElementIsElementType();

            foreach (var panel in collector)
            {
                PanelType panelType=panel as PanelType;

                if (panelType != null)
                {
                    ElementId materialID = panelType.LookupParameter("Material").AsElementId();

                    Material material = doc.GetElement(materialID) as Material;

                    if (material != null)
                    {
                        var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        Parameter manufacturer_PanelType = panelType.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        Parameter model_PanelType = panelType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                        Parameter mark_PanelType = panelType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                        var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        Parameter description_PanelType = panelType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                        double density = 0;

                        if (propertySetElement != null)
                        {
                            StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                            density = structuralAsset.Density * 35.315;
                        }

                        string manufacturer_string = null;

                        if (!string.IsNullOrEmpty(manufacturer_PanelType.AsString()))
                        {
                            manufacturer_string = manufacturer_PanelType.AsString();

                        }
                        else if (!string.IsNullOrEmpty(manufacturer.AsString()))
                        {
                            manufacturer_string = manufacturer.AsString();

                            manufacturer_PanelType.Set(manufacturer_string);
                        }
                        else
                        {
                            manufacturer_string = "XXX";
                        }

                        string model_string = null;

                        if (!string.IsNullOrEmpty(model_PanelType.AsString()))
                        {
                            model_string = model_PanelType.AsString();
                        }
                        else if (!string.IsNullOrEmpty(model.AsString()))
                        {
                            model_string = model.AsString();

                            model_PanelType.Set(model_string);
                        }
                        else
                        {
                            model_string = "XXX";
                        }

                        var thickness=panelType.get_Parameter(BuiltInParameter.CURTAIN_WALL_SYSPANEL_THICKNESS);

                        string thickness_string = null;

                        if(!string.IsNullOrEmpty(thickness.AsValueString()))
                        {
                            thickness_string = thickness.AsValueString();
                        }
                        else
                        {
                            thickness_string = "XXX";
                        }

                        if (Guid.TryParse(mark.AsString(), out Guid result))
                        {    
                            mark_PanelType.Set(mark.AsString() + "-" + thickness_string + "-" + density.ToString());

                            if (!string.IsNullOrEmpty(description.AsString()))
                            {
                                description_PanelType.Set(description.AsString());
                            }                           
                        }
                        else
                        {
                            var category = material.MaterialClass;
                         
                            string year_string = null;

                            var year= panelType.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                            if(year!=null && !string.IsNullOrEmpty(year.AsString()))
                            {
                                year_string = year.AsString();
                            }
                            else
                            {
                                year_string = "XXX";
                            }

                            var region = panelType.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                            string region_string = null;

                            if (region != null && !string.IsNullOrEmpty(region.AsString()))
                            {
                                region_string = region.AsString();
                            }
                            else
                            {
                                region_string = "XXX";
                            }

                            var regionLevel = panelType.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                            string regionLevel_string = null;

                            if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                            {
                                regionLevel_string = regionLevel.AsString();
                            }
                            else
                            {
                                regionLevel_string = "XXX";
                            }

                            string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                            mark_PanelType.Set(comments_string);
                        }
                    }
                }
            }
           
        }//L

        private void SetStructuralConnections(Document doc)
        {
            var collector = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_StructConnections)).WhereElementIsElementType();

            foreach(var connection in collector)
            {
                FamilySymbol connectionType = connection as FamilySymbol;

                if (connectionType != null)
                {
                    var materialPara = connectionType.LookupParameter("Material");
                    
                    ElementId materialID=materialPara.AsElementId();

                    Material material = doc.GetElement(materialID) as Material;

                    if (material != null)
                    {
                        var manufacturer = material.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        Parameter manufacturer_PanelType = connectionType.get_Parameter(BuiltInParameter.ALL_MODEL_MANUFACTURER);

                        var model = material.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        Parameter model_PanelType = connectionType.get_Parameter(BuiltInParameter.ALL_MODEL_MODEL);

                        var mark = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);

                        Parameter mark_PanelType = connectionType.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                        var description = material.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        Parameter description_PanelType = connectionType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION);

                        PropertySetElement propertySetElement = doc.GetElement(material.StructuralAssetId) as PropertySetElement;

                        double density = 0;

                        if (propertySetElement != null)
                        {
                            StructuralAsset structuralAsset = propertySetElement.GetStructuralAsset();

                            density = structuralAsset.Density * 35.315;
                        }

                        string manufacturer_string = null;

                        if (!string.IsNullOrEmpty(manufacturer_PanelType.AsString()))
                        {
                            manufacturer_string = manufacturer_PanelType.AsString();

                        }
                        else if (!string.IsNullOrEmpty(manufacturer.AsString()))
                        {
                            manufacturer_string = manufacturer.AsString();

                            manufacturer_PanelType.Set(manufacturer_string);
                        }
                        else
                        {
                            manufacturer_string = "XXX";
                        }

                        string model_string = null;

                        if (!string.IsNullOrEmpty(model_PanelType.AsString()))
                        {
                            model_string = model_PanelType.AsString();
                        }
                        else if (!string.IsNullOrEmpty(model.AsString()))
                        {
                            model_string = model.AsString();

                            model_PanelType.Set(model_string);
                        }
                        else
                        {
                            model_string = "XXX";
                        }

                        var thickness = connectionType.get_Parameter(BuiltInParameter.CURTAIN_WALL_SYSPANEL_THICKNESS);

                        string thickness_string = null;

                        if (!string.IsNullOrEmpty(thickness.AsValueString()))
                        {
                            thickness_string = thickness.AsValueString();
                        }
                        else
                        {
                            thickness_string = "XXX";
                        }

                        if (Guid.TryParse(mark.AsString(), out Guid result))
                        {
                            mark_PanelType.Set(mark.AsString() + "-" + thickness_string + "-" + density.ToString());

                            if (!string.IsNullOrEmpty(description.AsString()))
                            {
                                description_PanelType.Set(description.AsString());
                            }
                        }
                        else
                        {
                            var category = material.MaterialClass;

                            string year_string = null;

                            var year = connectionType.LookupParameter("Pset_ManufacturerTypeInformation.ProductionYear[Type]");

                            if (year != null && !string.IsNullOrEmpty(year.AsString()))
                            {
                                year_string = year.AsString();
                            }
                            else
                            {
                                year_string = "XXX";
                            }

                            var region = connectionType.LookupParameter("Cpset_MaterialInformation.Region[Type]");

                            string region_string = null;

                            if (region != null && !string.IsNullOrEmpty(region.AsString()))
                            {
                                region_string = region.AsString();
                            }
                            else
                            {
                                region_string = "XXX";
                            }

                            var regionLevel = connectionType.LookupParameter("Cpset_MaterialInformation.RegionLevel[Type]");

                            string regionLevel_string = null;

                            if (regionLevel != null && !string.IsNullOrEmpty(regionLevel.AsString()))
                            {
                                regionLevel_string = regionLevel.AsString();
                            }
                            else
                            {
                                regionLevel_string = "XXX";
                            }

                            string comments_string = category + "-" + manufacturer_string + "-" + model_string + "-" + year_string + "-" + regionLevel_string + "-" + region_string + "-" + "XXX" + "-" + density.ToString();

                            mark_PanelType.Set(comments_string);
                        }
                    }
                }
            }
        }
    }
}
