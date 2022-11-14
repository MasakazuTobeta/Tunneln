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
using Tunneln.items.hosts;

namespace Tunneln.items.connections
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Tunneln.items.connections"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Tunneln.items.connections;assembly=Tunneln.items.connections"
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
    ///     <MyNamespace:Connections/>
    ///
    /// </summary>
    public class Connections : ScrollViewer
    {
        static Connections()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Connections), new FrameworkPropertyMetadata(typeof(Connections)));
        }
        public StackPanel panel = new StackPanel();
        public Label label = new Label();
        public Button add_button = new Button();

        public Connections()
        {
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            this.label.Content = "Connections:";
            Common.ButtonAdd(this.add_button);
            this.add_button.Click += Add_Button_Click;
            this.panel.Children.Add(this.label);
            this.panel.Children.Add((Button)this.add_button);
            this.Content = this.panel;
        }

        public List<ExpdConLocal> ConnectionList
        {
            get
            {
                List<ExpdConLocal> ret = new List<ExpdConLocal>();
                foreach (UIElement _item in this.panel.Children)
                {
                    if (_item is ExpdConLocal)
                    {
                        ret.Add((_item as ExpdConLocal));
                    }
                }
                return ret;
            }
        }

        public bool InsertLast(ExpdConLocal new_item)
        {
            foreach (ExpdConLocal _latest in this.ConnectionList)
            {
                if (_latest.Equals(new_item))
                {
                    new_item = null;
                }
            }
            if (!(new_item is null))
            {
                this.panel.Children.Insert(this.panel.Children.Count - 1, new_item);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Add(ExpdConLocal item)
        {
            if (this.InsertLast(item))
            {
                MyConnection _con_item = item._my_connection;
                (Application.Current.MainWindow as MainWindow).Add_Connection(_con_item);
                item.Deleate.Click += (s, e) =>
                {
                    (Application.Current.MainWindow as MainWindow).Remove_Connection(_con_item);
                };
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Add(new ExpdConLocal());
        }
    }
}
