using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TunnellingMaster.items
{
    class Setter
    {
        public static void ButtonAdd(Button button)
        {
            button.Content = "✚";
            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.FontSize = 15;
            button.Height = 20;
            button.Width = 20;
            button.Padding = new Thickness(0,0,0,0);
        }
    }
}
