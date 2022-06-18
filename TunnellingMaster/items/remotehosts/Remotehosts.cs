using RandomFriendlyNameGenerator;
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

namespace TunnellingMaster.items.remotehosts
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TunnellingMaster.items.Remotehosts"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TunnellingMaster.items.Remotehosts;assembly=TunnellingMaster.items.Remotehosts"
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
    ///     <MyNamespace:Remotehosts/>
    ///
    /// </summary>
    public class Remotehosts : ScrollViewer
    {
        static Remotehosts()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Remotehosts), new FrameworkPropertyMetadata(typeof(Remotehosts)));
        }

        public StackPanel panel = new StackPanel();
        public Label label = new Label();
        public Button add_button = new Button();

        public Remotehosts()
        {
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            this.label.Content = "Remotehosts:";
            Setter.ButtonAdd(this.add_button);
            this.add_button.Click += Add_Button_Click;
            this.panel.Children.Add(this.label);
            this.panel.Children.Add((Button)this.add_button);
            this.Content = this.panel;
        }

        public List<string> get_name_list()
        {
            List<string> _ret = new List<string>();
            foreach (UIElement child in this.panel.Children)
            {
                if (child is IconRemotehost)
                {
                    _ret.Add((child as IconRemotehost).Text);
                }
            }
            return _ret;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            int _idx = this.panel.Children.Count - 1;
            IconRemotehost _new_item = new IconRemotehost("remotehost", IconRemotehost_State.Resource, IconRemotehost_Type.Server);
            List<string> _already_names = this.get_name_list();

            for (int _ii = 1; _ii <= 100; _ii++)
            {
                if (!_already_names.Contains(_new_item.Text))
                {
                    break;
                }
                _new_item.Text = NameGenerator.PersonNames.Get(separator: ".");
            }

            this.panel.Children.Insert(_idx, _new_item);
        }
    }
}
