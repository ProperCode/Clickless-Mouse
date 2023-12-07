using System;
using System.Windows;

namespace Clickless_Mouse
{
    public partial class WindowChangelog : Window
    {
        public WindowChangelog()
        {
            try
            {
                InitializeComponent();

                TB.IsReadOnly = true;

                TB.Text = "All notable changes to Clickless Mouse will be documented here."
                + "\n\n[2.1] - December 7, 2023:"
                + "\n- Changed: app from now on requires administrator rights to run."
                + "\n- Changed: default \"Cursor time in square to register a click\" is now 100ms."
                + "\n- Changed: improved mouse button holding and releasing."
                +"\n- Other minor improvements.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error WC001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}