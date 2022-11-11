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
using TunnellingMaster.items.connections.groups;
using TunnellingMaster.items.elements;
using TunnellingMaster.items.hosts;

namespace TunnellingMaster.items.connections
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TunnellingMaster.items.connections"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TunnellingMaster.items.connections;assembly=TunnellingMaster.items.connections"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:CommonPannel/>
    ///
    /// </summary>
    public class CommonPannel : StackPanel
    {
        static CommonPannel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommonPannel), new FrameworkPropertyMetadata(typeof(CommonPannel)));
        }

        public CommonPannel()
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.Orientation = Orientation.Horizontal;
            this.AllowDrop = true;
            this.CanHorizontallyScroll = true;
            this.MouseLeftButtonDown += this.root_MouseLeftButtonDown;
        }

        public override string ToString()
        {
            List<string> ret = new List<string>();
            foreach (object _item in this.Children)
            {
                Type _type = _item.GetType();
                if (_type == typeof(groups.TargetComputer))
                {
                    groups.TargetComputer _local = (groups.TargetComputer)_item;
                    if (_local.panel.Children.Count >= 2)
                    {
                        ret.Add("Local=" + ((IconLocalhost)_local.panel.Children[1]).Text + ":" + _local.port.Text);
                    }
                }
                else if (_type == typeof(groups.RemoteHost))
                {
                    groups.RemoteHost _remote = (groups.RemoteHost)_item;
                    if (_remote.panel.Children.Count >= 2)
                    {
                        ret.Add("Remote=" + ((IconRemotehost)_remote.panel.Children[1]).Text + ":" + ((IconRemotehost)_remote.panel.Children[1]).Type.ToString() + ":" + _remote.port.Text);
                    }
                }
            }
            return string.Join(Config.Config.SEPARATOR, ret);
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
