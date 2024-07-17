using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SelectTreeWindow.xaml
    /// </summary>
    public partial class SelectTreeWindow : ChildWindow
    {

        //public ObservableCollection<TreeNode<string>> items;
        public SelectTreeWindow(ObservableCollection<TreeNode<string>> allItem)
        {
            InitializeComponent();

            itemForSelection.ItemsSource = allItem;

            //items = allItem;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult= true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

/*        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            if(checkBox.IsChecked !=false)
            {
                checkBox.IsChecked = true;
            }
            else if(checkBox.IsChecked !=true)
            {
                checkBox.IsChecked = false;
            }
        }*/
    }
}
