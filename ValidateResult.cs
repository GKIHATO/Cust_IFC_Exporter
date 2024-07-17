using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace Cust_IFC_Exporter
{
    public class ValidateResult:INotifyPropertyChanged
    {
        public string Id 
        { get
            {
                return $"#{IfcLabel}__{IfcType}({Ifc_GUID})";
            } 
        }
        [Name("IfcLabel")]
        public string IfcLabel { get; set; }
        [Name("IfcType")]
        public string IfcType { get; set; }
        [Name("Ifc_GUID")]
        public string Ifc_GUID { get; set; }

        [Name("ConceptRootName")]
        public string ConceptRootName { get; set; }
        [Name("ConceptName")]
        public string ConceptName { get; set; }
        [Name("Result")]
        public string Result { get; set;}
        [Name("Parameters")]
        public string Parameters { get; set; }

        public bool Hide { get { return _hide; } set { _hide = value; OnPropertyChanged(nameof(Hide)); } }

        public SolidColorBrush Background { get { return _background; } set { _background = value; OnPropertyChanged(nameof(Background)); } }

        private SolidColorBrush _background=null;

        private bool _hide=true; //True to show, false to hide

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}