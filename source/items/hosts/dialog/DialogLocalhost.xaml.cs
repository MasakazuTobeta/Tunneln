using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfControlLibrary;

namespace Tunneln.items.hosts.dialog
{
    /// <summary>
    /// DialogLocalhost.xaml の相互作用ロジック
    /// </summary>
    public partial class DialogLocalhost : Window
    {
        public DialogLocalhost(IconLocalhost localhost = null)
        {
            InitializeComponent();
            this.add_virtual_adapter.buttonToggle.Click += this.add_virtual_adapter_Click;
            if (!(localhost is null))
            {
                this.name.Text = localhost.Text;
                this.address.Text = localhost.address;
                this.color = localhost.color;
                this.add_to_hosts_file.IsOn = localhost.hosts_file;
                this.add_virtual_adapter.IsOn = localhost.virtual_adpt;
            }
        }

        public Color color
        {
            get
            {
                return (this.icon_top.Foreground as SolidColorBrush).Color;
            }
            set
            {
                Brush _b = new SolidColorBrush(value);
                this.icon_top.Foreground = _b;
            }
        }

        public bool Verify
        {
            get
            {
                bool is_ok = true;
                IPAddress _ip = Common.GetIPv4Addressl(this.address.Text);
                if (_ip is null)
                {
                    is_ok = false;
                }
                else
                {
                    bool _is_ok = !(this.add_virtual_adapter.IsOn && _ip.ToString().StartsWith("127."));
                    if (!_is_ok)
                    {
                        this.label_add_virtual_adapter.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    is_ok = _is_ok;
                }
                if (!is_ok)
                {
                    this.address.Foreground = new SolidColorBrush(Colors.Red);
                }
                return is_ok;
            }
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void add_virtual_adapter_Click(object sender, RoutedEventArgs e)
        {
            if (this.add_virtual_adapter.IsOn)
            {
                if (this.address.Text.StartsWith("127.") || this.address.Text.Length<=0)
                {
                    this.address.Text = Common.GenerateRandomIPv4Address("169.254.0.0/16");
                }
            }
            else
            {
                this.label_add_virtual_adapter.Foreground = new SolidColorBrush(Colors.Black);
                if (this.address.Text.Length <= 0)
                {
                    this.address.Text = Common.GenerateRandomIPv4Address("127.0.0.0/8");
                }
            }
        }

        private void address_TextChanged(object sender, TextChangedEventArgs e)
        {
            IPAddress _ip = Common.GetIPv4AddressInLocal(this.address.Text);
            if (_ip is null)
            {
                this.address.FontWeight = FontWeights.Normal;
            }
            else
            {
                this.address.FontWeight = FontWeights.Bold;
                this.address.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        #region  ******************************* Add hosts
        private void add_to_hosts_file_Click(object sender, EventArgs e)
        {
            this.Topmost = true;
            if ((e as ToggleSwitch.EventArgsBool).value)
            {
                List<string> exists = this.CheckExistInHostsFile();
                if (exists.Count <= 0)
                {
                    /* Add the name+ip to hosts file */
                    string stdOut;
                    string stdErr;
                    int exitCode;
                    RunCmd(@"echo " + this.address.Text + " " + this.name.Text + @" >> C:\Windows\System32\drivers\etc\hosts", out stdOut, out stdErr, out exitCode, admin:true);
                }
            }
            else
            {
                List<string> _cmd = new List<string>();
                foreach (string exist in this.CheckExistInHostsFile())
                {
                    _cmd.Add(@"resource\grep.bat C:\Windows\System32\drivers\etc\hosts .\.hosts.bak " + exist + " #" + exist);
                }
                if (_cmd.Count > 0)
                {
                    /* Remove the ip from hosts file */
                    string stdOut;
                    string stdErr;
                    int exitCode;
                    RunCmd(string.Join("&&", _cmd) +
                           @"&& copy /Y .\.hosts.bak C:\Windows\System32\drivers\etc\hosts",
                            out stdOut, out stdErr, out exitCode, admin: true, workdir:Directory.GetCurrentDirectory());
                }
            }
            this.Topmost = false;
        }


        private List<string> CheckExistInHostsFile()
        {
            List<string> ret = new List<string>();
            string stdOut;
            string stdErr;
            int exitCode;
            if (RunCmd(@"type C:\Windows\System32\drivers\etc\hosts", out stdOut, out stdErr, out exitCode) == 0)
            {
                foreach (Match _match in Regex.Matches(stdOut, this.address.Text + " "))
                {
                    ret.Add(_match.Value.Replace("\r",""));
                }
            }
            IEnumerable<string> _result = ret.Distinct();
            return _result.ToList(); ;
        }

        /// <summary>
        /// 管理者権限が必要なプログラムを起動する
        /// </summary>
        /// <param name="fileName">プログラムのフルパス。</param>
        /// <param name="arguments">プログラムに渡すコマンドライン引数。</param>
        /// <param name="parentForm">親プログラムのウィンドウ。</param>
        /// <param name="waitExit">起動したプログラムが終了するまで待機する。</param>
        /// <returns>起動に成功した時はtrue。
        /// 「ユーザーアカウント制御」ダイアログでキャンセルされた時はfalse。</returns>
        public static int RunCmd(string executeCommand, out string stdOut, out string stdErr, out int exitCode, bool admin = false, string workdir = null)
        {
            stdOut = "";
            stdErr = "";
            exitCode = 0;

            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.CreateNoWindow = false;
                if (!(workdir is null))
                {
                    executeCommand = "cd \"" + workdir + "\" &&" + executeCommand;
                }
                if (admin)
                {
                    proc.StartInfo.Verb = "runas";
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.RedirectStandardOutput = false;
                    proc.StartInfo.RedirectStandardError = false;
                }
                else
                {
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                }
                proc.StartInfo.Arguments = @"/c " + executeCommand;
                proc.Start();
                proc.WaitForExit();
                if (!admin)
                {
                    stdOut = proc.StandardOutput.ReadToEnd();
                    stdErr = proc.StandardError.ReadToEnd();
                }
                exitCode = proc.ExitCode;
                proc.Close();

                return 0;
            }
            catch(Exception e)
            {
                return 16;
            }
        }

        #endregion
    }
}
