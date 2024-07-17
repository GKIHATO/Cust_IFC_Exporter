using Autodesk.UI.Windows;
using System.Windows;
using System.ComponentModel;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using Xbim.MvdXml.DataManagement;
using System.Windows.Forms;
using System;
using Xbim.MvdXml;
using Xbim.Ifc;
using Xbim.Common;
using System.Linq;
using Xbim.Ifc2x3.CostResource;
using Xbim.Common.Metadata;
using System.IO;
using Xbim.Ifc4.Kernel;
using System.Threading;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for ValidateStatusBar.xaml
    /// </summary>
    public partial class ValidateStatusBar : ChildWindow
    {

        #region Attributes

        int currentNum = 0; 

        int totalNum = 0;

        int currentFileNum = 0;

        int totalFileNum = 0;

        List<string> _docsIsexported;

        string _er_Name = null;

        string _mvdFilePath = null;

        string _fileSavePath = null;

        BackgroundWorker worker;

        MvdEngine mvdEngine;

        List<ResultClasss> result;

        public List<string> csvFileNames;

        mvdXML mvdFile;
        
        #endregion Attribute

        #region Methods
        public ValidateStatusBar(List<string> docsIsExported,string ER_Name,string mvdFilePath,string fileSavePath)
        {
            InitializeComponent();

            this.totalFileNum = docsIsExported.Count;

            _docsIsexported = docsIsExported;

            _er_Name = ER_Name;

            _mvdFilePath = mvdFilePath;

            _fileSavePath = fileSavePath;

            csvFileNames = new List<string>();

            horizontalStackPanel.Visibility = System.Windows.Visibility.Visible;

            worker=new BackgroundWorker();

            worker.DoWork += Worker_DoWork;

            worker.ProgressChanged += Worker_ProgressChanged;

            worker.WorkerReportsProgress = true;

            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            worker.RunWorkerAsync();

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        { 
            if(statusText_Single.Visibility==System.Windows.Visibility.Collapsed)
            {
                statusText_Single.Visibility = System.Windows.Visibility.Visible;
            }
            if(totalNum>0)
            {
                double singleNum= (double)currentNum / totalNum * 100;

                double fileNum = 0;

                ValidatingProgress_Single.Value = singleNum;

                progressNum_Single.Text = $"{singleNum.ToString("0.00")}%";

                statusText_Single.Content = $"Validating {currentNum} of {totalNum} IFC instances";

                if(totalFileNum==1)
                {
                    fileNum = singleNum;
                }
                else
                {
                    fileNum = (double)currentFileNum / totalFileNum * 100;

                }

                ValidatingProgress_Total.Value = fileNum;

                progressNum_Total.Text = $"{fileNum.ToString("0.00")}%";

                statusText_Total.Content = $"Validating {currentFileNum} of {totalFileNum} File(s)";

            }
            else
            {
                System.Windows.MessageBox.Show("Empty File!");
            }
            
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            mvdFile = null;
            try
            {
                var comp = mvdXML.TestCompatibility(_mvdFilePath);
                if (comp == mvdXML.CompatibilityResult.InvalidNameSpace)
                {
                    var newName = Path.GetTempFileName();
                    if (mvdXML.FixNamespace(_mvdFilePath, newName))
                    {
                        mvdFile = mvdXML.LoadFromFile(newName);
                    }
                    else
                    {
                        var msg = $"Attempt to fix namespace in invalid xml file [{_mvdFilePath}] failed.";
                        System.Windows.Forms.MessageBox.Show(msg);
                    }
                }
                else
                {
                    mvdFile = mvdXML.LoadFromFile(_mvdFilePath);
                }

            }
            catch
            {
                var msg = $"Invalid xml file [{_mvdFilePath}].";
                System.Windows.MessageBox.Show(msg);
            }


            foreach (string doc in _docsIsexported)
            {
                currentNum++;

                LoadIntoMvdEngine(doc);
            } 

        }


        
    

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusText_Single.Content = "Validate Complete!";

            statusText_Total.Content = $"Total of {totalFileNum} file(s) are validated, proceed to see the result.";

            horizontalStackPanel.Visibility=System.Windows.Visibility.Visible;
        }


        private void LoadIntoMvdEngine(string doc)
        {

            currentNum = 0;

            currentFileNum++;

            mvdEngine = null;

            var model = IfcStore.Open(doc);

            mvdEngine =new MvdEngine(mvdFile, model);

            ModelViewExchangeRequirement er;

            er = mvdFile.Views[0].ExchangeRequirements.Where(e => e.name == _er_Name).FirstOrDefault();

            List<IPersistEntity> entities= model?.Instances?.ToList(); //Get all instances in the IFC file

            totalNum = entities.Count;

            List<ExpressType> list = mvdEngine.GetExpressTypes();

            var selectedIfcClasses=new HashSet<ExpressType>(list);

            result = new List<ResultClasss>();

            foreach (var entity in entities)
            {

                currentNum++;

                //Thread.Sleep(100);

                worker?.ReportProgress(0);

                ExpressType entityType = entity.ExpressType;

                if (selectedIfcClasses.Any())
                {
                    // if no one of the selected classes contains the element type in the subtypes skip entity
                    var needTest =
                        selectedIfcClasses.Any(
                            classesToTest =>
                                classesToTest == entityType ||
                                classesToTest.NonAbstractSubTypes.Contains(entityType));
                    if (!needTest)
                        continue;
                }

                var suitableRoots = mvdEngine.GetConceptRoots(entityType);

                foreach (ConceptRoot suitableRoot in suitableRoots)
                {
                    if (suitableRoot.Concepts == null|| !suitableRoot.AppliesTo(entity))
                        continue;

                    foreach (Concept concept in suitableRoot.Concepts)
                    { 
                        if (concept.Requirements == null)
                            continue;

                        var requirementsRequirement= concept.Requirements.Where(e => e.exchangeRequirement == er.uuid).FirstOrDefault();

                        if (requirementsRequirement == null)
                            continue;

                        ConceptTestResult testResult = requirementsRequirement.Test(entity);

                        string failedRules = null;

                        if (testResult == ConceptTestResult.Fail || testResult == ConceptTestResult.Warning)
                        {
                            failedRules = requirementsRequirement.ParentConcept.FailedTemplateRuleJoined;
                        }
                        else
                        {
                            failedRules="All Success!";
                        }

                        ResultClasss newresult = new ResultClasss(concept, entity, testResult,failedRules,suitableRoot.name);

                        result.Add(newresult);

                    }

                }
            }

            string fileNameWithoutExtension = $"Validate Result_{Path.GetFileNameWithoutExtension(doc)}";

            string csvPath = _fileSavePath + "\\" + fileNameWithoutExtension + ".csv";

            int count = 0;

            //check if file exist, if it does, rename the export file
            while(true)
            {
                if(File.Exists(csvPath))
                {
                    count++;
                    csvPath = _fileSavePath + "\\" + fileNameWithoutExtension + $"({count}).csv";
                }
                else
                { break; }
            }

            csvFileNames.Add(csvPath);

            using (StreamWriter writer = new StreamWriter(csvPath))
            {
                string[] header = { "IfcLabel", "IfcType", "Ifc_GUID", "ConceptRootName", "ConceptName", "Result", "Parameters" };

                writer.WriteLine(string.Join(",",header));

                foreach (var line in result)
                {
                    IfcRoot tp = line.entity as IfcRoot;

                    string id = tp.GlobalId;

                    string[] message = { line.entity.EntityLabel.ToString(), line.entity.ExpressType.ToString(),id, line.ConceptRootName, line.Concept.name,line.Results.ToString(),line.failedTemplateRules};
                
                    writer.WriteLine(string.Join(",", message));                 
                }

                writer.Close();
            }

        }

        #endregion Methods

        #region EventsHandlers

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        #endregion EventsHandlers
    }
}