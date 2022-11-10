using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TunnellingMaster.items.hosts.dialog
{
    /// <summary>
    /// DialogLocalhost.xaml の相互作用ロジック
    /// </summary>
    public partial class DialogLocalhost : Window
    {
        public DialogLocalhost(IconLocalhost localhost = null)
        {
            InitializeComponent();
            this.add_virtual_adapter.buttonToggle.Click += this.add_virtual_adapter_Click;
            if (!(localhost is null))
            {
                this.name.Text = localhost.Text;
                this.address.Text = localhost.address;
                this.color = localhost.color;
                this.add_to_hosts_file.IsOn = localhost.hosts_file;
                this.add_virtual_adapter.IsOn = localhost.virtual_adpt;
            }
        }

        public Color color
        {
            get
            {
                return (this.icon_top.Foreground as SolidColorBrush).Color;
            }
            set
            {
                Brush _b = new SolidColorBrush(value);
                this.icon_top.Foreground = _b;
            }
        }

        public bool Verify
        {
            get
            {
                bool is_ok = true;
                IPAddress _ip = Common.GetIPv4Addressl(this.address.Text);
                if (_ip is null)
                {
                    is_ok = false;
                }
                else
                {
                    bool _is_ok = !(this.add_virtual_adapter.IsOn && _ip.ToString().StartsWith("127."));
                    if (!_is_ok)
                    {
                        this.label_add_virtual_adapter.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    is_ok = _is_ok;
                }
                if (!is_ok)
                {
                    this.address.Foreground = new SolidColorBrush(Colors.Red);
                }
                return is_ok;
            }
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void add_virtual_adapter_Click(object sender, RoutedEventArgs e)
        {
            if (this.add_virtual_adapter.IsOn)
            {
                if (this.address.Text.StartsWith("127.") || this.address.Text.Length<=0)
                {
                    this.address.Text = Common.GenerateRandomIPv4Address("169.254.0.0/16");
                }
            }
            else
            {
                this.label_add_virtual_adapter.Foreground = new SolidColorBrush(Colors.Black);
                if (this.address.Text.Length <= 0)
                {
                    this.address.Text = Common.GenerateRandomIPv4Address("127.0.0.0/8");
                }
            }
        }

        private void address_TextChanged(object sender, TextChangedEventArgs e)
        {
            IPAddress _ip = Common.GetIPv4AddressInLocal(this.address.Text);
            if (_ip is null)
            {
                this.address.FontWeight = FontWeights.Normal;
            }
            else
            {
                this.address.FontWeight = FontWeights.Bold;
                this.address.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

    }
}
