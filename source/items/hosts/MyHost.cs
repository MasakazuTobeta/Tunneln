using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Tunneln.items.hosts
{
    public class MyHost
    {
        public object Item;

        public MyHost(object item)
        {
            this.Item = item;
        }

        public MyHost(List<string> config)
        {
            string _type = config[0];
            config.RemoveAt(0);
            switch (_type)
            {
                case "local":
                    this.Item = (object)(new IconLocalhost(config)); // List<string> -> Restore IconLocalhost
                    break;
                case "remote":
                    this.Item = (object)(new IconRemotehost(config)); // List<string> -> Restore IconRemotehost
                    break;
                case "proxy":
                    this.Item = (object)(new IconRemotehost(config)); // List<string> -> Restore IconRemotehost
                    break;
                default:
                    this.Item = null;
                    break;
            }
        }

        public override string ToString()
        {
            List<string> _tmp = new List<string>();
            switch (this.StringOfType())
            {
                case "local":
                    _tmp.Add("local");
                    _tmp.Add(((IconLocalhost)this.Item).ToString());
                    return string.Join(config.Config.SEPARATOR, _tmp);
                case "remote":
                    _tmp.Add("remote");
                    _tmp.Add(((IconRemotehost)this.Item).ToString());
                    return string.Join(config.Config.SEPARATOR, _tmp);
                case "proxy":
                    _tmp.Add("proxy");
                    _tmp.Add(((IconRemotehost)this.Item).ToString());
                    return string.Join(config.Config.SEPARATOR, _tmp);
                default:
                    _tmp.Add("unknown");
                    _tmp.Add("Unknown host type");
                    return string.Join(config.Config.SEPARATOR, _tmp);
            }
        }

        public string GetName()
        {
            switch (this.StringOfType())
            {
                case "local":
                    return ((IconLocalhost)this.Item).Text;
                default:
                    return ((IconRemotehost)this.Item).Text;
            }
        }

        public string GetRawType()
        {
            switch (this.StringOfType())
            {
                case "local":
                    return "local";
                default:
                    return ((IconRemotehost)this.Item).Type.ToString();
            }
        }

        public string StringOfType()
        {
            Type _type = this.Item.GetType();
            if (_type == typeof(IconLocalhost))
            {
                return "local";
            }
            else if(_type == typeof(IconRemotehost))
            {
                IconRemotehost _remote = (IconRemotehost)Item;
                if (_remote.IsProxy())
                {
                    return "proxy";
                }else if (_remote.IsServer())
                {
                    return "remote";
                }
                else
                {
                    return "unknown";
                }
            }
            else
            {
                return "unknown";
            }
        }
    }

    public class MyHosts
    {
        private List<MyHost> _hosts = new List<MyHost>();

        private MainWindow _main_window = null;
        private Localhosts _localhosts = null;
        private Proxies _proxies = null;
        private Remotehosts _remotehosts = null;

        public MyHosts()
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
                if (this._localhosts is null)
                {
                    this._localhosts = this._main_window.stack_localhosts;
                }
                if (this._proxies is null)
                {
                    this._proxies = this._main_window.stack_proxies;
                }
                if (this._remotehosts is null)
                {
                    this._remotehosts = this._main_window.stack_remotehosts;
                }
            }
        }

        public void Add(MyHost _item)
        {
            this.CheckPanels();
            if (!this._hosts.Contains(_item))
            {
                this._hosts.Add(_item);
                switch (_item.StringOfType())
                {
                    case "local":
                        this._localhosts.Add((IconLocalhost)_item.Item);
                        break;

                    case "remote":
                        this._remotehosts.Add((IconRemotehost)_item.Item);
                        break;

                    case "proxy":
                        this._proxies.Add((IconRemotehost)_item.Item);
                        break;

                    default:
                        break;
                }
            }
        }

        public void Remove(MyHost _item)
        {
            this._hosts.Remove(_item);
        }


        internal MyHost Find(string name, string type)
        {
            this.CheckPanels();
            foreach(MyHost _ref in this._hosts)
            {
                if ((_ref.GetName() == name) && (_ref.GetRawType() == type))
                {
                    return _ref;
                }
            }
            return null;
        }
    }
}
