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
    /// Connections.xaml の相互作用ロジック
    /// </summary>
    public partial class Connections : MainStackCommon
    {
        public Connections()
        {
            InitializeComponent();
        }
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            this.Children.Insert(
                this.Children.Count - 1,
                new items.expanders.ExpdConLocal()
                );
        }
    }
}