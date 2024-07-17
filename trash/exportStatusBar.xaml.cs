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
using System.ComponentModel;
using Autodesk.Revit.DB.ExternalService;
using Revit.IFC.Export.Exporter;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using System.IO.Pipes;
using System.IO;

namespace Cust_IFC_Exporter
{
    /// <summary>
    /// Interaction logic for exportStatusBar.xaml
    /// </summary>
    public partial class exportStatusBar : ChildWindow, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        /*        private DispatcherTimer timer;

                private static readonly object lockObject_1 = new object();

                private static readonly object lockObject_2 = new object();

                private static readonly object lockObject_3 = new object();*/

        int currentNum = 0;

        int totalNum = 0;

        int currentFileNum = 0;

        //public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
        /*
                public static AutoResetEvent AutoResetEvent1 = new AutoResetEvent(false);

                public static AutoResetEvent AutoResetEvent2 = new AutoResetEvent(false);*/

        int totalFileNum = 0;

        NamedPipeServerStream serverPipe_1;

        NamedPipeServerStream serverPipe_2;

        public void InitializeServer()
        {
            Task.Run(async () =>
            {
                serverPipe_1 = new NamedPipeServerStream("ReadFile", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                serverPipe_2 = new NamedPipeServerStream("ReadEle", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                // Asynchronously wait for a client to connect
                await serverPipe_1.WaitForConnectionAsync();           

                do
                {
                    await ProcessClientDataAsync(false,serverPipe_1);

                    await serverPipe_2.WaitForConnectionAsync();

                    do
                    {
                        await ProcessClientDataAsync(true, serverPipe_2);

                    } while (currentNum < totalNum);

                }while (currentFileNum < totalFileNum);

                
            });
        }

        private async Task ProcessClientDataAsync(bool eleOrFile, NamedPipeServerStream serverPipe)
        {
            try
            {
                using (var reader = new StreamReader(serverPipe))
                {
                    string data = null;
                    if (eleOrFile)
                    {

                        data = await reader.ReadLineAsync();

                        int.TryParse(data, out int intValue);

                        currentNum = intValue;

                        data = await reader.ReadLineAsync();

                        int.TryParse(data, out int intTotalValue);

                        totalNum = intTotalValue;

                        OnPropertyChanged();
                    }
                    else
                    {
                        int.TryParse(data, out int intFileValue);

                        currentFileNum = intFileValue;
                    }
                        

                        // Handle the received data (e.g., display it in TextBox1)
                        //textBox1.Invoke(new Action(() => TextBox1.AppendText(data + Environment.NewLine)));
                    
                }
            }
            catch (IOException)
            {
                // Handle exceptions (e.g., client disconnected)
            }
        }

        /*        public void updateInformation()
                {
                    do
                    {
                        currentNum++;

                        AutoResetEvent.WaitOne();

                        OnPropertyChanged();

                        if(currentFileNum == totalFileNum && currentFileNum==totalFileNum)
                        {
                            break;
                        }
                    } while (true);
                }*/

        /*        public void updateInformation1()
                {
                    do
                    {
                        currentNum++;

                        AutoResetEvent1.WaitOne();

                        if (currentFileNum == totalFileNum)
                        {
                            break;
                        }
                    } while (true);
                }*/

        string ElementInfoText
        {
            get
            {
                if (totalNum != 0)
                {
                    return $"Exporting {currentNum} of {totalNum} Element";
                }
                else
                {
                    return null;
                }

            }
        }

        string FileInfoText
        {
            get
            {
                if (totalFileNum != 0)
                {
                    return $"Exporting {currentFileNum} of {totalFileNum} file";
                }
                else
                {
                    return "";
                }

            }
        }

        double SingleFileP
        {
            get
            {
                if (totalNum != 0)
                {
                    return currentNum / totalNum * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        double TotalFileP
        {
            get
            {
                if (totalFileNum > 1)
                {
                    return currentFileNum / totalFileNum * 100;
                }
                else if (totalFileNum == 1)
                {
                    return SingleFileP;
                }
                else
                {
                    return 0;
                }
            }
        }

        public exportStatusBar(int totalFileNum)
        {
            InitializeComponent();

            this.totalFileNum = totalFileNum;

            







            /*            timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromMilliseconds(10);
                        timer.Tick += Timer_Tick;
                        timer.Start();*/

            /*Exporter.GetTotalNumber += Exporter_GetTotalNumber;

            Exporter.GetElementCount += Exporter_GetElementCount;

            Command.PassCurrentNumb += Command_PassCurrentNumb;*/

            //Timer timer = new Timer(UpdateProperty, null, 0, 1000);

            //updateInformation();

        }

        /*        private async void ConnectServer_1()
                {
                    clientPipe_ReadFileNum = new NamedPipeClientStream(".", "filePipe", PipeDirection.In);
                    reader_ReadFileNum = new StreamReader(clientPipe_ReadFileNum);

                    clientPipe_ReadFileNum.Connect();
                }*/





        /*        private void Timer_Tick(object sender, EventArgs e)
                {
                    OnPropertyChanged();
                }
        */
        /*        private void Command_PassCurrentNumb(object sender, int e)
                {

                    currentFileNum = e;    
                }
                private void Exporter_GetElementCount(object sender, int e)
                {

                    currentNum = e;

                    //AutoResetEvent.Set();  

                }

                private void Exporter_GetTotalNumber(object sender, int e)
                {

                      totalNum = e;

                }*/





        #region d
        protected void OnPropertyChanged(string name = null)
        {
            if (statusText_Single.Visibility == Visibility.Collapsed)
            {
                statusText_Single.Visibility = Visibility.Visible;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            if (currentNum == totalNum && currentFileNum == totalFileNum)
            {
                statusText_Total.Content = "Export Complete!";

                horizontalStackPanel.Visibility = Visibility.Visible;
            }
        }
        #endregion d

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
