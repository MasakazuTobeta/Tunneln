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

namespace TunnellingMaster.items.remotehosts
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
        public string address = "localhost";
        public string user = "";
        public string pass = "";


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

        public override string ToString()
        {
            var ret = new Dictionary<string, string>() { { "type", "remotehost" }, };
            return ret.ToString();
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

        private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void root_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void root_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void root_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
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
                this.Text = _dialog.name.Text;
                this.address = _dialog.address.Text;
                _dialog.Close();
                this.UpdateView();
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
            while (!(_parent.parent is null))
            {
                _parent = _parent.parent;
            }
            _parent.children.Add(this);

            this.parent = _parent;
            this.Text   = _parent.Text;
            this.Type   = _parent.Type;
            this.Color  = _parent.Color;
            this.State  = IconRemotehost_State.InFlow;
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
                        this.SetParent(parent);
                    }
                }
            }
            e.Handled = true;
        }
    }
}
