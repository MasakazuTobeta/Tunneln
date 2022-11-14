using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Tunneln.items.hosts;

namespace Tunneln.items.connections.groups
{
    /// <summary>
    /// YourComputer.xaml の相互作用ロジック
    /// </summary>
    public partial class YourComputer : UserControl
    {
        public YourComputer(string Text = "local\r\nhost")
        {
            InitializeComponent();
            this.panel.Children.Add(new IconLocalhost(Text, IconLocalhost_State.Blank));
        }

        public string Adress { 
            get 
            {
                return (this.panel.Children[1] as IconLocalhost).address;
            } 
        }

        public List<int> Ports
        {
            get
            {
                List<int> ret = new List<int>();
                foreach (string _ports in this.port.Text.Split(","))
                {
                    if (_ports.Contains("-"))
                    {
                        string[] _tmp = _ports.Split("-");
                        int _port_s = Int32.Parse(_tmp[0]);
                        int _port_e = Int32.Parse(_tmp[1]);
                        foreach (int _p in Enumerable.Range(_port_s, _port_e))
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

        public override string ToString()
        {
            List<string> ret = new List<string>();
            ret.Add( "YourComputer:" + this.port.Text );
            if (this.panel.Children.Count >= 2)
            {
                ret.Add(this.panel.Children[1].ToString());
            }
            return string.Join("->", ret); ;
        }
    }
}
