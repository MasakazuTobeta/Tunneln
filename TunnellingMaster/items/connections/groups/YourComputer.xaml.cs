using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// YourComputer.xaml の相互作用ロジック
    /// </summary>
    public partial class YourComputer : UserControl
    {
        public YourComputer(string Text = "local\r\nhost")
        {
            InitializeComponent();
            this.panel.Children.Add(new IconLocalhost(Text, IconLocalhost_State.Blank));
        }

    }
}
