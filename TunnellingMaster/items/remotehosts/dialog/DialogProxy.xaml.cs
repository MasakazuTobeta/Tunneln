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
using System.Windows.Shapes;

namespace TunnellingMaster.items.remotehosts.dialog
{
    public enum ProxyType { 
        HTTP = 0,
        SOCKS = 1,
        HTTP_pac = 2,
    };

    /// <summary>
    /// DialogProxy.xaml の相互作用ロジック
    /// </summary>
    public partial class DialogProxy : Window
    {
        public DialogProxy(IconRemotehost remotehost = null)
        {
            InitializeComponent();
            if (!(remotehost is null))
            {
                this.name.Text = remotehost.Text;
                this.proxyType = remotehost.proxyType;
                this.address.Text = remotehost.address;
                this.user.Text = remotehost.user;
                this.pass.Text = remotehost.pass;
                this.Color = remotehost.Color;
            }
        }

        public ProxyType proxyType
        {
            get {
                if (ReferenceEquals(this.type.SelectedItem, this.type_http))
                {
                    return ProxyType.HTTP;
                }else if (ReferenceEquals(this.type.SelectedItem, this.type_socks))
                {
                    return ProxyType.SOCKS;
                }else if (ReferenceEquals(this.type.SelectedItem, this.type_http_pac))
                {
                    return ProxyType.HTTP_pac;
                }
                return ProxyType.HTTP;
            }
            set {
                switch (value)
                {
                    case ProxyType.HTTP:
                        this.type.SelectedItem = this.type_http;
                        break;
                    case ProxyType.SOCKS:
                        this.type.SelectedItem = this.type_socks;
                        break;
                    case ProxyType.HTTP_pac:
                        this.type.SelectedItem = this.type_http_pac;
                        break;
                    default:
                        this.type.SelectedItem = this.type_http;
                        break;
                }
            }
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

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
