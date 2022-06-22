using ColorHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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

        public IconLocalhost(string Text = "localhost", IconLocalhost_State State = IconLocalhost_State.Resource)
        {
            InitializeComponent();
            this.Text = Text;
            this.State = State;
            if (Text=="localhost")
            {
                this.address = "127.0.0.1";
            }
            else
            {
                this.address = this.Get_Local_Address_Random();
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
                this.Color = this.GetRandomColor();
            }
            foreach (IconLocalhost _child in this.children)
            {
                _child.SetParent(this);
            }
        }

        public IconLocalhost copy_properties(IconLocalhost _src, IconLocalhost _dst)
        {
            _dst.Text = _src.Text;
            return _dst;
        }

        public override string ToString()
        {
            var ret = new Dictionary<string, string>() { {"type", "localhost"},};
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
            this.OpenDialogLocalhost();
        }

        private void SetParent(IconLocalhost parent)
        {
            Debug.Print("Set parent");
            IconLocalhost _parent = parent;
            while (!(_parent.parent is null))
            {
                _parent = _parent.parent;
            }
            _parent.children.Add(this);
            this.parent = _parent;
            this.Text   = _parent.Text;
            this.Color  = _parent.Color;
            this.State  = IconLocalhost_State.InFlow;
            this.UpdateView();
        }


        public void OpenDialogLocalhost()
        {
            dialog.DialogLocalhost _dialog = new dialog.DialogLocalhost(this);
            _dialog.ok.Click += (s, e) =>
            {
                /* ダイアログから回収してくる値 */
                this.Text = _dialog.name.Text;
                this.address = _dialog.address.Text;
                this.enable = true;
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
                        this.SetParent(parent);
                    }
                }
            }
            e.Handled = true;
        }

        private string Get_Local_Address_Random()
        {
            Random r = new System.Random();
            int no = r.Next(2, 2 << 16);
            List<string> ret = new List<string> {"0", "0" , "0" , "127" };
            int ii = 0;
            foreach (string s in Regex.Split(no.ToString("X"), @"(?<=\G.{2})(?!$)"))
            {
                ret[ii] = Convert.ToInt32(s, 16).ToString();
                ii++;
            }
            ret.Reverse();
            return string.Join(".", ret);
        }
    }
}
