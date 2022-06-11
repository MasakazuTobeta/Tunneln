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
using TunnellingMaster.items.localhosts;

namespace TunnellingMaster.items
{
    /// <summary>
    /// Localhosts.xaml の相互作用ロジック
    /// </summary>
    public partial class Localhosts : ScrollViewer
    {

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",                           // プロパティ名
                typeof(string),                   // プロパティの型
                typeof(Localhosts),               // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata("Text:"));   // 初期値

        public Localhosts()
        {
            InitializeComponent();
        }

        public List<string> get_name_list()
        {
            List<string> _ret = new List<string>();
            foreach (UIElement child in this.panel.Children)
            {
                if (child is IconLocalhost)
                {
                    _ret.Add((child as IconLocalhost).Text);
                }
            }
            return _ret;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            int _idx = this.panel.Children.Count - 1;
            IconLocalhost _new_item = new IconLocalhost();
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
