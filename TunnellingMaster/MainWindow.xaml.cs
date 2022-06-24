using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        private static TimeSpan AUTO_SAVE_FREQ = new TimeSpan(0, 0, 5);
        private DispatcherTimer _timer;
        private Settings _settings = new Settings();

        public MainWindow()
        {
            InitializeComponent();
            this.SetupTimer();
        }

        private void TimerMethod(object sender, EventArgs e)
        {
            List<IconLocalhost> localhosts = new List<IconLocalhost>();
            foreach (UIElement _item in this.stack_localhosts.panel.Children)
            {
                if (_item is IconLocalhost)
                {
                    localhosts.Add((IconLocalhost)_item);
                }
                _settings.localhosts = localhosts;
            }

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
    }
}
