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

        public string JsonString
        {
            get
            {
                Dictionary<string, string> _dict = new Dictionary<string, string>();
                return JsonSerializer.Serialize(_dict, new JsonSerializerOptions { WriteIndented = true });
            }
        }

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

        public List<Dictionary<string, string>> Verification()
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();
            if (this.panel.Children.Count <= 2)
            {
                this.InsertLast(new ExpdConLocal());
                ret.Add(new Dictionary<string, string>() { { "Info", "Added port forwarding template" } });
            }
            return ret;
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

        public bool InsertLast(ExpdConLocal new_items)
        {
            foreach (ExpdConLocal _latest in this.ConnectionList)
            {
                if (_latest.Equals(new_items))
                {
                    new_items = null;
                }
            }
            if (!(new_items is null))
            {
                this.panel.Children.Insert(this.panel.Children.Count - 1, new_items);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            this.InsertLast(new ExpdConLocal());
        }
    }
}
