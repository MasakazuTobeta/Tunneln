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
using ColorHelper;
using TunnellingMaster.items.connections;

namespace TunnellingMaster.items.hosts
{
    public enum IconRemotehost_State
    {
        Resource = 0,
        Blank = 1,
        InFlow = 2,
    }

    public enum IconRemotehost_Type
    {
        Server = 0,
        Proxy  = 1,
    }

    /// <summary>
    /// IconRemotehost.xaml の相互作用ロジック
    /// </summary>
    public partial class IconRemotehost : UserControl
    {
        private IconRemotehost parent = null;
        private List<IconRemotehost> children = new List<IconRemotehost>();
        public dialog.ProxyType proxyType = 0;
        public string address = "";
        public string user    = "";
        public string pass    = "";
        public string keyfile = "";
        public bool enable = false;
        private MyHost _my_host;

        public bool Equals(IconLocalhost other)
        {
            if (other == null) return false;
            return (this.Text.Equals(other.Text));
        }

        public override string ToString()
        {
            List<string> _tmp = new List<string>();
            _tmp.Add(this.Text); // 0
            _tmp.Add(this.Type.ToString()); // 1
            _tmp.Add(this.State.ToString()); // 2
            _tmp.Add(this.address); // 3
            _tmp.Add(this.user); // 4
            _tmp.Add(this.keyfile); // 5
            _tmp.Add(EncryptUtils.Encrypt(this.pass)); // 6
            return string.Join(Config.Config.SEPARATOR, _tmp);
        }

        public IconRemotehost(List<string> config)
        {
            InitializeComponent();
            this._my_host = new MyHost((object)this);

            this.Text = config[0];

            IconRemotehost_Type _type = IconRemotehost_Type.Server;
            Enum.TryParse(config[1], out _type);
            this.Type = _type;

            IconRemotehost_State _state = IconRemotehost_State.Resource;
            Enum.TryParse(config[2], out _state);
            this.State = _state;

            this.address = config[3];

            this.user = config[4];

            this.keyfile = config[5];

            this.pass = EncryptUtils.Decrypt(config[6]);

            UpdateView();
        }

        public IconRemotehost(string Text = "remotehost", 
                              IconRemotehost_State State = IconRemotehost_State.Resource, 
                              IconRemotehost_Type Type = IconRemotehost_Type.Server)
        {
            InitializeComponent();
            this._my_host = new MyHost((object)this);
            this.Text  = Text;
            this.State = State;
            this.Type  = Type;
            UpdateView();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",                              // プロパティ名
                typeof(string),                      // プロパティの型
                typeof(IconRemotehost),               // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata("remotehost"));  // 初期値

        public IconRemotehost_State State
        {
            get { return (IconRemotehost_State)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                "State",                                                // プロパティ名
                typeof(IconRemotehost_State),                            // プロパティの型
                typeof(IconRemotehost),                                  // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata(IconRemotehost_State.Resource));    // 初期値


        public IconRemotehost_Type Type
        {
            get { return (IconRemotehost_Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(
                "Type",                                               // プロパティ名
                typeof(IconRemotehost_Type),                          // プロパティの型
                typeof(IconRemotehost),                               // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata(IconRemotehost_Type.Server));    // 初期値

        public Color Color
        {
            get
            {
                return (this.icon1_image.Foreground as SolidColorBrush).Color;
            }
            set
            {
                Brush _b = new SolidColorBrush(value);
                this.icon1_image.Foreground = _b;
                this.icon2_image.Foreground = _b;
            }
        }

        internal MyHost MyHost()
        {
            return this._my_host;
        }

        public FontAwesome.WPF.FontAwesomeIcon Icon
        {
            get
            {
                return this.icon1_image.Icon;
            }
            set
            {
                this.icon1_image.Icon = value;
                this.icon2_image.Icon = value;
            }

        }

        public Color GetRandomColor(int s = 100, int v = 80)
        {
            Random r = new Random();
            int h = r.Next(0, 100);
            RGB rgb = ColorHelper.ColorConverter.HsvToRgb(new HSV(Convert.ToByte(h),
                                                                  Convert.ToByte(s),
                                                                  Convert.ToByte(v)));
            return Color.FromArgb(255,
                                  Convert.ToByte(rgb.R),
                                  Convert.ToByte(rgb.G), 
                                  Convert.ToByte(rgb.B));
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
                if (value)
                {
                    this.Background = new SolidColorBrush(Color.FromArgb(64, 161, 251, 142));
                }
                else
                {
                    this.Background = new SolidColorBrush(Color.FromArgb(0, 161, 251, 142));
                }
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

        private void UpdateView()
        {
            switch (this.State)
            {
                case IconRemotehost_State.InFlow:
                    this.icon_std.Visibility = Visibility.Collapsed;
                    this.icon_blank.Visibility = Visibility.Collapsed;
                    this.icon_in_flow.Visibility = Visibility.Visible;
                    break;
                case IconRemotehost_State.Blank:
                    this.icon_std.Visibility = Visibility.Collapsed;
                    this.icon_blank.Visibility = Visibility.Visible;
                    this.icon_in_flow.Visibility = Visibility.Collapsed;
                    break;
                default: //IconRemotehost_State.Resource
                    this.icon_std.Visibility = Visibility.Visible;
                    this.icon_blank.Visibility = Visibility.Collapsed;
                    this.icon_in_flow.Visibility = Visibility.Collapsed;
                    break;
            }
            switch (this.Type)
            {
                case IconRemotehost_Type.Proxy:
                    this.Icon = FontAwesome.WPF.FontAwesomeIcon.Connectdevelop;
                    break;
                default:
                    this.Icon = FontAwesome.WPF.FontAwesomeIcon.Server;
                    break;
            }
            if (this.parent is null)
            {
                this.Color = this.GetRandomColor();
            }
            foreach (IconRemotehost _child in this.children)
            {
                _child.SetParent(this);
            }
        }

        public IconRemotehost copy_properties(IconRemotehost _src, IconRemotehost _dst)
        {
            _dst.Text = _src.Text;
            return _dst;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                //data.SetData(DataFormats.StringFormat, this.ToString());
                data.SetData("Object", this);
                Dispatcher.BeginInvoke(
                    new Action(() => DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move))
                    );
            }
        }

        private void root_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.OpenDialogServer();
        }

        public void OpenDialogServer()
        {
            dialog.DialogServer _dialog = new dialog.DialogServer(this);
            _dialog.ok.Click += (s, e) =>
            {
                if (_dialog.Verify)
                {
                    IconRemotehost _item = this;
                    while (!(_item.parent is null))
                    {
                        if (ReferenceEquals(_item, _item.parent))
                        {
                            break;
                        }
                        _item = _item.parent;
                    }

                    /* ダイアログから回収してくる値 */
                    _item.Text = _dialog.name.Text;
                    _item.address = _dialog.address.Text;
                    _item.user = _dialog.user.Text;
                    _item.pass = _dialog.pass.Password;
                    _item.keyfile = _dialog.key.Text;

                    _item.enable = true;
                    _dialog.Close();
                    _item.UpdateView();
                }
            };
            _dialog.Closed += (s, e) =>
            {
                this.IsEnabled = true;
            };
            this.IsEnabled = false;
            _dialog.Show();
        }

        public void SetParent(IconRemotehost parent)
        {
            Debug.Print("Set parent");
            IconRemotehost _parent = parent;
            this.parent = _parent;
            this.Text   = _parent.Text;
            this.Type   = _parent.Type;
            this.Color  = _parent.Color;
            this.State  = IconRemotehost_State.InFlow;
            this.address = _parent.address;
            this.user = _parent.user;
            this.pass = _parent.pass;
            this.keyfile = _parent.keyfile;
            this.UpdateView();
        }

        public void SetChild(IconRemotehost child)
        {
            this.children.Add(child);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent("Object"))
            {
                var data = e.Data.GetData("Object");
                if (typeof(IconRemotehost).IsInstanceOfType(data))
                {
                    IconRemotehost parent = (data as IconRemotehost);
                    if (ReferenceEquals(parent, this))
                    {
                        Debug.Print("Is instance of self");
                    }
                    else if (this.State != IconRemotehost_State.Resource)
                    {
                        parent.children.Add(this);
                        this.SetParent(parent);
                    }
                }
            }
            e.Handled = true;
        }

        internal bool IsProxy()
        {
            return (this.Type == IconRemotehost_Type.Proxy);
        }

        internal bool IsServer()
        {
            return (this.Type == IconRemotehost_Type.Server);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.UpdateView();
            }
        }

        private void Context_Remove_Click(object sender, RoutedEventArgs e)
        {
            this.Remove_Me();
        }

        private void Remove_Me()
        {
            switch (this.State)
            {
                case IconRemotehost_State.Resource:
                    (this.Parent as StackPanel).Children.Remove(this);
                    MyHost _item = new MyHost(this);
                    (Application.Current.MainWindow as MainWindow).Remove_Host(_item);
                    foreach (IconRemotehost _child in this.children)
                    {
                        _child.Remove_Me();
                    }
                    break;
                case IconRemotehost_State.InFlow:
                    this.parent = null;
                    this.State = IconRemotehost_State.Blank;
                    this.UpdateView();
                    break;
                default:
                    if ((this.Parent as StackPanel).Parent.GetType() == typeof(items.connections.groups.RemoteHost))
                    {
                        items.connections.groups.RemoteHost _flow_icon = (items.connections.groups.RemoteHost)(this.Parent as StackPanel).Parent;
                        (_flow_icon.Parent as StackPanel).Children.Remove(_flow_icon);
                    }
                    break;
            }
        }

        private void Context_Edit_Click(object sender, RoutedEventArgs e)
        {
            this.OpenDialogServer();
        }
    }
}
