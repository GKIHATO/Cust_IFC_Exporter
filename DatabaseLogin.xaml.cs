using Autodesk.Revit.UI;
using Autodesk.UI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
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
    /// Interaction logic for DatabaseLogin.xaml
    /// </summary>
    public partial class DatabaseLogin : ChildWindow
    {
        public SQLDBConnect connect = new SQLDBConnect();

        static private string _lastServerName = "";

        static private string _lastDatabaseName = "";

        static private string _lastUserName = "New User";

        static private string _lastPassword = "12345678";

        static private bool _rememberMe = false;

        public DatabaseLogin()
        {
            InitializeComponent();

            if(_rememberMe)
            {
                sqlServerName.Text=_lastServerName;

                databaseName.Text = _lastDatabaseName;

                userName.Text = _lastUserName;

                password.Password = _lastPassword;

                rememberMe.IsChecked = true;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sqlServerName.Text))
            {
                TaskDialog newDialog = new TaskDialog("Login Failed");

                newDialog.MainContent = "Please input a Server Name!";

                newDialog.Show();
            }
            else if (string.IsNullOrWhiteSpace(databaseName.Text))
            {
                TaskDialog newDialog = new TaskDialog("Login Failed");

                newDialog.MainContent = "Please input a User Name!";

                newDialog.Show();
            }
            else if (string.IsNullOrWhiteSpace(userName.Text))
            {
                TaskDialog newDialog = new TaskDialog("Login Failed");

                newDialog.MainContent = "Please input a User Name!";

                newDialog.Show();
            }
            else if (string.IsNullOrWhiteSpace(password.Password))
            {
                TaskDialog newDialog = new TaskDialog("Login Failed");

                newDialog.MainContent = "Please input the Password!";

                newDialog.Show();
            }
            else
            {
                if(rememberMe.IsChecked==true)
                {
                    _lastServerName = sqlServerName.Text;

                    _lastDatabaseName = databaseName.Text;

                    _lastUserName = userName.Text;

                    _lastPassword = password.Password;
                    
                    _rememberMe = true;
                }

                string connectionString = $"Data Source={sqlServerName.Text};Initial Catalog={databaseName.Text};User Id={userName.Text};Password={password.Password};";

                if (connect.ConnectDB(connectionString))
                {
                    DialogResult = true;
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
