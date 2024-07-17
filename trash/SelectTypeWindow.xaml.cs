using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for SelectTypeWindow.xaml
    /// </summary>
    public partial class SelectTypeWindow : ChildWindow
    {

        List<Filter> items;
        public SelectTypeWindow(List<Filter> allItem)
        {
            InitializeComponent();
            
            itemForSelection.ItemsSource = allItem;

            items = allItem;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult= true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() != "Select All")
            {
                for (int i = 1; i < items.Count(); i++)
                {
                    if (items[i].IsSelected == false)
                    {
                        return;
                    }
                }

                items[0].IsSelected = true;
            }
            else
            {
                for (int i = 1; i < items.Count(); i++)
                {
                    items[i].IsSelected = true;
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox.Content.ToString() == "Select All")
            {
                for (int i = 1; i < items.Count; i++)
                {
                    if (items[i].IsSelected == false)
                    {
                        return;
                    }
                }

                for (int i = 1; i < items.Count; i++)
                {
                    items[i].IsSelected = false;
                }
            }
            else
            {
                items[0].IsSelected = false;
            }
        }
    }
}
