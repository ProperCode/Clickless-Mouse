using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Clickless_Mouse
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowManual : Window
    {
        public WindowManual()
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
