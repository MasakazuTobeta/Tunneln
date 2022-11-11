using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TunnellingMaster.items.dialogs
{
    /// <summary>
    /// InputUsernameAndPassword.xaml の相互作用ロジック
    /// </summary>
    public partial class InputUsernameAndPassword : Window
    {
        public InputUsernameAndPassword()
        {
            InitializeComponent();
        }

        #region ******************************* Change View
        public void setVisible(Visibility uname = Visibility.Visible, Visibility pass = Visibility.Visible)
        {
            this.g_username.Visibility = uname;
            this.g_password.Visibility = pass;
        }

        public void seEnable(bool uname = true, bool pass = true)
        {
            this.username.IsEnabled = uname;
            this.password.IsEnabled = pass;
        }
        #endregion

        #region ******************************* Propaties
        public string Message
        {
            get { return this.message.Text; }
            set { this.message.Text = value; }

        }

        public string Target
        {
            get { return this.target.Text; }
            set { this.target.Text = value; }
        }

        public string Username
        {
            get { return this.username.Text; }
            set { this.username.Text = value; }
        }

        public string Password
        {
            get { return this.password.Password; }
            set { this.password.Password = value; }
        }

        #endregion

        #region ******************************* Callbacks
        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        #endregion

    }
}
