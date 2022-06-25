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

        public bool Equals(IconLocalhost other)
        {
            if (other == null) return false;
            return (this.Text.Equals(other.Text));
        }

        public override string ToString()
        {
            return this.Text;
        }

        public Dictionary<string, string> JsonDict
        {
            get
            {
                return new Dictionary<string, string>()
                            {
                              {"name"   , this.Text},
                              {"address", this.address},
                              {"user"   , this.user},
                              {"pass"   , EncryptUtils.Encrypt(this.pass)},
                              {"keyfile", this.keyfile},
                              {"type"   , this.Type.ToString()},
                              {"state"  , this.State.ToString()},
                            };
            }
            set
            {
                string _val;
                if (value.TryGetValue("name", out _val)) { this.Text = _val; };
                if (value.TryGetValue("address", out _val)) { this.address = _val; };
                if (value.TryGetValue("user", out _val)) { this.user = _val; };
                if (value.TryGetValue("pass", out _val)) { this.pass = EncryptUtils.Decrypt(_val); };
                if (value.TryGetValue("keyfile", out _val)) { this.keyfile = _val; };
                if (value.TryGetValue("type", out _val)) { if (Enum.TryParse(_val, out IconRemotehost_Type _type)) { this.Type = _type; }; };
                if (value.TryGetValue("state", out _val)) { if (Enum.TryParse(_val, out IconRemotehost_State _state)) { this.State = _state; }; };
                this.UpdateView();
            }
        }

        public IconRemotehost(string Text = "remotehost", 
                              IconRemotehost_State State = IconRemotehost_State.Resource, 
                              IconRemotehost_Type Type = IconRemotehost_Type.Server)
        {
            InitializeComponent();
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

        private void SetParent(IconRemotehost parent)
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
    }
}
