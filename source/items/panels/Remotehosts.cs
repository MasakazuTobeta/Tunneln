using RandomFriendlyNameGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tunneln.items.hosts
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Tunneln.items.hosts"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Tunneln.items.hosts;assembly=Tunneln.items.hosts"
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


        public List<IconRemotehost> IconList
        {
            get
            {
                List<IconRemotehost> ret = new List<IconRemotehost>();
                foreach (UIElement _child in this.panel.Children)
                {
                    if (_child is IconRemotehost)
                    {
                        ret.Add((_child as IconRemotehost));
                    }
                }
                return ret;
            }
        }


        public Remotehosts()
        {
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            this.label.Content = "Remotehosts:";
            Common.ButtonAdd(this.add_button);
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

        internal void Add(IconRemotehost item)
        {
            if (!(this.panel.Children.Contains(item)))
            {
                this.InsertLast(item);
            }
        }

        public void InsertLast(IconRemotehost item)
        {
            if (!(this.panel.Children.Contains(item)))
            {
                this.panel.Children.Insert(this.panel.Children.Count - 1, item);
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.root_MouseLeftButtonDown(null, null);
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
            _new_item.OpenDialogServer();
            this.InsertLast(_new_item);

            _new_item.IsEnabledChanged += (s, e) =>
            {
                if (!(_new_item.enable))
                {
                    /* 初回ダイアログでCancelボタンが押されたためアイコン除去 */
                    this.panel.Children.Remove(_new_item);
                    _new_item = null;
                }
                else
                {
                    (Application.Current.MainWindow as MainWindow).Add_Host(_new_item.MyHost());
                }
            };
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
