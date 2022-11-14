using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TunnellingMaster.items.hosts;

namespace TunnellingMaster.items.connections.groups
{
    /// <summary>
    /// RemoteHost.xaml の相互作用ロジック
    /// </summary>
    public partial class RemoteHost : UserControl
    {
        public RemoteHost(string Text = "remote\r\nhost")
        {
            InitializeComponent();
            this.panel.Children.Add(new IconRemotehost(Text, IconRemotehost_State.Blank, IconRemotehost_Type.Server));
            this.AllowDrop = true;
        }

        public bool IsProxy { 
            get
            {
                return (this.panel.Children[1] as IconRemotehost).IsProxy();
            } 
        }

        public string Adress
        {
            get
            {
                return (this.panel.Children[1] as IconRemotehost).address;
            }
        }

        public bool IsKeyAuthentication { 
            get
            {
                return (this.panel.Children[1] as IconRemotehost).IsKeyAuthentication;
            }
        }

        public List<int> Ports
        {
            get
            {
                List<int> ret = new List<int>();
                foreach(string _ports in this.port.Text.Split(","))
                {
                    if (_ports.Contains("-"))
                    {
                        string[] _tmp = _ports.Split("-");
                        int _port_s = Int32.Parse(_tmp[0]);
                        int _port_e = Int32.Parse(_tmp[1]);
                        foreach(int _p in Enumerable.Range(_port_s, _port_e))
                        {
                            ret.Add(_p);
                        }
                    }
                    else
                    {
                        ret.Add(Int32.Parse(_ports));
                    }
                }
                return ret;
            }
        }

        public string keyfile { 
            get 
            {
                return (this.panel.Children[1] as IconRemotehost).keyfile;
            } 
        }

        public string Username {
            get 
            {
                return (this.panel.Children[1] as IconRemotehost).user;
            }
            set 
            {
                (this.panel.Children[1] as IconRemotehost).user = value;
                (this.panel.Children[1] as IconRemotehost).UpdateView();
            } 
        }

        public string Password
        {
            get
            {
                return (this.panel.Children[1] as IconRemotehost).pass;
            }
            set
            {
                (this.panel.Children[1] as IconRemotehost).pass = value;
                (this.panel.Children[1] as IconRemotehost).UpdateView();
            }
        }

        public override string ToString()
        {
            List<string> ret = new List<string>();
            ret.Add(this.port.Text);
            if (this.panel.Children.Count >= 2)
            {
                ret.Add(this.panel.Children[1].ToString());
            }
            return string.Join("->", ret); ;
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            e.Handled = false;
        }

    }

}
