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
using static System.Windows.Forms.AxHost;

namespace CMSXtream.Pages.DataEntry
{
    /// <summary>
    /// Interaction logic for DiviceMaintance.xaml
    /// </summary>
    public partial class DiviceMaintance : UserControl
    {
        public DiviceMaintance()
        {
            InitializeComponent();
        }

        private void divCon1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SKTIPAddres != "")
                {
                    if (StaticProperty.SDK.GetConnectState())
                    {
                        MessageBox.Show("Device 1 already connected");
                        return;
                    }

                    int ret = StaticProperty.SDK.sta_ConnectTCP(StaticProperty.SKTIPAddres, StaticProperty.SKTPortName, StaticProperty.SKTCommKey);
                    if (ret == 1)
                    {
                        MessageBox.Show("Device 1 connected");
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
        }

        private void div2Con1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SKTIPAddres_2 != "")
                {
                    if (StaticProperty.SDK_2.GetConnectState())
                    {
                        MessageBox.Show("Device 2 already connected");
                        return;
                    }

                    int ret = StaticProperty.SDK_2.sta_ConnectTCP(StaticProperty.SKTIPAddres, StaticProperty.SKTPortName, StaticProperty.SKTCommKey);
                    if (ret == 1)
                    {
                        MessageBox.Show("Device 2 connected");
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
            }
        }

        private void divDisCon1_Click(object sender, RoutedEventArgs e)
        {
            if (StaticProperty.SKTIPAddres != "")
            {
                if (StaticProperty.SDK.GetConnectState())
                {
                    StaticProperty.SDK.sta_UnRegRealTime_MC();
                    StaticProperty.SDK.sta_DisConnect();                   
                    StaticProperty.SDK.SetConnectState(false);
                    MessageBox.Show("Device 1 Disconnected");
                }
                else
                {
                    MessageBox.Show("Device 1 already Disconnected");
                    return;
                }
            }
        }

        private void div2DisCon1_Click(object sender, RoutedEventArgs e)
        {
            if (StaticProperty.SKTIPAddres_2 != "")
            {
                if (StaticProperty.SDK_2.GetConnectState())
                {
                    StaticProperty.SDK_2.sta_UnRegRealTime_MC();
                    StaticProperty.SDK_2.sta_DisConnect();
                    StaticProperty.SDK_2.SetConnectState(false);
                    MessageBox.Show("Device 2 Disconnected");
                }
            }
        }

        private void divSyncUser_Click(object sender, RoutedEventArgs e)
        {
            if (StaticProperty.SKTIPAddres != "")
            {
                int rec = StaticProperty.SDK.sta_GetAllUserInfo_MC();
                if (rec == 1)
                {
                    MessageBox.Show("Device 1 Members Synchronized");
                }
            }
        }

        private void divSyncLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SDK.GetConnectState())
                {
                    int rec = StaticProperty.SDK.sta_readAttLog();
                    if (rec == 1)
                    {
                        MessageBox.Show("Device 1 Logs Synchronized");
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
        }

        private void div2SyncLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SDK_2.GetConnectState())
                {
                    int rec = StaticProperty.SDK_2.sta_readAttLog();
                    if (rec == 1)
                    {
                        MessageBox.Show("Device 2 Logs Synchronized");
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
        }

        private void divEventSub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SDK.GetConnectState())
                {
                    int rec = StaticProperty.SDK.sta_RegRealTime_MC();
                    if (rec == 1)
                    {
                        MessageBox.Show("Device 1 Event Subscribed");
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }            
        }

        private void div2EventSub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SDK_2.GetConnectState())
                {
                    int rec = StaticProperty.SDK_2.sta_RegRealTime_MC2();
                    if (rec == 1)
                    {
                        MessageBox.Show("Device 2 Event Subscribed");
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
        }

        private void divEventUnSub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SDK.GetConnectState())
                {
                    StaticProperty.SDK.sta_UnRegRealTime_MC();
                    MessageBox.Show("Device 2 Event Unsubscribed");
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
        }

        private void div2EventUnSub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StaticProperty.SDK_2.GetConnectState())
                {
                    StaticProperty.SDK_2.sta_UnRegRealTime_MC2();
                    MessageBox.Show("Device 2 Event Unsubscribed");
                }
            }
            catch (Exception ex)
            {
                LogFile logger = new LogFile();
                logger.MyLogFile(ex);
                throw;
            }
        }
    }
}
