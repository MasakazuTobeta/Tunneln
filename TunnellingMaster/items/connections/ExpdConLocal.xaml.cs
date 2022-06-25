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
using TunnellingMaster.items.connections;

namespace TunnellingMaster.items.connections
{
    /// <summary>
    /// ExpdConLocal.xaml の相互作用ロジック
    /// </summary>
    public partial class ExpdConLocal : Expander
    {
        public string hash { 
            get {
                return Common.GetHashedTextString(this.ToString());
            } 
        }

        public override string ToString()
        {
            return this.flow_panel.ToString();
        }

        public bool Equals(ExpdConLocal other)
        {
            if (other == null) return false;
            return (this.ToString().Equals(other.ToString()));
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message",                          // プロパティ名
                typeof(string),                     // プロパティの型
                typeof(Expander),                   // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata("Message"));   // 初期値

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",                            // プロパティ名
                typeof(string),                     // プロパティの型
                typeof(Expander),                   // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata("Title"));     // 初期値

        public ExpdConLocal()
        {
            InitializeComponent();
            SetInitialConnection();
            SetupDropEvent();
        }

        private CommonPannel flow_panel = null;
        private void SetInitialConnection()
        {
            this.flow_panel = (CommonPannel)(new LocalPortForward());
            this.connection_view.Content = this.flow_panel;
        }

        private void DragOver_LocalHost(object sender, DragEventArgs e)
        {

        }

        private void Dropped_LocalHost(object sender, DragEventArgs e)
        {

        }

        private void SetupDropEvent()
        {
            
        }

        private void Setting_LocalHost(object sender, MouseButtonEventArgs e)
        {

        }

        private void Setting_RemoteHost(object sender, MouseButtonEventArgs e)
        {

        }

        private void Deleate_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as StackPanel).Children.Remove(this);
        }

        private void Your_Port_Edit(object sender, MouseButtonEventArgs e)
        {

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
