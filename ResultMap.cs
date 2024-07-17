using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cust_IFC_Exporter
{
    internal class ResultMap : ClassMap<ValidateResult>
    {
        public ResultMap()
        {
            Map(m => m.IfcLabel).Name("IfcLabel");
            Map(m => m.IfcType).Name("IfcType");
            Map(m => m.Ifc_GUID).Name("Ifc_GUID");
            Map(m => m.ConceptRootName).Name("ConceptRootName");
            Map(m => m.ConceptName).Name("ConceptName");
            Map(m => m.Result).Name("Result");
            Map(m => m.Parameters).Name("Parameters");

        }
    }
}
