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

namespace SSH_Tunnel_BOX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_add_connection_Click(object sender, RoutedEventArgs e)
        {
            this.stack_connections.Children.Insert(
                this.stack_connections.Children.Count - 1,
                new Connection());
        }

        private void btn_add_localhost_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_add_proxy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_add_remotehost_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_connect_all_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_disconnect_all_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void bnt_load_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
