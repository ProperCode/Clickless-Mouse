using System;
using System.Windows;

namespace Clickless_Mouse
{
    /// <summary>
    /// Interaction logic for WindowEULA.xaml
    /// </summary>
    public partial class WindowEULA : Window
    {
        public WindowEULA()
        {
            try
            {
                InitializeComponent();
                TB.IsReadOnly = true;

                TB.Text = "MIT License"
                    + "\n\nClickless Mouse"
+ "\n\nCopyright © 2019 - 2024 Mikołaj Magowski"
+ "\n\n"
+ "Permission is hereby granted, free of charge, to any person obtaining a copy "
+ "of this software and associated documentation files(the \"Software\"), to deal "
+ "in the Software without restriction, including without limitation the rights "
+ "to use, copy, modify, merge, publish, distribute, sublicense, and/ or sell "
+ "copies of the Software, and to permit persons to whom the Software is "
+ "furnished to do so, subject to the following conditions: "
+ "\n\n"
+ "The above copyright notice and this permission notice shall be included in all "
+ "copies or substantial portions of the Software."
+ "\n\n"
+ "THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR "
+ "IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, "
+ "FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE "
+ "AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER "
+ "LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, "
+ "OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE "
+ "SOFTWARE.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error WE001", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
