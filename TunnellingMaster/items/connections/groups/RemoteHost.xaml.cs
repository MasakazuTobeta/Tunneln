using System;
using System.Collections.Generic;
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
    }

}
