﻿using System;
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

namespace TunnellingMaster.items
{
    /// <summary>
    /// MainStackCommon.xaml の相互作用ロジック
    /// </summary>
    public partial class MainStackCommon : StackPanel
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",                                  // プロパティ名
                typeof(string),                          // プロパティの型
                typeof(MainStackCommon),                 // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata("Text:"));   // 初期値

        public MainStackCommon()
        {
            InitializeComponent();
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}