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

namespace Tunneln.items
{
    /// <summary>
    /// MainButtons.xaml の相互作用ロジック
    /// </summary>
    public partial class MainButtons : StackPanel
    {
        public MainButtons()
        {
            InitializeComponent();
        }

        private void btn_disconnect_all_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).Stop_Connection();
        }

        private void btn_connect_all_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).Start_Connection();
        }

        private bool _focused = false;
        public bool Focused
        {
            get
            {
                return _focused;
            }
            set
            {
                this._focused = value;
            }
        }

        private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focused = true;
            (Application.Current.MainWindow as MainWindow).SetSelectedElement(this);
            (Application.Current.MainWindow as MainWindow).ChangedSelectedElement += this.ChangedSelectedElement;
        }

        private void ChangedSelectedElement(object sender, EventArgs e)
        {
            this.Focused = ReferenceEquals(this, sender);
        }
    }
}
