using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Cust_IFC_Exporter
{
    public class Filter:INotifyPropertyChanged
    {
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool _isSelected;

        private string _name;

        public Filter(string name, bool isSelected)
        {
            Name= name;
            IsSelected= isSelected;
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }
}
