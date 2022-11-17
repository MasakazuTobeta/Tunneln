using Renci.SshNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Tunneln.items.connections
{
    public class MyConnection
    {
        public ExpdConLocal Item;
        private MainWindow _main_window = null;


        public MyConnection(ExpdConLocal item)
        {
            this.Item = item;
        }

        private void CheckPanels()
        {
            if (this._main_window is null)
            {
                this._main_window = Application.Current.MainWindow as MainWindow;
            }
        }

        public override string ToString()
        {
            List<string> _tmp = new List<string>();
            _tmp.Add(this.Item.PfType);
            _tmp.Add(this.Item.ToString());
            return string.Join(config.Config.SEPARATOR, _tmp);
        }

        public event EventHandler Connected;
        protected virtual void OnConnected(EventArgs e)
        {
            Connected?.Invoke(this, e);
        }

        public event EventHandler Disconnected;
        protected virtual void OnDisconnected(EventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }

        public event EventHandler Failed;
        protected virtual void OnFailed(EventArgs e)
        {
            Failed?.Invoke(this, e);
        }

        public MyConnection(List<string> config)
        {
            string _type = config[0];
            config.RemoveAt(0);
            this.Item = new ExpdConLocal(config);
            this.Item.PfType = _type;
        }

        #region ******************************* Start connect
        private PrivateKeyFile GetPkeyfile(string path, string pass = "")
        {
            PrivateKeyFile pkeyfile = null;
            if (pass.Length < 1)
            {
                try
                {
                    /* Try no pass */
                    pkeyfile = new PrivateKeyFile(path);
                    return pkeyfile;
                }
                catch{}
            }
            else
            {
                try
                {
                    /* Try with pass */
                    pkeyfile = new PrivateKeyFile(path, pass);
                    return pkeyfile;
                }
                catch
                {
                    try
                    {
                        /* Try no pass */
                        pkeyfile = new PrivateKeyFile(path);
                        return pkeyfile;
                    }
                    catch { }
                }
            }
            List<string> _u_p = this._main_window.GetUsernameAndPassword(path, username: null);
            if (_u_p.Count >= 2)
            {
                try
                {
                    /* Input pass */
                    pkeyfile = new PrivateKeyFile(path, _u_p[1]);
                    return pkeyfile;
                }
                catch
                {
                    this._main_window.RemoveUsernameAndPassword(path, username: null);
                }
            }
            return pkeyfile;
        }


        private List<ConnectionInfo> CreateConnectionInfo(groups.RemoteHost _host, string adress = null, List<int> ports = null)
        {
            List<ConnectionInfo> ret = new List<ConnectionInfo>();
            bool _exist_username = (_host.Username.Length > 0);
            bool _exist_password = (_host.Password.Length > 0);
            bool _is_keyauth = _host.IsKeyAuthentication;
            if (adress is null)
            {
                adress = _host.Adress;
            }
            if (ports is null)
            {
                ports = _host.Ports;
            }
            if (_exist_username && _exist_password && !_is_keyauth)
            {
                /* PassAuth - UserName:Exist, UserPass:Exist */
                foreach (int _p in ports)
                {
                    ret.Add((ConnectionInfo)(new PasswordConnectionInfo(adress, _p, _host.Username, _host.Password)));
                }
            }
            else if (_exist_username && _exist_password && _is_keyauth)
            {
                /* 1.KeyAuth 2.PassAuth - UserName:Exist, UserPass:Exist */
                PrivateKeyFile pkeyfile = this.GetPkeyfile(_host.keyfile, pass: _host.Password);
                if (!(pkeyfile is null))
                {
                    PrivateKeyAuthenticationMethod auth = new PrivateKeyAuthenticationMethod(_host.Username, pkeyfile);
                    foreach (int _p in ports)
                    {
                        ret.Add(new ConnectionInfo(adress, _p, _host.Username, auth));
                    }
                }
                else
                {
                    foreach (int _p in ports)
                    {
                        ret.Add((ConnectionInfo)(new PasswordConnectionInfo(adress, _p, _host.Username, _host.Password)));
                    }
                }
            }
            else if (_exist_username && !_exist_password && !_is_keyauth)
            {
                /* PassAuth - UserName:Exist */
                List<string> _uname_pass = this._main_window.GetUsernameAndPassword(_host.Adress, username: _host.Username);
                if (_uname_pass.Count >= 2)
                {
                    foreach (int _p in ports)
                    {
                        ret.Add((ConnectionInfo)(new PasswordConnectionInfo(adress, _p, _uname_pass[0], _uname_pass[1])));
                    }
                }
            }
            else if (!_exist_username && !_exist_password && !_is_keyauth)
            {
                /* PassAuth */
                List<string> _uname_pass = this._main_window.GetUsernameAndPassword(_host.Adress);
                if (_uname_pass.Count >= 2)
                {
                    _host.Username = _uname_pass[0];
                    foreach (int _p in ports)
                    {
                        ret.Add((ConnectionInfo)(new PasswordConnectionInfo(adress, _p, _uname_pass[0], _uname_pass[1])));
                    }
                }

            }
            else if (_exist_username && !_exist_password && _is_keyauth)
            {
                /* KeyAuth - UserName:Exist */
                PrivateKeyFile pkeyfile = this.GetPkeyfile(_host.keyfile, pass: _host.Password);
                if (!(pkeyfile is null))
                {
                    PrivateKeyAuthenticationMethod auth = new PrivateKeyAuthenticationMethod(_host.Username, pkeyfile);
                    foreach (int _p in ports)
                    {
                        ret.Add(new ConnectionInfo(adress, _p, _host.Username, auth));
                    }
                }
            }
            else if (!_exist_username && !_exist_password && _is_keyauth)
            {
                /* KeyAuth */
                PrivateKeyFile pkeyfile = this.GetPkeyfile(_host.keyfile, pass: _host.Password);
                if (!(pkeyfile is null))
                {
                    List<string> _uname_pass = this._main_window.GetUsernameAndPassword(_host.Adress, pass : null);
                    _host.Username = _uname_pass[0];
                    PrivateKeyAuthenticationMethod auth = new PrivateKeyAuthenticationMethod(_host.Username, pkeyfile);
                    foreach (int _p in ports)
                    {
                        ret.Add(new ConnectionInfo(adress, _p, _host.Username, auth));
                    }
                }

            }
            foreach (ConnectionInfo _info in ret)
            {
                _info.Timeout = new TimeSpan(0, 1, 0);
            }

            return ret;
        }

        private List<SshClient> clients = new List<SshClient>();
        private List<ForwardedPort> forwards = new List<ForwardedPort>();
        internal void Start_Connection()
        {
            if ((this.forwards.Count <= 0) && (this.clients.Count <= 0))
            {
                this.CheckPanels();
                groups.YourComputer _localhost = null;
                if (this.Item.PfType == "DPF")
                {
                    if (this.Item.flow_panel.Children[this.Item.flow_panel.Children.Count - 1].GetType() == typeof(groups.RemoteHost))
                    {
                        this.Item.flow_panel.Children.Add(new groups.SocksIcon());
                    }
                }
                foreach (object item in this.Item.flow_panel.Children)
                {
                    if (item.GetType() == typeof(groups.YourComputer))
                    {
                        _localhost = (groups.YourComputer)item;
                    }
                    else if (item.GetType() == typeof(groups.RemoteHost))
                    {
                        if (_localhost is null)
                        {
                            throw new NotSupportedException("Localhost must be defined as the leftmost. " + this.ToString());
                        }
                        else
                        {
                            groups.RemoteHost _group_host = (groups.RemoteHost)item;
                            if (_group_host.IsProxy)
                            {
                                throw new NotImplementedException("Proxy servers not yet supported");
                            }
                            else
                            {
                                bool _is_target_host = (!(this.Item.flow_panel.Children.IndexOf((UIElement)item) < this.Item.flow_panel.Children.Count - 1));
                                if (!(_is_target_host))
                                {
                                    /* Jump host */
                                    if (clients.Count < 1)
                                    {
                                        /* First host */
                                        List<ConnectionInfo> _connectionInfo = this.CreateConnectionInfo(_group_host);
                                        if (_connectionInfo.Count <= 0)
                                        {
                                            throw new IndexOutOfRangeException("Connection information could not be created.");
                                        }
                                        SshClient _client = new SshClient(_connectionInfo[0]);
                                        _client.ErrorOccurred += _client_ErrorOccurred;
                                        _client.Connect();
                                        this.clients.Insert(0, _client);
                                    }
                                    else
                                    {
                                        /* N th host */
                                        ForwardedPortLocal _forward = new ForwardedPortLocal(_localhost.Adress, _group_host.Adress, (uint)_group_host.Ports[0]);
                                        this.clients[0].AddForwardedPort(_forward);
                                        _forward.Exception += _forward_Exception;
                                        _forward.Start();
                                        this.forwards.Insert(0, _forward);
                                        List<ConnectionInfo> _connectionInfo = this.CreateConnectionInfo(_group_host, adress: _forward.BoundHost, ports: new List<int> { (int)_forward.BoundPort });
                                        if (_connectionInfo.Count > 0)
                                        {
                                            SshClient _client = new SshClient(_connectionInfo[0]);
                                            _client.ErrorOccurred += _client_ErrorOccurred;
                                            _client.Connect();
                                            this.clients.Insert(0, _client);
                                        }
                                        else
                                        {
                                            this.Failed.Invoke(this, EventArgs.Empty);
                                        }
                                    }
                                }
                                else
                                {
                                    /* Target host */
                                    for (int _ii = 0; _ii < _group_host.Ports.Count; _ii++)
                                    {
                                        if (_ii >= _localhost.Ports.Count)
                                        {
                                            break;
                                        }
                                        /* Local port forwarding */
                                        int _local_port = _localhost.Ports[_ii];
                                        int _remote_port = _group_host.Ports[_ii];
                                        ForwardedPortLocal _forward = new ForwardedPortLocal(_localhost.Adress, (uint)_local_port, _group_host.Adress, (uint)_remote_port);
                                        this.clients[0].AddForwardedPort(_forward);
                                        _forward.Exception += _forward_Exception;
                                        _forward.Start();
                                        this.forwards.Insert(0, _forward);
                                    }
                                    this.Connected.Invoke(this, EventArgs.Empty);
                                }
                            }

                        }
                    }
                    else if (item.GetType() == typeof(groups.SocksIcon))
                    {
                        /* Dynamc port forwarding */
                        ForwardedPortDynamic _forward = new ForwardedPortDynamic(_localhost.Adress, (uint)_localhost.Ports[0]);
                        this.clients[0].AddForwardedPort(_forward);
                        _forward.Exception += _forward_Exception;
                        _forward.Start();
                        this.forwards.Insert(0, _forward);
                        this.Connected.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }
        #endregion


        #region ******************************* Disconnect
        public void Disconnect()
        {
            if ((this.forwards.Count>0) || (this.clients.Count>0))
            {
                for (int ii=0; ii < Math.Max(this.forwards.Count, this.clients.Count); ii++)
                {
                    if (this.forwards.Count > ii)
                    {
                        if (this.forwards[ii].IsStarted)
                        {
                            this.forwards[ii].Stop();
                        }

                    }
                    if (this.clients.Count > ii)
                    {
                        if (this.clients[ii].IsConnected)
                        {
                            this.clients[ii].Disconnect();
                            this.clients[ii].Dispose();
                        }

                    }
                }

                this.clients = new List<SshClient>();
                this.forwards = new List<ForwardedPort>();
            }
        }

        internal void Stop_Connection()
        {
            this.Disconnect();
        }

        private void _forward_Exception(object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            if ((sender.GetType() == typeof(ForwardedPortDynamic)) ||
                (sender.GetType() == typeof(ForwardedPortLocal)))
            {
            }
            else
            {
                this.Disconnect();
            }

        }

        private void _client_ErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            this.Disconnect();
        }
        #endregion
    }

    public class MyConnections : IEnumerable<MyConnection>
    {
        private List<MyConnection> _connections = new List<MyConnection>();
        private MainWindow _main_window = null;
        private Connections _panel = null;

        public int Count { get { return this._connections.Count; } }

        public MyConnections()
        {

        }

        private void CheckPanels()
        {
            if (this._main_window is null)
            {
                this._main_window = Application.Current.MainWindow as MainWindow;
            }
            if (!(this._main_window is null))
            {
                if (this._panel is null)
                {
                    this._panel = this._main_window.stack_connections;
                }
            }
        }

        public void Add(MyConnection _item)
        {
            this.CheckPanels();
            this._connections.Add(_item);
            this._panel.Add(_item.Item);
        }

        public void Remove(MyConnection _item)
        {
            this._connections.Remove(_item);
        }

        public MyConnection this[int index]
        {
            get { return this._connections[index]; }
        }

        public IEnumerator<MyConnection> GetEnumerator()
        {
            foreach (MyConnection s in this._connections)
            {
                yield return s;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
