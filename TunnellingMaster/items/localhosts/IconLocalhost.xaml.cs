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

namespace TunnellingMaster.items.localhosts
{
    /// <summary>
    /// IconLocalhost.xaml の相互作用ロジック
    /// </summary>
    public partial class IconLocalhost : UserControl
    {
        public IconLocalhost()
        {
            InitializeComponent();
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

        public IconLocalhost copy_properties(IconLocalhost _src, IconLocalhost _dst)
        {
            _dst.Text = _src.Text;
            return _dst;
        }

        private StackPanel _parent = null;
        private int _idx;
        private IconLocalhost _kagemusha = null;
        private Grid _root = null;
        private Vector _offset;

        private void MouseDragElementBehavior_DragBegun(object sender, MouseEventArgs e)
        {
            this._offset = this.VisualOffset;
            this.Visibility = Visibility.Hidden;
            this._parent = (StackPanel)this.Parent;
            this._idx = this._parent.Children.IndexOf(this);
            this._root = (Application.Current.MainWindow as MainWindow).root_grid;
            this._kagemusha = this.copy_properties(this, new IconLocalhost());
            this._parent.Children.Remove(this);
            this._root.Children.Add(this);
            this._parent.Children.Insert(this._idx, this._kagemusha);
        }

        private void MouseDragElementBehavior_Dragging(object sender, MouseEventArgs e)
        {
            this.Visibility = Visibility.Visible;

        }

        private void MouseDragElementBehavior_DragFinished(object sender, MouseEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            this._root.Children.Remove(this);
        }

        private void root_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
