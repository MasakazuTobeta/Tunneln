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
using WpfControlLibrary;

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
            return string.Join(config.Config.SEPARATOR, ret);
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

        public CommonPannel flow_panel = null;
        public MyConnection _my_connection;
        private void SetInitialConnection()
        {
            this.flow_panel = (CommonPannel)(new LocalPortForward());
            this.connection_view.Content = this.flow_panel;
            this._my_connection = new MyConnection(this);
            this._my_connection.Connected += (s, e) =>
            {
                this.Connected();
            };
            this._my_connection.Disconnected += (s, e) =>
            {
                this.StopConnection(false);
            };
            this._my_connection.Failed += (s, e) =>
            {
                this.StopConnection(false);
            };
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

        private DragDropBorder _drop_point = null;

        private void connection_view_DragLeave(object sender, DragEventArgs e)
        {
            if (!(this._drop_point is null))
            {
                if (true)//(!(this._drop_point.on_drag))
                {
                    this.flow_panel.Children.Remove(this._drop_point);
                    this._drop_point = null;
                }
            }
        }

        private void connection_view_DragOver(object sender, DragEventArgs e)
        {
            if (this.flow_panel.IsEnabled)
            {
                if (!(this._drop_point is null))
                {
                    var _drop_pos = e.GetPosition(this.flow_panel);
                    foreach (object _item in this.flow_panel.Children)
                    {
                        if (_item.GetType() == typeof(groups.RemoteHost))
                        {
                            StackPanel _ref_arrow = (_item as groups.RemoteHost).arrow;
                            var _ref_pos = _ref_arrow.TranslatePoint(new Point(0, 0), this.flow_panel);
                            if (_ref_pos.X + 25.0 > _drop_pos.X)
                            {
                                this.Insert_Flow_Item((_item as Control), this._drop_point);
                                return;
                            }
                        }
                    }
                    this.Insert_Flow_Item(null, this._drop_point, true);
                }
            }
        }

        private partial class DragDropBorder : UserControl
        {
            private ExpdConLocal parent;
            public bool on_drag = true;
            private Border border = new Border();

            public DragDropBorder(ExpdConLocal parent)
            {
                this.border.Height = 32;
                this.border.Width = 2;
                this.border.Margin = new Thickness(3);
                this.border.BorderBrush = Brushes.Black;
                this.border.BorderThickness = new Thickness(1);
                this.AddChild(this.border);
                this.AllowDrop = true;
            }

            protected override void OnDrop(DragEventArgs e)
            {
                base.OnDrop(e);
                e.Handled = false;
            }

            protected override void OnDragEnter(DragEventArgs e)
            {
                base.OnDragEnter(e);
                e.Handled = false;
            }
        }

        private void connection_view_DragEnter(object sender, DragEventArgs e)
        {
            if (this.flow_panel.IsEnabled)
            {
                if (e.Data.GetDataPresent("Object"))
                {
                    var data = e.Data.GetData("Object");
                    if (typeof(IconRemotehost).IsInstanceOfType(data))
                    {
                        if (this._drop_point is null)
                        {
                            this._drop_point = new DragDropBorder(this);
                            this.flow_panel.Children.Add(this._drop_point);
                        }
                    }
                }
                e.Handled = true;
            }
        }

        private void connection_view_Drop(object sender, DragEventArgs e)
        {
            if (this.flow_panel.IsEnabled)
            {
                if (!(this._drop_point is null))
                {
                    if (e.Data.GetDataPresent("Object"))
                    {
                        var data = e.Data.GetData("Object");
                        if (typeof(IconRemotehost).IsInstanceOfType(data))
                        {
                            IconRemotehost _parent_host = (data as IconRemotehost);
                            if (_parent_host.State == IconRemotehost_State.Resource)
                            {
                                groups.RemoteHost _insert_group = new groups.RemoteHost();
                                IconRemotehost _insert_host = (_insert_group.panel.Children[1] as IconRemotehost);
                                _insert_host.SetParent(_parent_host);
                                _parent_host.SetChild(_insert_host);
                                this.flow_panel.Children.Insert(this.flow_panel.Children.IndexOf(this._drop_point), _insert_group);
                            }
                        }
                    }
                    this.flow_panel.Children.Remove(this._drop_point);
                    this._drop_point = null;
                }
            }
        }

        private void Insert_Flow_Item(Control item1, Control item2, bool add_last = false)
        {
            if (this.flow_panel.IsEnabled)
            {
                int idx_now = this.flow_panel.Children.IndexOf(item2);
                if (add_last)
                {
                    if (!(idx_now == this.flow_panel.Children.Count - 1))
                    {
                        this.flow_panel.Children.Remove(item2);
                        this.flow_panel.Children.Add(item2);
                    }
                }
                else
                {
                    int idx_next = this.flow_panel.Children.IndexOf(item1);
                    if ((idx_next < idx_now) || (idx_now + 1 < idx_next))
                    {
                        this.flow_panel.Children.Remove(item2);
                        this.flow_panel.Children.Insert(idx_next, item2);
                    }
                }
            }
        }

        internal string PfType
        {
            get { return (string)(this.connection_type.SelectedItem as ComboBoxItem).Tag; }
            set 
            {
                foreach (ComboBoxItem _item in this.connection_type.Items)
                {
                    if ((_item.Tag as string) == value)
                    {
                        this.connection_type.SelectedItem = _item;
                        break;
                    }
                }
            }
        }

        public bool Verification()
        {
            return true;
        }

        public void Connected()
        {
            Debug.Print("Connected");
            this.status_icon.Icon = FontAwesome.WPF.FontAwesomeIcon.Link;
            this.status_icon.Spin = true;
            this.status_icon.SpinDuration = 15;
        }

        public void StopConnection(bool do_disconnect=true)
        {
            Debug.Print("Stop connection");
            this.status_icon.Icon = FontAwesome.WPF.FontAwesomeIcon.Unlink;
            this.status_icon.Spin = false;
            this.flow_panel.IsEnabled = true;
        }

        public bool StartConnection()
        {
            Debug.Print("Start connection");
            if (this.Verification())
            {
                this.status_icon.Icon = FontAwesome.WPF.FontAwesomeIcon.Spinner;
                this.status_icon.Spin = true;
                this.status_icon.SpinDuration = 5;
                this.flow_panel.IsEnabled = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void toggle_switch_Click(object sender, EventArgs e)
        {
            if ((e as ToggleSwitch.EventArgsBool).value)
            {
                if (this.StartConnection())
                {
                    this._main_window.Start_Connection(this._my_connection);
                }
            }
            else
            {
                this.StopConnection();
                this._main_window.Stop_Connection(this._my_connection);
            }
        }
    }
}
