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
using TunnellingMaster.items.hosts;

namespace TunnellingMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string AUTO_SAVE_FILE = @"./settings.json"; 
        private static TimeSpan AUTO_SAVE_FREQ = new TimeSpan(0, 5, 0);
        private DispatcherTimer _timer;
        public event EventHandler ChangedSelectedElement;


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += this.MyInitialize;
        }

        public void SetSelectedElement(object sender)
        {
            this.ChangedSelectedElement?.Invoke(sender, EventArgs.Empty);
        }

        private void MyInitialize(object sender, RoutedEventArgs e)
        {
            this.Load();
            this.stack_localhosts.Verification();
            this.stack_proxies.Verification();
            this.stack_remotehosts.Verification();
            this.stack_connections.Verification();
            this.SetupTimer();
            if (this.ChangedSelectedElement != null)
            {
                this.ChangedSelectedElement(sender, EventArgs.Empty);
            }
            this.MouseLeftButtonDown += this.root_MouseLeftButtonDown;
        }

        public void Load()
        {
            string text = "";
            try{ using (StreamReader sr = new StreamReader(AUTO_SAVE_FILE)){ text = sr.ReadToEnd(); } }
            catch (Exception e){ Debug.WriteLine(e.Message);}
            this.JsonDict = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(text);
        }

        public Dictionary<string, List<Dictionary<string, string>>> JsonDict
        {
            get
            {
                return new Dictionary<string, List<Dictionary<string, string>>>()
                            {
                              {"localhosts", this.stack_localhosts.JsonDict},
                              {"proxies", this.stack_proxies.JsonDict},
                              {"remotehosts", this.stack_remotehosts.JsonDict}
//                              {"connections", this.stack_connections.JsonDict},
                            };
            }
            set
            {
                List<Dictionary<string, string>> _vals;
                if (value.TryGetValue("localhosts", out _vals)) { this.stack_localhosts.JsonDict = _vals; };
                if (value.TryGetValue("proxies", out _vals)) { this.stack_proxies.JsonDict = _vals; };
                if (value.TryGetValue("remotehosts", out _vals)) { this.stack_remotehosts.JsonDict = _vals; };
                //if (value.TryGetValue("connections", out _vals)) { this.stack_connections.JsonDict = _vals; };

            }
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this.JsonDict, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AUTO_SAVE_FILE, json);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                /* Ctrl+ S */
                this.Save();
            }
        }

        private void TimerMethod(object sender, EventArgs e)
        {
            this.Save();
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = AUTO_SAVE_FREQ;
            _timer.Tick += new EventHandler(TimerMethod);
            _timer.Start();
            this.Closing += new CancelEventHandler(StopTimer);
        }

        private void StopTimer(object sender, CancelEventArgs e)
        {
            _timer.Stop();
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
            this.SetSelectedElement(this);
        }

    }
}
