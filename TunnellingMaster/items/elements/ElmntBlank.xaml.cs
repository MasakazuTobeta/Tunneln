using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Disposables;
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

namespace TunnellingMaster.items.elements
{
    /// <summary>
    /// ElmntBlank.xaml の相互作用ロジック
    /// </summary>
    public partial class ElmntBlank : Border
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();
        public ReactivePropertySlim<string> DropFile { get; }
        public ReactiveCommand<DragEventArgs> FileDropCommand { get; private set; }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",                                  // プロパティ名
                typeof(string),                          // プロパティの型
                typeof(ElmntBlank),                      // プロパティを所有する型＝このクラスの名前
                new PropertyMetadata("local\r\nhost"));  // 初期値

        public ElmntBlank()
        {
            InitializeComponent();
            EnableDragDrop();
        }

        private void EnableDragDrop()
        {
            this.AllowDrop = true;
        }

    }
}
