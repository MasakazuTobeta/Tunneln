using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TunnellingMaster.items;
using TunnellingMaster.items.connections;
using TunnellingMaster.items.hosts;

namespace TunnellingMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string AUTO_SAVE_FILE = @"./settings.csv";
        public TimeSpan AUTO_SAVE_FREQ = new TimeSpan(0, 1, 0);

        public event EventHandler ChangedSelectedElement;

        private Config.Config _config = new Config.Config(AUTO_SAVE_FILE);
        private MyHosts _my_hosts = new MyHosts();
        private MyConnections _my_connections = new MyConnections();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += this.MyInitialize;
        }

        public void SetSelectedElement(object sender)
        {
            this.ChangedSelectedElement?.Invoke(sender, EventArgs.Empty);
        }

        public void Add_Host(MyHost _item)
        {
            Debug.Print(_item.ToString());
            this._my_hosts.Add(_item);
            this._config.Add((object)_item);
        }

        public void Add_Connection(MyConnection _item)
        {
            Debug.Print(_item.ToString());
            this._my_connections.Add(_item);
            this._config.Add((object)_item);
        }

        public void Remove_Host(MyHost _item)
        {
            Debug.Print(_item.ToString());
            this._my_hosts.Remove(_item);
            this._config.Remove((object)_item);
        }

        public void Remove_Connection(MyConnection _item)
        {
            Debug.Print(_item.ToString());
            this._my_connections.Remove(_item);
            this._config.Remove((object)_item);
        }

        public MyHost Find_Host(string name, string type)
        {
            return this._my_hosts.Find(name, type);
        }

        public void Start_Connection(MyConnection _item)
        {
            Debug.Print(_item.ToString());
        }

        public void Start_Connection()
        {
            List<Exception> ex = new List<Exception>();
            foreach (MyConnection _item in this._my_connections)
            {
                try
                {
                    this.Start_Connection(_item);
                }
                catch (Exception _e)
                {
                    ex.Add(_e);
                }
            }
            if (ex.Count > 0)
            {
                throw new AggregateException("Multiple Errors Occured", ex);
            }
        }

        public void Stop_Connection(MyConnection _item)
        {
            Debug.Print(_item.ToString());
        }

        public void Stop_Connection()
        {
            List<Exception> ex = new List<Exception>();
            foreach (MyConnection _item in this._my_connections)
            {
                try
                {
                    this.Stop_Connection(_item);
                }
                catch (Exception _e)
                {
                    ex.Add(_e);
                }
            }
            if (ex.Count > 0)
            {
                throw new AggregateException("Multiple Errors Occured", ex);
            }
        }


        private void MyInitialize(object sender, RoutedEventArgs e)
        {
            this._config.Load();
            if (this.ChangedSelectedElement != null)
            {
                this.ChangedSelectedElement(sender, EventArgs.Empty);
            }
            this.MouseLeftButtonDown += this.root_MouseLeftButtonDown;
            this._config.EnableAutoSave(this.AUTO_SAVE_FREQ);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                /* Ctrl+ S */
                this._config.Save();
            }
        }

        private bool _focused = false;
        public bool Focused
        {
            get{return _focused;}
            set{this._focused = value;}
        }

        private void root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Focused = true;
            this.SetSelectedElement(this);
        }

    }
}
