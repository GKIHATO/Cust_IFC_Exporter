using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Cust_IFC_Exporter
{
    public class ValidateResultsByElement: INotifyPropertyChanged
    {
        public string Name {  get; set; }

        public List<ValidateResult> Results { get; set; }

        public bool Hide { get { return _hide; } set { _hide = value; OnPropertyChanged(nameof(Hide)); } }

        public SolidColorBrush Background { get { return _background; } set { _background = value; OnPropertyChanged(nameof(Background)); } }

        private SolidColorBrush _background = null;

        private bool _hide = true; //True to show, false to hide

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
