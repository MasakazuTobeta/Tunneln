using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using TunnellingMaster.items.connections;
using TunnellingMaster.items.hosts;

namespace TunnellingMaster.Config
{
    class Config
    {
        public const string version = "1.0";
        public static string SEPARATOR = "\t";

        private string _path;
        public string Path
        {
            get { return this._path; }
            set { this._path = value; }
        }


        public List<MyHost> hosts = new List<MyHost>();
        public List<MyConnection> connections = new List<MyConnection>();
        private DispatcherTimer _timer;
        private MainWindow _main_window;


        public Config(string path)
        {
            this._path = path;
            Debug.Print(path);
        }

        private void CheckMainWindow()
        {
            if (this._main_window is null)
            {
                this._main_window = Application.Current.MainWindow as MainWindow;
            }
        }

        public void EnableAutoSave(TimeSpan timeSpan)
        {
            this._timer = new DispatcherTimer();
            this._timer.Interval = timeSpan;
            this._timer.Tick += new EventHandler(this.Save);
            this._timer.Start();
        }

        public void DisableAutoSave()
        {
            _timer.Stop();
        }

        public void Load()
        {
            this.Load(this.Path);
        }

        public void Load(string path)
        {
            List<Exception> ex = new List<Exception>();
            if (File.Exists(path))
            {
                this.CheckMainWindow();
                StreamReader sr = new StreamReader(path);
                {
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            string line = sr.ReadLine();
                            string[] values = line.Split('\t');
                            List<string> lists = new List<string>();
                            lists.AddRange(values);
                            if (lists.Count >= 2)
                            {
                                string _type = lists[0];
                                lists.RemoveAt(0);
                                switch (_type)
                                {
                                    case "Host":
                                        MyHost _host = new MyHost(lists);
                                        this._main_window.Add_Host(_host);
                                        break;
                                    case "Connection":
                                        MyConnection _connection = new MyConnection(lists);
                                        this._main_window.Add_Connection(_connection);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        catch (Exception _e)
                        {
                            ex.Add(_e);
                        }
                    }
                }
            }
            if (ex.Count > 0)
            {
                throw new AggregateException("Multiple Errors Occured", ex);
            }
        }

        public void Save(object sender, EventArgs e)
        {
            this.Save(this.Path);
        }

        public void Save()
        {
            this.Save(this.Path);
        }

        public void Save(string path)
        {
            this.CheckMainWindow();
            List<string> _txt = new List<string>();
            foreach (MyHost _hots in this.hosts)
            {
                List<string> _tmp = new List<string>();
                _tmp.Add("Host");
                _tmp.Add(_hots.ToString().Replace("\r", "").Replace("\n", ""));
                _txt.Add(string.Join(Config.SEPARATOR, _tmp));
            }
            foreach (MyConnection _connection in this.connections)
            {
                List<string> _tmp = new List<string>();
                _tmp.Add("Connection");
                _tmp.Add(_connection.ToString().Replace("\r", "").Replace("\n", ""));
                _txt.Add(string.Join(Config.SEPARATOR, _tmp));
            }
            File.WriteAllLines(path, _txt.Distinct());
        }

        public void Add(object item)
        {
            if (item.GetType() == typeof(MyHost))
            {
                if (!(this.hosts.Contains((MyHost)item))){
                    this.hosts.Add((MyHost)item);
                }
            }else{
                if (!(this.connections.Contains((MyConnection)item)))
                {
                    this.connections.Add((MyConnection)item);
                }
            }
        }

        public void Remove(object item)
        {
            if (item.GetType() == typeof(MyHost))
            {
                if (this.hosts.Contains((MyHost)item))
                {
                    this.hosts.Remove((MyHost)item);
                }
            }
            else
            {
                if (this.connections.Contains((MyConnection)item))
                {
                    this.connections.Remove((MyConnection)item);
                }
            }
        }

        public void Add(List<object> items)
        {
            List<Exception> ex = new List<Exception>();
            foreach (object _item in items)
            {
                try
                {
                    this.Add(_item);
                }catch(Exception _e)
                {
                    ex.Add(_e);
                }
            }
            if (ex.Count > 0)
            {
                throw new AggregateException("Multiple Errors Occured", ex);
            }
        }

        public void Remove(List<object> items)
        {
            List<Exception> ex = new List<Exception>();
            foreach (object _item in items)
            {
                try
                {
                    this.Remove(_item);
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
    }
}
