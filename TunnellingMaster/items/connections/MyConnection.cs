using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TunnellingMaster.items.connections
{
    public class MyConnection
    {
        public ExpdConLocal Item;

        public MyConnection(ExpdConLocal item)
        {
            this.Item = item;
        }

        public override string ToString()
        {
            List<string> _tmp = new List<string>();
            _tmp.Add("LPF");
            _tmp.Add(this.Item.ToString());
            return string.Join(Config.Config.SEPARATOR, _tmp);
        }

        public MyConnection(List<string> config)
        {
            string _type = config[0];
            config.RemoveAt(0);
            switch (_type)
            {
                case "LPF":
                    this.Item = new ExpdConLocal(config); // List<string> -> Restore LPF
                    break;
                default:
                    this.Item = null;
                    break;
            }
        }
    }

    public class MyConnections : IEnumerable<MyConnection>
    {
        private List<MyConnection> _connections = new List<MyConnection>();
        private MainWindow _main_window = null;
        private Connections _panel = null;

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
