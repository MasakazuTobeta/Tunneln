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
using TunnellingMaster.items.dialogs;
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
            _item.Start_Connection(false);
        }

        public void Start_Connection()
        {
            List<Exception> ex = new List<Exception>();
            foreach (MyConnection _item in this._my_connections)
            {
                try
                {
                    foreach (object _tmp in this.stack_connections.panel.Children)
                    {
                        if (_tmp.GetType() == typeof(ExpdConLocal))
                        {
                            ExpdConLocal _con = (ExpdConLocal)_tmp;
                            if (!(_con.toggle_switch.IsOn))
                            {
                                _con.toggle_switch.IsOn = true;
                            }
                        }
                    }
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
            _item.Stop_Connection(false);
        }

        public void Stop_Connection()
        {
            List<Exception> ex = new List<Exception>();
            foreach (MyConnection _item in this._my_connections)
            {
                try
                {
                    foreach (object _tmp in this.stack_connections.panel.Children)
                    {
                        if (_tmp.GetType() == typeof(ExpdConLocal))
                        {
                            ExpdConLocal _con = (ExpdConLocal)_tmp;
                            if (_con.toggle_switch.IsOn)
                            {
                                _con.toggle_switch.IsOn = false;
                            }
                        }
                    }
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

        internal void Connected(MyConnection myConnection)
        {
            myConnection.Item.Connected();
        }

        internal void Disconnect(MyConnection myConnection)
        {
            myConnection.Item.StopConnection(false);
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


        #region ******************************* Temporary Authorization
        Dictionary<string, string> _user_name = new Dictionary<string, string>();
        Dictionary<string, string> _password = new Dictionary<string, string>();
        public List<string> GetUsernameAndPassword(string kwd, string msg = "", string username = "", string pass = "")
        {
            List<string> ret = new List<string>();
            InputUsernameAndPassword _form = new InputUsernameAndPassword();
            _form.Target = kwd;
            if (msg.Length > 0)
            {
                _form.Message = msg;
            }
            if (!(username is null))
            {
                if (!(username.Length > 0))
                {
                    this._user_name[kwd] = username;
                }
            }
            else
            {
                this._user_name[kwd] = "";
                _form.setVisible(uname: Visibility.Collapsed);
            }
            if (pass is null)
            {
                _form.setVisible(pass: Visibility.Collapsed);
            }
            if (!this._user_name.ContainsKey(kwd))
            {
                // Get Username and Password
                if ((bool)_form.ShowDialog())
                {
                    string kwd_pass = this._user_name[kwd] + "@" + kwd;
                    ret.Add(this._user_name[kwd]);
                    ret.Add(this._password[kwd_pass]);
                }
            }
            else
            {
                if (pass is null)
                {
                    ret.Add(this._user_name[kwd]);
                    ret.Add("");
                }
                else
                {
                    string kwd_pass = this._user_name[kwd] + "@" + kwd;
                    if (!this._password.ContainsKey(kwd_pass))
                    {
                        // Get Password
                        _form.Username = this._user_name[kwd];
                        if ((bool)_form.ShowDialog())
                        {
                            this._password[kwd_pass] = _form.Password;
                            ret.Add(this._user_name[kwd]);
                            ret.Add(this._password[kwd_pass]);
                        }
                    }
                    else
                    {
                        ret.Add(this._user_name[kwd]);
                        ret.Add(this._password[kwd_pass]);
                    }
                }
            }
            return ret;
        }
        public void RemoveUsernameAndPassword(string kwd, string username = "")
        {
            if (username is null)
            {
                username = "";
            }
            if (this._user_name.ContainsKey(kwd))
            {
                this._user_name.Remove(kwd);
            }
            if (this._password.ContainsKey(username + "@" + kwd))
            {
                this._user_name.Remove(username + "@" + kwd);
            }
        }
        #endregion
    }
}
