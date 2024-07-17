using System.Collections.Generic;
using System.Windows.Documents;
using Xbim.Common;
using Xbim.MvdXml;
using Xbim.MvdXml.DataManagement;

namespace Cust_IFC_Exporter
{
    internal class ResultClasss
    {
        public Concept Concept { get; set; }

        public string ConceptRootName { get; set; }

        public ConceptTestResult Results { get; set; }

        public IPersistEntity entity { get; set; }

        public string failedTemplateRules;

        public ResultClasss(Concept concept, IPersistEntity entity, ConceptTestResult testResult,string failedRules, string conceptRootName)
        {

            this.Concept = concept;
            this.Results = testResult;
            this.entity = entity;
            failedTemplateRules = failedRules;
            ConceptRootName = conceptRootName;
        }


    }
}