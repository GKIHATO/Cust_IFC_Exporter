using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for IFCExport_SelectFile.xaml
    /// </summary>
    public partial class IFCExport_SelectFile :ChildWindow
    {

        private IList<Document> m_OrderedDocuments = null;

        private IList<Document> m_DocumentsToExport = null;

        public IList<Document> OrderedDocuments
        {
            get
            {
                if (m_OrderedDocuments == null)
                    m_OrderedDocuments = new List<Document>();

                return m_OrderedDocuments;
            }
            set { m_OrderedDocuments = value; }
        }
        public IList<Document> DocumentsToExport
        {
            get
            {
                if (m_DocumentsToExport == null)
                    m_DocumentsToExport = new List<Document>();
                return m_DocumentsToExport;
            }
            set { m_DocumentsToExport = value; }
        }
        public IFCExport_SelectFile (UIApplication uiapp)
        {
            InitializeComponent();
            updateListView(uiapp);
        }

        private void updateListView(UIApplication uiapp)
        {
            DocumentSet docSet = uiapp.Application.Documents;

            Document activeDocument = uiapp.ActiveUIDocument.Document;
            List<CheckBox> checkBoxes = new List<CheckBox>();
            int exportDocumentCount = 0;

            OrderedDocuments =null;
            foreach (Document doc in docSet)
            {
                if (CanExportDocument(doc))
                {
                    // Count the number of Documents which can be exported
                    exportDocumentCount++;
                }
            }

            foreach (Document doc in docSet)
            {
                if (CanExportDocument(doc))
                {
                    CheckBox cb = createCheckBoxForDocument(doc, OrderedDocuments.Count);

                    // Add the active document as the top item.
                    if (doc.Equals(activeDocument))
                    {
                        // This should only be hit once
                        cb.IsChecked = true;
                        checkBoxes.Insert(0, cb);

                        if (exportDocumentCount == 1)
                        {
                            // If a single project is to be exported, make it read only
                            cb.IsEnabled = false;
                        }
                        OrderedDocuments.Insert(0, doc);
                    }
                    else
                    {
                        checkBoxes.Add(cb);
                        OrderedDocuments.Add(doc);
                    }

                }
            }

            this.listView.ItemsSource = checkBoxes;
        }

        private bool CanExportDocument(Document doc)
        {
            return (doc != null && !doc.IsFamilyDocument && !doc.IsLinked);
        }

        private CheckBox createCheckBoxForDocument(Document doc, int id)
        {
            CheckBox cb = new CheckBox();

            cb.Content = doc.Title;
            if (!String.IsNullOrEmpty(doc.PathName))
            {
                // If the user saves the file, the path where the document is saved is displayed
                // with a ToolTip, else it displays a message that the file is not saved.
                cb.ToolTip = doc.PathName;
            }
            else
            {
                cb.ToolTip = "File is Invalid or Empty";
            }
            ToolTipService.SetShowOnDisabled(cb, true);
            cb.SetValue(AutomationProperties.AutomationIdProperty, "projectToExportCheckBox" + id);
            return cb;
        }

        private int GetDocumentExportCount()
        {
            DocumentsToExport = null;
            List<CheckBox> cbList = this.listView.Items.Cast<CheckBox>().ToList();
            int count = 0;
            int docToExport = 0;
            foreach (CheckBox cb in cbList)
            {
                if ((bool)cb.IsChecked)
                {
                    DocumentsToExport.Add(OrderedDocuments[count]);
                    docToExport++;
                }
                count++;
            }

            return docToExport;
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {

            IFCExport_MainWindow.docsExportCount = GetDocumentExportCount();

            if (DocumentsToExport.Count()==0)
            {
                DialogResult = null;

            }
            else
            {
                DialogResult = true;

                IFCExport_MainWindow.documentsToExport = DocumentsToExport;
            }

        }

        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
