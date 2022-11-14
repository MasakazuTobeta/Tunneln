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
using System.Text.RegularExpressions;

namespace Tunneln.items.elements
{
    /// <summary>
    /// ElmntYou.xaml の相互作用ロジック
    /// </summary>
    public partial class ElmntYou : UserControl
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",                        // プロパティ名
                typeof(string),                 // プロパティの型
                typeof(ElmntYou),               // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata("2222"));  // 初期値

        public ElmntYou()
        {
            InitializeComponent();
        }

        private void port_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }

        private void port_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けを許可しない
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
