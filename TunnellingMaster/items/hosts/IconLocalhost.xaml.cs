using ColorHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TunnellingMaster.items.hosts
{
    public enum IconLocalhost_State
    {
        Resource = 0,
        Blank = 1,
        InFlow = 2,
    }
    /// <summary>
    /// IconLocalhost.xaml の相互作用ロジック
    /// </summary>
    public partial class IconLocalhost : UserControl
    {
        private IconLocalhost parent = null;
        private List<IconLocalhost> children = new List<IconLocalhost>();
        public bool enable = false;
        public string address = "localhost";
        public bool hosts_file = false;
        public bool virtual_adpt = false;
        private MyHost _my_host;
        //public Guid hash = Guid.NewGuid();

        public bool Equals(IconLocalhost other)
        {
            if (other == null) return false;
            return (this.Text.Equals(other.Text));
        }

        public override string ToString()
        {
            List<string> _tmp = new List<string>();
            _tmp.Add(this.Text); // 0
            _tmp.Add(this.State.ToString()); // 1
            _tmp.Add(this.address); // 2
            _tmp.Add(this.hosts_file.ToString()); // 3
            _tmp.Add(this.virtual_adpt.ToString()); // 4
            return string.Join(Config.Config.SEPARATOR, _tmp);
        }

        public IconLocalhost(List<string> config)
        {
            InitializeComponent();
            this._my_host = new MyHost((object)this);

            this.Text = config[0];

            IconLocalhost_State _state = IconLocalhost_State.Resource;
            Enum.TryParse(config[1], out _state);
            this.State = _state;

            this.address = config[2];

            this.hosts_file = System.Convert.ToBoolean(config[3]);

            this.virtual_adpt = System.Convert.ToBoolean(config[4]);

            UpdateView();
        }

        public IconLocalhost(string Text = "localhost", IconLocalhost_State State = IconLocalhost_State.Resource)
        {
            InitializeComponent();
            this._my_host = new MyHost((object)this);

            this.Text = Text;
            this.State = State;
            if (Text=="localhost")
            {
                this.address = "127.0.0.1";
            }
            else
            {
                this.address = Common.GenerateRandomIPv4Address();
            }
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
                typeof(IconLocalhost),               // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata("localhost"));  // 初期値

        public IconLocalhost_State State
        {
            get { return (IconLocalhost_State)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
                "State",                                                // プロパティ名
                typeof(IconLocalhost_State),                            // プロパティの型
                typeof(IconLocalhost),                                  // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata(IconLocalhost_State.Resource));    // 初期値

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

        public Color color
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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.UpdateView();
            }
        }

        private void UpdateView()
        {
            switch (this.State)
            {
                case IconLocalhost_State.InFlow:
                    this.icon_std.Visibility = Visibility.Collapsed;
                    this.icon_blank.Visibility = Visibility.Collapsed;
                    this.icon_in_flow.Visibility = Visibility.Visible;
                    break;
                case IconLocalhost_State.Blank:
                    this.icon_std.Visibility = Visibility.Collapsed;
                    this.icon_blank.Visibility = Visibility.Visible;
                    this.icon_in_flow.Visibility = Visibility.Collapsed;
                    break;
                default: //IconLocalhost_State.Resource
                    this.icon_std.Visibility = Visibility.Visible;
                    this.icon_blank.Visibility = Visibility.Collapsed;
                    this.icon_in_flow.Visibility = Visibility.Collapsed;
                    break;
            }
            if (this.parent is null)
            {
                this.color = this.GetRandomColor();
            }
            foreach (IconLocalhost _child in this.children)
            {
                _child.SetParent(this);
            }
        }

        internal MyHost MyHost()
        {
            return this._my_host;
        }

        public IconLocalhost copy_properties(IconLocalhost _src, IconLocalhost _dst)
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
            this.OpenDialogLocalhost();
        }

        public void SetParent(IconLocalhost parent)
        {
            Debug.Print("Set parent");
            this.parent = parent;
            this.Text   = parent.Text;
            this.color  = parent.color;
            this.State  = IconLocalhost_State.InFlow;
            this.address = parent.address; 
            this.hosts_file = parent.hosts_file; 
            this.virtual_adpt = parent.virtual_adpt;
            //this.hash = _parent.hash;
            foreach (IconLocalhost _child in this.children)
            {
                _child.SetParent(parent);
            }
            this.UpdateView();
        }

        public void SetChild(IconLocalhost child)
        {
            this.children.Add(child);
        }


        public void OpenDialogLocalhost()
        {
            dialog.DialogLocalhost _dialog = new dialog.DialogLocalhost(this);
            _dialog.ok.Click += (s, e) =>
            {
                if (_dialog.Verify)
                {
                    IconLocalhost _item = this;
                    while (!(_item.parent is null))
                    {
                        if (ReferenceEquals(_item, _item.parent)){
                            break;
                        }
                        _item = _item.parent;
                    }
                    /* ダイアログから回収してくる値 */
                    _item.Text = _dialog.name.Text;
                    _item.address = _dialog.address.Text;
                    _item.hosts_file = _dialog.add_to_hosts_file.IsOn;
                    _item.virtual_adpt = _dialog.add_virtual_adapter.IsOn;

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

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent("Object"))
            {
                var data = e.Data.GetData("Object");
                if (typeof(IconLocalhost).IsInstanceOfType(data))
                {
                    IconLocalhost parent = (data as IconLocalhost);
                    if (ReferenceEquals(parent, this))
                    {
                        Debug.Print("Is instance of self");
                    }
                    else if (this.State != IconLocalhost_State.Resource)
                    {
                        parent.children.Add(this);
                        this.SetParent(parent);
                    }
                }
            }
            e.Handled = true;
        }


        public void GetPortproxyInfo()
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = @"/c netsh interface portproxy show all /w";
            p.Start();
            string results = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            StringReader sr = new StringReader(results);
            string text_line;
            while ((text_line = sr.ReadLine()) != null)
            {
                if (Regex.IsMatch(text_line, @"\s*\d+.\d+.\d+.\d+\s+\d+\s+\d+.\d+.\d+.\d+\s+\d+"))
                {
                    MatchCollection ip_list = Regex.Matches(text_line, @"\d+.\d+.\d+.\d+");
                    foreach (Match _ip in ip_list)
                    {
                        text_line = text_line.Replace(_ip.ToString(), " ");
                    }
                    MatchCollection port_list = Regex.Matches(text_line, @"\d+");
                    if ((ip_list.Count == 2) && (port_list.Count == 2))
                    {
                        if ((ip_list[0].ToString() == this.address) && (ip_list[1].ToString() == this.address))
                        {
                            Debug.WriteLine(text_line);
                        }
                    }
                }
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
                case IconLocalhost_State.Resource:
                    (this.Parent as StackPanel).Children.Remove(this);
                    MyHost _item = new MyHost(this);
                    (Application.Current.MainWindow as MainWindow).Remove_Host(_item);
                    foreach (IconLocalhost _child in this.children)
                    {
                        _child.Remove_Me();
                    }
                    break;
                default:
                    this.parent = null;
                    this.State = IconLocalhost_State.Blank;
                    this.UpdateView();
                    break;
            }
        }

        private void Context_Edit_Click(object sender, RoutedEventArgs e)
        {
            this.OpenDialogLocalhost();
        }
    }
}
