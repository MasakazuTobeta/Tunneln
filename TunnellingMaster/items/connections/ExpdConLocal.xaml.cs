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
using TunnellingMaster.items.hosts;

namespace TunnellingMaster.items.connections
{
    /// <summary>
    /// ExpdConLocal.xaml の相互作用ロジック
    /// </summary>
    public partial class ExpdConLocal : Expander
    {
        private MainWindow _main_window;

        private void CheckMainWindow()
        {
            if (this._main_window is null)
            {
                this._main_window = Application.Current.MainWindow as MainWindow;
            }
        }

        public string hash { 
            get {
                return Common.GetHashedTextString(this.ToString());
            } 
        }

        public override string ToString()
        {
            List<string> ret = new List<string>();
            ret.Add(this.Title);
            ret.Add(this.flow_panel.ToString());
            return string.Join(Config.Config.SEPARATOR, ret);
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

        public ExpdConLocal(List<string> config)
        {
            InitializeComponent();
            SetInitialConnection();
            SetupDropEvent();
            this.CheckMainWindow();
            this.Title = config[0];
            config.RemoveAt(0);
            this.flow_panel.Children.Clear();
            foreach (string _str in config)
            {
                if (_str.StartsWith("Local="))
                {
                    string[] _tmp = _str.Replace("Local=", "").Split(':');
                    if (_tmp.Length == 2)
                    {
                        groups.YourComputer _group = new groups.YourComputer();
                        string _name = _tmp[0];
                        string _port = _tmp[1];
                        MyHost _host = this._main_window.Find_Host(_name, "local");
                        if (!(_host is null))
                        {
                            IconLocalhost _icon = (IconLocalhost)_host.Item;
                            ((IconLocalhost)_group.panel.Children[1]).SetParent(_icon);
                            _icon.SetChild((IconLocalhost)_group.panel.Children[1]);
                            _group.port.Text = _port;
                            this.flow_panel.Children.Add(_group);
                        }
                    }
                }
                else if (_str.StartsWith("Remote="))
                {
                    string[] _tmp = _str.Replace("Remote=", "").Split(':');
                    if (_tmp.Length == 3)
                    {
                        groups.RemoteHost _group = new groups.RemoteHost();
                        string _name = _tmp[0];
                        string _type = _tmp[1];
                        string _port = _tmp[2];
                        MyHost _host = this._main_window.Find_Host(_name, _type);
                        if (!(_host is null))
                        {
                            IconRemotehost _icon = (IconRemotehost)_host.Item;
                            ((IconRemotehost)_group.panel.Children[1]).SetParent(_icon);
                            _icon.SetChild((IconRemotehost)_group.panel.Children[1]);
                            _group.port.Text = _port;
                            this.flow_panel.Children.Add(_group);
                        }
                    }
                }
            }
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

        private void connection_view_DragLeave(object sender, DragEventArgs e)
        {

        }

        private void connection_view_DragOver(object sender, DragEventArgs e)
        {

        }

        private void connection_view_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void connection_view_Drop(object sender, DragEventArgs e)
        {

        }
    }
}
