using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Clickless_Mouse_2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowInstructions : Window
    {
        public WindowInstructions()
        {
            InitializeComponent();
        }

        private void Hyperlink_PreviewMouseUp(object sender, MouseEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            System.Diagnostics.Process.Start(hyperlink.NavigateUri.ToString());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
