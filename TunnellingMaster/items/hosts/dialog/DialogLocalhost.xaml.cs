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
        public class ConstPF : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int listenport { get; set; }
            public int connectport { get; set; }
            public bool edited { get; set; }
            private bool _enable;
            public bool enable
            {
                get
                {
                    return this._enable;
                }
                set
                {
                    if (this._enable == value) return;
                    this.edited = true;
                    this._enable = value;
                    var h = PropertyChanged;
                    if (h != null) h(this, new PropertyChangedEventArgs("enable"));
                }
            }
        }

        public List<ConstPF> netsh = new List<ConstPF>();

        public DialogLocalhost(IconLocalhost localhost = null)
        {
            InitializeComponent();
            if (!(localhost is null))
            {
                this.name.Text = localhost.Text;
                this.address.Text = localhost.address;
                this.Color = localhost.Color;
            }
            this.const_pf_grid.ItemsSource = new ObservableCollection<ConstPF> 
                    {
                        new ConstPF { listenport=445, connectport=4445, enable=false, edited=false },
                    };
            this.GetPortproxyInfo();
        }

        public Color Color
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

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            foreach (ServiceController scTemp in scServices)
            {
                if (scTemp.ServiceName == "LanmanServer")
                {
                    Debug.WriteLine(scTemp.StartType);
                }
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {

        }

        public int CountChar(string s, char c)
        {
            return s.Length - s.Replace(c.ToString(), "").Length;
        }

        private void address_TextChanged(object sender, TextChangedEventArgs e)
        {
            IPAddress _ip;
            if (IPAddress.TryParse(this.address.Text, out _ip))
            {
                if (this.CountChar(this.address.Text, '.') == 3)
                {
                    this.address.FontWeight = FontWeights.Bold;
                    this.GetPortproxyInfo();
                }
            }
            else
            {
                this.address.FontWeight = FontWeights.Normal;
            }
        }

        public void UpdateDataGrid()
        {
            if (!(this.const_pf_grid is null))
            {
                if (!(this.const_pf_grid.ItemsSource is null))
                {
                    foreach (ConstPF _pf_netsh in this.netsh)
                    {
                        foreach (ConstPF _pf_gui in this.const_pf_grid.ItemsSource)
                        {
                            if (!(_pf_gui.edited) && (_pf_gui.listenport == _pf_netsh.listenport) && (_pf_gui.connectport == _pf_netsh.connectport))
                            {
                                _pf_gui.enable = _pf_netsh.enable;
                            }
                        }
                    }
                }
            }
        }

        public void GetPortproxyInfo()
        {
            this.netsh = new List<ConstPF>();
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = @"/c netsh interface portproxy show all /w";
            p.Start();
            string results = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            StringReader sr = new StringReader(results);
            string text_line;
            while ((text_line = sr.ReadLine()) != null)
            {
                Debug.WriteLine(text_line);
                if (Regex.IsMatch(text_line, @"\s*\d+.\d+.\d+.\d+\s+\d+\s+\d+.\d+.\d+.\d+\s+\d+"))
                {
                    MatchCollection ip_list = Regex.Matches(text_line, @"\d+.\d+.\d+.\d+");
                    foreach (Match _ip in ip_list)
                    {
                        text_line = text_line.Replace(_ip.ToString(), " ");
                    }
                    MatchCollection port_list = Regex.Matches(text_line, @"\d+");
                    if ((ip_list.Count == 2) && (port_list.Count == 2))
                    {
                        if ((ip_list[0].ToString() == this.address.Text) && (ip_list[1].ToString() == this.address.Text))
                        {
                            this.netsh.Add(new ConstPF
                            {
                                listenport = int.Parse(port_list[0].ToString()),
                                connectport = int.Parse(port_list[1].ToString()),
                                enable = true,
                                edited = false,
                            });
                        }
                    }
                }
            }
            this.UpdateDataGrid();
        }
    }
}
