using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Windows.Threading;
using WindowsInput;
using WindowsInput.Native;
using System.Net;
using System.Windows.Media;

namespace Clickless_Mouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Main config
        bool SL_enabled;
        bool SR_enabled;
        bool SM_enabled;
        bool SLH_enabled;
        bool SRH_enabled;
        bool screen_panning;

        //Square config
        const int default_cursor_idle_time_ms = 200;
        const int lowest_cursor_idle_time_ms = 100;
        const int additional_cursor_idle_time = 300; //after click is performed it gives user more
                              //time to start moving mouse without squares blocking top movement path
        const int default_cursor_time_in_square_ms = 100;
        const int lowest_cursor_time_in_square_ms = 10;
        const int default_time_to_start_mouse_movement_ms = 700;
        const int lowest_time_to_start_mouse_movement_ms = 300;
        const int default_size = 50;
        const int lowest_size = 10;
        const int default_border_width = 2;
        const int lowest_border_width = 1;
        const int default_color1 = -16776961;
        const int default_color2 = -256;
        const int default_min_square_size_percents = 60;
        const int lowest_min_square_size_percents = 10;
        const int loop_time_ms = 10; //how often is cursor position checked (10ms recommended)
                                     //changing this requires changing default and lowest:
                                     //cursor_idle_time_ms, time_to_start_mouse_movement_ms
                                     //and cursor_time_in_square_ms

        int cursor_idle_time_ms; //idle time before squares appear
        int loops_to_show_squares_after_cursor_idle;
        int time_to_start_mouse_movement_ms; //time to start mouse movement after squares appear, 
                                         //before they disappear (700 default, lowest reasonable 500)
        int loops_to_start_mouse_movement;
        int cursor_time_in_square_ms; //cursor hover time in square needed to perform a click
        int size;
        int border_width;
        System.Drawing.Color color1 = System.Drawing.Color.FromArgb(default_color1); //square color 1
        System.Drawing.Color color2 = System.Drawing.Color.FromArgb(default_color2); //square color 2
        string square_color1_str;
        string square_color2_str;
        int min_square_size_percents = default_min_square_size_percents; //how much square size can
                                   //be decreased if it would be covered by left or right screen edge
        //----------------------------------

        const string prog_name = "Clickless Mouse";
        const string prog_version = "2.2";
        const string url_latest_version = "https://raw.githubusercontent.com/ProperCode/Clickless-Mouse/master/other/latest_version.txt";
        const string url_homepage = "github.com/ProperCode/Clickless-Mouse";
        string latest_version = "";
        const string copyright_text = "Copyright © 2019 - 2024 Mikołaj Magowski. All rights reserved.";
        string settings_filename = "settings.txt";

        Square SL, SR, SM, SLH, SRH;
        DateTime last_click_time;
        Thread THRmouse_monitor, THRsquares_monitor, THRmouse_monitor2;
        int displacement = 0;
        
        bool saving_enabled = false;
        //full path is necessary if run at startup is used (running at startup uses different current
        //directory
        string app_folder_path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        language lang = language.en;
        bool loading_error = false;
        Process prc;

        NotifyIcon ni = new NotifyIcon();

        InputSimulator sim = new InputSimulator();

        public MainWindow()
        {
            is_program_already_running();

            prc = Process.GetCurrentProcess();
            prc.PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            InitializeComponent();

            Stream iconStream = System.Windows.Application.GetResourceStream(
                new Uri("pack://application:,,,/Clickless Mouse;component/clickless_mouse.ico")).Stream;
            ni.Icon = new System.Drawing.Icon(iconStream);
            iconStream.Close();
            ni.MouseClick += new System.Windows.Forms.MouseEventHandler(ni_MouseClick);

            CultureInfo ci = CultureInfo.InstalledUICulture;

            if (ci.DisplayName.ToLower().Contains("polski") || ci.Name.ToLower().Contains("pl-pl"))
            {
                lang = language.pl;
            }
            else lang = language.en;

            change_language(lang);
            
            Wmain.Title = prog_name + " " + prog_version;
            
            restore_default_settings();

            load_settings();

            fix_wrong_values();

            saving_enabled = true;

            regenerate_squares();            

            if (loading_error)
            {
                save_settings(); //save settings so loading error won't happen again (default values
                                 //will take place of unread values)
            }

            change_language(lang);

            if (CHBstart_minimized.IsChecked == true)
            {
                this.WindowState = WindowState.Minimized;

                if (CHBminimize_to_tray.IsChecked == true)
                {
                    this.Hide();
                    ni.Visible = true;
                }
            }

            CenterWindowOnScreen();

            TBsquare_color1.IsReadOnly = true;
            TBsquare_color2.IsReadOnly = true;

            THRmouse_monitor = new Thread(new ThreadStart(monitor_mouse));
            THRmouse_monitor.Priority = ThreadPriority.Highest;
            THRmouse_monitor.Start();
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        void is_program_already_running()
        {
            Process[] arr = Process.GetProcesses();
            string[] a;
            int i = 0;

            foreach (Process p in arr)
            {
                if (p.ProcessName == prog_name)
                {
                    i++;
                }
            }

            if (i > 1)
            {
                System.Windows.Forms.MessageBox.Show(prog_name + " is already running.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }
        }

        void fix_wrong_values()
        {
            if (cursor_idle_time_ms < lowest_cursor_idle_time_ms)
                TBcursor_idle_before_squares_appear.Text = lowest_cursor_idle_time_ms.ToString();

            if (time_to_start_mouse_movement_ms < lowest_time_to_start_mouse_movement_ms)
                TBtime_to_start_mouse.Text = lowest_time_to_start_mouse_movement_ms.ToString();

            if (cursor_time_in_square_ms < lowest_cursor_time_in_square_ms)
                TBcursor_time_in_square.Text = lowest_cursor_time_in_square_ms.ToString();

            if(size < lowest_size)
                TBsquare_size.Text = lowest_size.ToString();

            if (border_width < lowest_border_width)
                TBsquare_border.Text = lowest_border_width.ToString();

            if (min_square_size_percents < lowest_min_square_size_percents)
            {
                TBmin_square_size.Text = lowest_min_square_size_percents.ToString();
            }
            else if (min_square_size_percents > 100)
            {
                TBmin_square_size.Text = "100";
            }
        }

        void restore_default_settings()
        {
            CHBLMB.IsChecked = true;
            CHBRMB.IsChecked = false;
            CHBdoubleLMB.IsChecked = false;
            CHBholdLMB.IsChecked = false;
            CHBholdRMB.IsChecked = false;
            CHBscreen_panning.IsChecked = false;

            //Checkboxes Checked and Unchecked events work only after form is loaded
            //so they have to be called manually in order to restore settings before form is loaded
            CHBLMB_CheckedChanged(null, null);
            CHBRMB_CheckedChanged(null, null);
            CHBdoubleLMB_CheckedChanged(null, null);
            CHBholdLMB_CheckedChanged(null, null);
            CHBholdRMB_CheckedChanged(null, null);
            CHBscreen_panning_CheckedChanged(null, null);
            CHBcheck_for_updates_CheckedChanged(null, null);

            TBcursor_idle_before_squares_appear.Text = default_cursor_idle_time_ms.ToString();
            TBtime_to_start_mouse.Text = default_time_to_start_mouse_movement_ms.ToString();
            TBcursor_time_in_square.Text = default_cursor_time_in_square_ms.ToString();

            CHBrun_at_startup.IsChecked = false;
            CHBstart_minimized.IsChecked = false;
            CHBminimize_to_tray.IsChecked = false;
            CHBcheck_for_updates.IsChecked = false;

            TBsquare_size.Text = default_size.ToString();
            TBsquare_border.Text = default_border_width.ToString();
            
            square_color1_str = default_color1.ToString();
            square_color2_str = default_color2.ToString();

            int argb = Convert.ToInt32(square_color1_str);

            byte[] values = BitConverter.GetBytes(argb);

            byte a = values[3];
            byte r = values[2];
            byte g = values[1];
            byte b = values[0];

            TBsquare_color1.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            color1 = System.Drawing.Color.FromArgb(a, r, g, b);

            argb = Convert.ToInt32(square_color2_str);

            values = BitConverter.GetBytes(argb);

            a = values[3];
            r = values[2];
            g = values[1];
            b = values[0];

            TBsquare_color2.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            color2 = System.Drawing.Color.FromArgb(a, r, g, b);

            TBmin_square_size.Text = default_min_square_size_percents.ToString();

            TBscreen_size.Text = "";
            int x = Screen.PrimaryScreen.Bounds.Width;
            int y = Screen.PrimaryScreen.Bounds.Height;

            Lscreen_resolution.Content = contentLscreen_resolution + x + "x" + y;
        }

        void regenerate_squares()
        {
            regenerate_SL();
            regenerate_SR();
            regenerate_SM();
            regenerate_SLH();
            regenerate_SRH();
        }

        public int x = 0, y = 0;
        int max_x = Screen.PrimaryScreen.Bounds.Width - 1, 
            max_y = Screen.PrimaryScreen.Bounds.Height - 1;
        public bool squares_visible = false;
        int show_zone;
        int SL_start_x;
        int SL_start_y;
        int SL_end_x;
        int SL_end_y;
        int SR_start_x;
        int SR_start_y;
        int SR_end_x;
        int SR_end_y;
        int SM_start_x;
        int SM_start_y;
        int SM_end_x;
        int SM_end_y;
        int SLH_start_x;
        int SLH_start_y;
        int SLH_end_x;
        int SLH_end_y;
        int SRH_start_x;
        int SRH_start_y;
        int SRH_end_x;
        int SRH_end_y;
        
        void monitor_mouse2()
        {
            //Thread.Sleep(5000); //debug only

            int x1, y1;
            bool pressed_up, pressed_left, pressed_down, pressed_right;
            pressed_up = pressed_left = pressed_down = pressed_right = false;

            while (true)
            {
                //user may change screen resolution so max_x and max_y should be updated
                max_x = Screen.PrimaryScreen.Bounds.Width - 1;
                max_y = Screen.PrimaryScreen.Bounds.Height - 1;

                x1 = System.Windows.Forms.Cursor.Position.X;
                y1 = System.Windows.Forms.Cursor.Position.Y;

                if (x1 == 0) //if (x1 == 0 && pressed_left == false) would be a mistake (we need
                            //continous pressing as well as holding)
                {
                    key_down(VirtualKeyCode.LEFT);
                    pressed_left = true;
                }
                else if (x1 != 0 && pressed_left == true)
                {
                    key_up(VirtualKeyCode.LEFT);
                    pressed_left = false;
                }

                if (x1 == max_x)
                {
                    key_down(VirtualKeyCode.RIGHT);
                    pressed_right = true;
                }
                else if (x1 != max_x && pressed_right == true)
                {
                    key_up(VirtualKeyCode.RIGHT);
                    pressed_right = false;
                }

                if (y1 == 0)
                {
                    key_down(VirtualKeyCode.UP);
                    pressed_up = true;
                }
                else if (y1 != 0 && pressed_up == true)
                {
                    key_up(VirtualKeyCode.UP);
                    pressed_up = false;
                }

                if (y1 == max_y)
                {
                    key_down(VirtualKeyCode.DOWN);
                    pressed_down = true;
                }
                else if (y1 != max_y && pressed_down == true)
                {
                    key_down(VirtualKeyCode.DOWN);
                    pressed_down = false;
                }

                real_sleep(50);
            }
        }

        void calculate_squares_start_positions()
        {
            displacement = (int)(size / 2);
            show_zone = size + displacement;

            SL_start_x = x - show_zone;
            SL_start_y = y - show_zone;
            SL_end_x = SL_start_x + size;
            SL_end_y = SL_start_y + size;

            SR_start_x = x + displacement;
            SR_start_y = y - show_zone;
            SR_end_x = SR_start_x + size;
            SR_end_y = SR_start_y + size;

            SM_start_x = x - displacement;
            SM_start_y = y - 2 * size;
            SM_end_x = SM_start_x + size;
            SM_end_y = SM_start_y + size;

            SLH_start_x = x - 2 * size;
            SLH_start_y = y - displacement;
            SLH_end_x = SLH_start_x + size;
            SLH_end_y = SLH_start_y + size;

            SRH_start_x = x + size;
            SRH_start_y = y - displacement;
            SRH_end_x = SRH_start_x + size;
            SRH_end_y = SRH_start_y + size;
        }

        int banned_x = -1;
        int banned_y = -1;
        int previous_size = 0;
        void monitor_mouse()
        {
            int i = 0;
            int x1 = 0, x2 = 0, y1 = 0, y2 = 0;

            while (true)
            {
                x1 = System.Windows.Forms.Cursor.Position.X;
                y1 = System.Windows.Forms.Cursor.Position.Y;
                Thread.Sleep(loop_time_ms);
                x2 = System.Windows.Forms.Cursor.Position.X;
                y2 = System.Windows.Forms.Cursor.Position.Y;

                //max_x and max_y are updated in monitor_mouse2 by THRmouse_monitor2 which works
                //only when screen_panning == true
                if (screen_panning && (x2 == 0 || x2 == max_x || y2 == 0 || y2 == max_y))
                {
                    banned_x = -1;
                    banned_y = -1;
                    i = 0;
                    mouse_move_detected();
                }
                else if (x1 == x2 && y1 == y2)
                {
                    if (x1 != banned_x || y1 != banned_y)
                    {
                        i++;
                    }
                }
                else
                {
                    banned_x = -1;
                    banned_y = -1;
                    i = 0;
                    mouse_move_detected();
                }
                if (i > loops_to_show_squares_after_cursor_idle && squares_visible == false)
                {
                    x = x2;
                    y = y2;
                    
                    TimeSpan time_elapsed_since_last_click = new TimeSpan();
                    if (last_click_time != null)
                    {
                        time_elapsed_since_last_click = DateTime.Now - last_click_time;
                    }

                    if (last_click_time != null 
                        && time_elapsed_since_last_click.TotalMilliseconds > 
                        cursor_idle_time_ms + additional_cursor_idle_time)
                    {
                        int original_size = size;
                        int minimum_size = 
                            (int)Math.Round((double)original_size * min_square_size_percents/100);

                        calculate_squares_start_positions();
                        
                        int screen_width = Screen.PrimaryScreen.Bounds.Width;

                        //if SLH is visible when at minimum_size and 80% or more of SLH 
                        //is out of left screen edge
                        if (SLH_enabled && x > minimum_size && SLH_start_x <= -1 * size * 0.8)
                        {
                            //decrease square size so at least 25% is visible, but square size >= minimum_size
                            size = (int)(x / 1.25);

                            if (size < minimum_size)
                            {
                                size = minimum_size;
                            }
                        }
                        //if SL is visible when at minimum_size and 80% or more of SL 
                        //is out of left screen edge
                        else if (SL_enabled && x > minimum_size / 2 && SL_start_x <= -1 * size * 0.8)
                        {
                            //decrease square size so at least 25% is visible, but square size >= minimum_size
                            size = (int)(x / 0.75);

                            if (size < minimum_size)
                            {
                                size = minimum_size;
                            }
                        }
                        //if SRH is visible when at minimum_size and 80% or more of SRH 
                        //is out of left screen edge
                        else if (SRH_enabled && x < (screen_width - 1) - minimum_size
                            && SRH_start_x >= (screen_width - 1) - size * 0.2)
                        {
                            //decrease square size so at least 25% is visible, but square size >= minimum_size
                            size = (int)(((screen_width - 1) - x) / 1.25);

                            if (size < minimum_size)
                            {
                                size = minimum_size;
                            }
                        }
                        //if SR is visible when at minimum_size and 80% or more of SR
                        //is out of left screen edge
                        else if (SR_enabled && x < (screen_width - 1) - 0.5 * minimum_size
                            && SR_start_x >= (screen_width - 1) - size * 0.2)
                        {
                            //decrease square size so at least 25% is visible, but square size >= minimum_size
                            size = (int)(((screen_width - 1) - x) / 0.75);

                            if (size < minimum_size)
                            {
                                size = minimum_size;
                            }
                        }

                        if (original_size != size)
                        {
                            calculate_squares_start_positions();
                            regenerate_squares();
                        }
                        else if(previous_size != size)
                        {
                            regenerate_squares();
                        }

                        //if top screen edge would cover squares show them below mouse cursor instead
                        if (SM_enabled && SM_start_y < -1 * size * 0.75 || (SM_enabled == false
                            && (SL_enabled || SR_enabled) && SL_start_y < -1 * size * 0.75))
                        {
                            SL_start_y = y + displacement;
                            SL_end_y = SL_start_y + size;

                            SR_start_y = y + displacement;
                            SR_end_y = SR_start_y + size;

                            SM_start_y = y + size;
                            SM_end_y = SM_start_y + size;
                        }

                        bool mi_file_open = false;
                        bool mi_restore_open = false;
                        bool mi_language_open = false;
                        bool mi_help_open = false;
                        bool is_this_focused = false;
                        bool is_instructions_focused = false;

                        this.MIfile.Dispatcher.Invoke(DispatcherPriority.Normal, 
                            new Action(() => { mi_file_open = MIfile.IsSubmenuOpen; }));
                        this.MIrestore.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { mi_restore_open = MIrestore.IsSubmenuOpen; }));
                        this.MIlanguage.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { mi_language_open = MIlanguage.IsSubmenuOpen; }));
                        this.MIhelp.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { mi_help_open = MIhelp.IsSubmenuOpen; }));
                        
                        this.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { is_this_focused = this.IsActive; }));
                        wm.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { is_instructions_focused = wm.IsActive; }));

                        if (SL_enabled)
                            show_SL(true);
                        if (SR_enabled)
                            show_SR(true);
                        if (SM_enabled)
                            show_SM(true);
                        if (SLH_enabled)
                            show_SLH(true);
                        if (SRH_enabled)
                            show_SRH(true);

                        squares_visible = true;

                        //reopen submenu that was closed because squares appeared
                        if (mi_file_open)
                            this.MIfile.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { MIfile.IsSubmenuOpen = mi_file_open; }));
                        if (mi_restore_open)
                            this.MIrestore.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { MIrestore.IsSubmenuOpen = mi_restore_open; }));
                        if (mi_language_open)
                            this.MIlanguage.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { MIlanguage.IsSubmenuOpen = mi_language_open; }));
                        if (mi_help_open)
                            this.MIhelp.Dispatcher.Invoke(DispatcherPriority.Normal,
                            new Action(() => { MIhelp.IsSubmenuOpen = mi_help_open; }));

                        //give back stolen focus (by squares) to a Window if it
                        //was focused before they appeared
                        if (is_this_focused)
                        {
                            this.Dispatcher.Invoke(DispatcherPriority.Normal,
                                new Action(() => { this.Focus(); }));
                        }
                        else if(is_instructions_focused)
                        {
                            wm.Dispatcher.Invoke(DispatcherPriority.Normal,
                                new Action(() => { wm.Focus(); }));
                        }

                        THRsquares_monitor = new Thread(new ThreadStart(monitor_squares));
                        THRsquares_monitor.Priority = ThreadPriority.Highest;
                        THRsquares_monitor.Start();
                        i = 0;

                        previous_size = size;
                        size = original_size;
                    }                    
                }
                else if (i > loops_to_start_mouse_movement && squares_visible)
                {
                    THRsquares_monitor.Abort();
                    squares_visible = false;
                    if (SL_enabled)
                        show_SL(false);
                    if (SR_enabled)
                        show_SR(false);
                    if (SM_enabled)
                        show_SM(false);
                    if (SLH_enabled)
                        show_SLH(false);
                    if (SRH_enabled)
                        show_SRH(false);
                    i = 0;
                    banned_x = x;
                    banned_y = y;
                }
            }
        }

        void monitor_squares()
        {
            int i_SL = 0, i_SR = 0, i_SM = 0, i_SLH = 0, i_SRH = 0;
            int i_max = cursor_time_in_square_ms / loop_time_ms;
            int pos_x, pos_y;

            while (i_SL < i_max && i_SR < i_max && i_SM < i_max
                && i_SLH < i_max && i_SRH < i_max && squares_visible)
            {
                pos_x = System.Windows.Forms.Cursor.Position.X;
                pos_y = System.Windows.Forms.Cursor.Position.Y;

                if (SL_enabled)
                {
                    if (is_cursor_in_SL(pos_x, pos_y))
                    {
                        i_SL++;
                    }
                    else i_SL = 0;
                }
                if (SR_enabled)
                {
                    if (is_cursor_in_SR(pos_x, pos_y))
                    {
                        i_SR++;
                    }
                    else i_SR = 0;
                }
                if (SM_enabled)
                {
                    if (is_cursor_in_SM(pos_x, pos_y))
                    {
                        i_SM++;
                    }
                    else i_SM = 0;
                }
                if (SLH_enabled)
                {
                    if (is_cursor_in_SLH(pos_x, pos_y))
                    {
                        i_SLH++;
                    }
                    else i_SLH = 0;
                }
                if (SRH_enabled)
                {
                    if (is_cursor_in_SRH(pos_x, pos_y))
                    {
                        i_SRH++;
                    }
                    else i_SRH = 0;
                }
                Thread.Sleep(loop_time_ms);
            }
            if (i_SL >= i_max)
            {
                LMBClick(x, y, 100);
                if (SL_enabled)
                    show_SL(false);
                if (SR_enabled)
                    show_SR(false);
                if (SM_enabled)
                    show_SM(false);
                if (SLH_enabled)
                    show_SLH(false);
                if (SRH_enabled)
                    show_SRH(false);
                last_click_time = DateTime.Now;
                squares_visible = false;
            }
            else if (i_SR >= i_max)
            {
                RMBClick(x, y, 100);
                if (SL_enabled)
                    show_SL(false);
                if (SR_enabled)
                    show_SR(false);
                if (SM_enabled)
                    show_SM(false);
                if (SLH_enabled)
                    show_SLH(false);
                if (SRH_enabled)
                    show_SRH(false);
                last_click_time = DateTime.Now;
                squares_visible = false;
            }
            else if (i_SM >= i_max)
            {
                DLMBClick(x, y, 100);
                if (SL_enabled)
                    show_SL(false);
                if (SR_enabled)
                    show_SR(false);
                if (SM_enabled)
                    show_SM(false);
                if (SLH_enabled)
                    show_SLH(false);
                if (SRH_enabled)
                    show_SRH(false);
                last_click_time = DateTime.Now;
                squares_visible = false;
            }
            if (i_SLH >= i_max)
            {
                LMBHold(x, y, 100);
                if (SL_enabled)
                    show_SL(false);
                if (SR_enabled)
                    show_SR(false);
                if (SM_enabled)
                    show_SM(false);
                if (SLH_enabled)
                    show_SLH(false);
                if (SRH_enabled)
                    show_SRH(false);
                last_click_time = DateTime.Now;
                squares_visible = false;
            }
            else if (i_SRH >= i_max)
            {
                RMBHold(x, y, 100);
                if (SL_enabled)
                    show_SL(false);
                if (SR_enabled)
                    show_SR(false);
                if (SM_enabled)
                    show_SM(false);
                if (SLH_enabled)
                    show_SLH(false);
                if (SRH_enabled)
                    show_SRH(false);
                last_click_time = DateTime.Now;
                squares_visible = false;
            }
        }

        private void mouse_move_detected()
        {
            if (squares_visible)
            {
                int x1 = System.Windows.Forms.Cursor.Position.X, 
                    y1 = System.Windows.Forms.Cursor.Position.Y;
                if (is_cursor_outside_zone(x1, y1))
                {
                    if (SL_enabled)
                        show_SL(false);
                    if (SR_enabled)
                        show_SR(false);
                    if (SM_enabled)
                        show_SM(false);
                    if (SLH_enabled)
                        show_SLH(false);
                    if (SRH_enabled)
                        show_SRH(false);
                    squares_visible = false;
                }
            }
        }

        bool is_cursor_in_SL(int x1, int y1)
        {
            if (x1 >= SL_start_x && x1 <= SL_end_x
                && y1 >= SL_start_y && y1 <= SL_end_y)
            {
                return true;
            }
            else return false;
        }

        bool is_cursor_in_SR(int x1, int y1)
        {
            if (x1 >= SR_start_x && x1 <= SR_end_x
                && y1 >= SR_start_y && y1 <= SR_end_y)
            {
                return true;
            }
            else return false;
        }

        bool is_cursor_in_SM(int x1, int y1)
        {
            if (x1 >= SM_start_x && x1 <= SM_end_x
                && y1 >= SM_start_y && y1 <= SM_end_y)
            {
                return true;
            }
            else return false;
        }

        bool is_cursor_in_SLH(int x1, int y1)
        {
            if (x1 >= SLH_start_x && x1 <= SLH_end_x
                && y1 >= SLH_start_y && y1 <= SLH_end_y)
            {
                return true;
            }
            else return false;
        }

        bool is_cursor_in_SRH(int x1, int y1)
        {
            if (x1 >= SRH_start_x && x1 <= SRH_end_x
                && y1 >= SRH_start_y && y1 <= SRH_end_y)
            {
                return true;
            }
            else return false;
        }

        bool is_cursor_outside_zone(int x1, int y1)
        {
            //if (SM_enabled == false)
            //{
            //    if (x1 > x + show_zone || x1 < x - show_zone || y1 > y + show_zone || y1 < y - show_zone)
            //    {
            //        return true;
            //    }
            //    else return false;
            //}
            //else
            //{
            //    if (x1 > x + show_zone || x1 < x - show_zone || y1 > y + show_zone || y1 < y - show_zone - (int)(size / 2))
            //    {
            //        return true;
            //    }
            //    else return false;
            //}

            //temp solution
            if (x1 > x + show_zone + displacement || x1 < x - show_zone - displacement
                || y1 > y + show_zone || y1 < y - show_zone - displacement)
            {
                return true;
            }
            else return false;
        }

        void real_sleep(int time)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            do
            {
                Thread.Sleep(10);
            }
            while (stopwatch.ElapsedMilliseconds < time);
            stopwatch.Stop();
        }

        void freeze_mouse(int X, int Y, int time)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            do
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(X, Y);
                Thread.Sleep(1);
            }
            while (stopwatch.ElapsedMilliseconds < time);
        }

        delegate void Callback1(bool show);

        void show_SL(bool show)
        {
            if (SL.InvokeRequired)
            {
                try
                {
                    Callback1 d = new Callback1(show_SL);
                    SL.Invoke(d, new object[] { show });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (show)
                {
                    SL.Location = new System.Drawing.Point(SL_start_x, SL_start_y);
                    SL.Show();
                }
                else SL.Hide();
            }
        }
        void show_SR(bool show)
        {
            if (SR.InvokeRequired)
            {
                try
                {
                    Callback1 d = new Callback1(show_SR);
                    SR.Invoke(d, new object[] { show });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (show)
                {
                    SR.Location = new System.Drawing.Point(SR_start_x, SR_start_y);
                    SR.Show();
                }
                else SR.Hide();
            }
        }
        void show_SM(bool show)
        {
            if (SM.InvokeRequired)
            {
                try
                {
                    Callback1 d = new Callback1(show_SM);
                    SM.Invoke(d, new object[] { show });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (show)
                {
                    SM.Location = new System.Drawing.Point(SM_start_x, SM_start_y);
                    SM.Show();
                }
                else SM.Hide();
            }
        }
        void show_SLH(bool show)
        {
            if (SLH.InvokeRequired)
            {
                try
                {
                    Callback1 d = new Callback1(show_SLH);
                    SLH.Invoke(d, new object[] { show });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (show)
                {
                    SLH.Location = new System.Drawing.Point(SLH_start_x, SLH_start_y);
                    SLH.Show();
                }
                else SLH.Hide();
            }
        }
        void show_SRH(bool show)
        {
            if (SRH.InvokeRequired)
            {
                try
                {
                    Callback1 d = new Callback1(show_SRH);
                    SRH.Invoke(d, new object[] { show });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (show)
                {
                    SRH.Location = new System.Drawing.Point(SRH_start_x, SRH_start_y);
                    SRH.Show();
                }
                else SRH.Hide();
            }
        }

        delegate void Callback2();

        void regenerate_SL()
        {
            if (SL != null && SL.InvokeRequired)
            {
                try
                {
                    Callback2 d = new Callback2(regenerate_SL);
                    SL.Invoke(d, new object[] { });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (SL != null)
                    SL.Dispose();

                SL = new Square(size, border_width, color1, color2);
                SL.TopMost = true;
                SL.Show();
                SL.Height = size;
                SL.Width = size;

                SL.Hide();
            }
        }

        void regenerate_SR()
        {
            if (SR != null && SR.InvokeRequired)
            {
                try
                {
                    Callback2 d = new Callback2(regenerate_SR);
                    SR.Invoke(d, new object[] { });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (SR != null)
                    SR.Dispose();

                SR = new Square(size, border_width, color1, color2);
                SR.TopMost = true;
                SR.Show();
                SR.Height = size;
                SR.Width = size;

                SR.Hide();
            }
        }

        void regenerate_SM()
        {
            if (SM != null && SM.InvokeRequired)
            {
                try
                {
                    Callback2 d = new Callback2(regenerate_SM);
                    SM.Invoke(d, new object[] { });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (SM != null)
                    SM.Dispose();

                SM = new Square(size, border_width, color1, color2);
                SM.TopMost = true;
                SM.Show();
                SM.Height = size;
                SM.Width = size;

                SM.Hide();
            }
        }

        void regenerate_SLH()
        {
            if (SLH != null && SLH.InvokeRequired)
            {
                try
                {
                    Callback2 d = new Callback2(regenerate_SLH);
                    SLH.Invoke(d, new object[] { });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (SLH != null)
                    SLH.Dispose();

                SLH = new Square(size, border_width, color1, color2);
                SLH.TopMost = true;
                SLH.Show();
                SLH.Height = size;
                SLH.Width = size;

                SLH.Hide();
            }
        }

        void regenerate_SRH()
        {
            if (SRH != null && SRH.InvokeRequired)
            {
                try
                {
                    Callback2 d = new Callback2(regenerate_SRH);
                    SRH.Invoke(d, new object[] { });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                if (SRH != null)
                    SRH.Dispose();

                SRH = new Square(size, border_width, color1, color2);
                SRH.TopMost = true;
                SRH.Show();
                SRH.Height = size;
                SRH.Width = size;

                SRH.Hide();
            }
        }

        //----------------------------------------------------------------------------------

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private void Wmain_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized && CHBminimize_to_tray.IsChecked == true)
            {
                this.Hide();
                ni.Visible = true;
            }
        }

        void ni_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ni.Visible = false;
            Show();
            this.WindowState = WindowState.Normal;
            SetForegroundWindow(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MIexit_Click(null, null);
        }

        private void MIexit_Click(object sender, RoutedEventArgs e)
        {
            ni.Visible = false;
            ni.Dispose();

            release_buttons_and_keys();
            Process.GetCurrentProcess().Kill();
        }

        private void MIdefault_colors_Click(object sender, RoutedEventArgs e)
        {
            saving_enabled = false; //to avoid multiple saves

            square_color1_str = default_color1.ToString();
            square_color2_str = default_color2.ToString();

            int argb = Convert.ToInt32(square_color1_str);

            byte[] values = BitConverter.GetBytes(argb);

            byte a = values[3];
            byte r = values[2];
            byte g = values[1];
            byte b = values[0];

            TBsquare_color1.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            color1 = System.Drawing.Color.FromArgb(a, r, g, b);

            argb = Convert.ToInt32(square_color2_str);

            values = BitConverter.GetBytes(argb);

            a = values[3];
            r = values[2];
            g = values[1];
            b = values[0];

            TBsquare_color2.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            color2 = System.Drawing.Color.FromArgb(a, r, g, b);

            regenerate_squares();

            saving_enabled = true;

            save_settings();
        }

        private void MIdefault_settings_Click(object sender, RoutedEventArgs e)
        {
            saving_enabled = false; //to avoid multiple saves

            restore_default_settings();

            regenerate_squares();

            saving_enabled = true;
            save_settings();
        }

        WindowManual wm = new WindowManual();

        private void MIenglish_Click(object sender, RoutedEventArgs e)
        {
            lang = language.en;
            change_language(lang);
            save_settings();
        }

        private void MIpolish_Click(object sender, RoutedEventArgs e)
        {
            lang = language.pl;
            change_language(lang);
            save_settings();
        }

        private void MImanual_Click(object sender, RoutedEventArgs e)
        {
            wm.Show();
        }

        private void MIabout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content;
                MyWebClient wc = new MyWebClient();
                content = wc.DownloadString(url_latest_version);

                latest_version = content.Replace("\r\n", "").Trim();
            }
            catch (WebException we)
            {
                latest_version = "unknown";
            }

            try
            {
                WindowAbout w = new WindowAbout();

                w.Lprogram_name.Content = prog_name;
                w.Llatest_version.Content = "Latest version: " + latest_version;
                w.Linstalled_version.Content = "Installed version: " + prog_version;
                w.Lhomepage.Content = url_homepage;
                w.Lcopyright.Content = copyright_text;
                
                w.Show();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CHBLMB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (saving_enabled)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBLMB.IsChecked == true)
                SL_enabled = true;
            else
                SL_enabled = false;

            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void CHBRMB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (saving_enabled)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBRMB.IsChecked == true)
                SR_enabled = true;
            else
                SR_enabled = false;

            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void CHBdoubleLMB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (saving_enabled)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBdoubleLMB.IsChecked == true)
                SM_enabled = true;
            else
                SM_enabled = false;

            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void CHBholdLMB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (saving_enabled)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBholdLMB.IsChecked == true)
                SLH_enabled = true;
            else
                SLH_enabled = false;

            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void CHBholdRMB_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (saving_enabled)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBholdRMB.IsChecked == true)
                SRH_enabled = true;
            else
                SRH_enabled = false;

            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void CHBscreen_panning_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (CHBscreen_panning.IsChecked == true && THRmouse_monitor2 == null)
            {
                THRmouse_monitor2 = new Thread(new ThreadStart(monitor_mouse2));
                THRmouse_monitor2.Priority = ThreadPriority.Highest;
                THRmouse_monitor2.Start();
                screen_panning = true;
            }
            else if (CHBscreen_panning.IsChecked == false && THRmouse_monitor2 != null)
            {
                screen_panning = false;
                THRmouse_monitor2.Abort();
                THRmouse_monitor2 = null;
            }

            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void CHBcheck_for_updates_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if((bool)CHBcheck_for_updates.IsChecked)
            {
                update_app_if_necessary();
            }

            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void TBcursor_idle_before_squares_appear_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TBcursor_idle_before_squares_appear.Text.Length > 0)
                {
                    int x = int.Parse(TBcursor_idle_before_squares_appear.Text);
                    if (x < 1)
                    {
                        TBcursor_idle_before_squares_appear.Text = "1";
                        throw new Exception(cursor_idle_time_error + "1 ms");
                    }

                    cursor_idle_time_ms = x;
                    loops_to_show_squares_after_cursor_idle = (int)Math.Round(
                        (double)(cursor_idle_time_ms / loop_time_ms));

                    if (cursor_idle_time_ms < lowest_cursor_idle_time_ms)
                    {
                        cursor_idle_time_ms = lowest_cursor_idle_time_ms;
                        loops_to_show_squares_after_cursor_idle =
                            (int)Math.Round((double)(lowest_cursor_idle_time_ms / loop_time_ms));
                    }

                    if (saving_enabled)
                    {
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TBtime_to_start_mouse_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TBtime_to_start_mouse.Text.Length > 0)
                {
                    int x = int.Parse(TBtime_to_start_mouse.Text);
                    if (x < 1)
                    {
                        TBtime_to_start_mouse.Text = "1";
                        throw new Exception(time_to_start_mouse_movement_error + "1 ms.");
                    }

                    time_to_start_mouse_movement_ms = x;
                    loops_to_start_mouse_movement = (int)Math.Round(
                        (double)time_to_start_mouse_movement_ms / loop_time_ms);

                    if (time_to_start_mouse_movement_ms < lowest_time_to_start_mouse_movement_ms)
                    {
                        time_to_start_mouse_movement_ms = lowest_time_to_start_mouse_movement_ms;
                        loops_to_start_mouse_movement = (int)Math.Round(
                            (double)(lowest_time_to_start_mouse_movement_ms / loop_time_ms));
                    }

                    if (saving_enabled)
                    {
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TBcursor_time_in_square_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TBcursor_time_in_square.Text.Length > 0)
                {
                    int x = int.Parse(TBcursor_time_in_square.Text);
                    if (x < 1)
                    {
                        TBcursor_time_in_square.Text = "1";
                        throw new Exception(cursor_time_in_square_error + "1 ms.");
                    }

                    cursor_time_in_square_ms = x;

                    if (cursor_time_in_square_ms < lowest_cursor_time_in_square_ms)
                        cursor_time_in_square_ms = lowest_cursor_time_in_square_ms;

                    if (saving_enabled)
                    {
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CHBrun_at_startup_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (saving_enabled)
            {
                //need a .bat file to start an .exe file for some reasons
                Microsoft.Win32.RegistryKey rkApp = Microsoft.Win32.Registry.CurrentUser
                    .OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (CHBrun_at_startup.IsChecked == true)
                {
                    if (rkApp.GetValue(prog_name) == null)
                    {
                        rkApp.SetValue(prog_name,
                            System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".exe", ".vbs"));
                    }
                    generate_bat_file();
                }
                else if (rkApp.GetValue(prog_name) != null)
                {
                    rkApp.DeleteValue(prog_name, false);
                }

                save_settings();
            }
        }

        void generate_bat_file()
        {
            FileStream fs = null;
            StreamWriter sw = null;
            string file_path = System.IO.Path.Combine(
                System.Reflection.Assembly.GetExecutingAssembly().Location.
                Replace(".exe", ".vbs"));

            try
            {
                fs = new FileStream(file_path, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);

                //cmd script (black window appearing for a moment is a problem)
                //sw.WriteLine("cd \"" + app_folder_path + "\"");
                //sw.WriteLine("start " 
                //    + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName);

                //vbs script (no window appearing during execution)
                sw.WriteLine("Set objShell = CreateObject(\"Wscript.Shell\")");
                sw.WriteLine("objShell.CurrentDirectory = \"" + app_folder_path + "\"");
                sw.WriteLine("strApp = \"\"\""
                    + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName + "\"\"\"");
                sw.WriteLine("objShell.Run(strApp)");

                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    if (sw != null)
                        sw.Close();
                    if (fs != null)
                        fs.Close();
                }
                catch (Exception ex2) { }
            }
        }
        private void CHBstart_minimized_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void CHBminimize_to_tray_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (saving_enabled)
            {
                save_settings();
            }
        }

        private void TBsquare_size_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TBsquare_size.Text.Length > 0)
                {
                    int x = int.Parse(TBsquare_size.Text);
                    if (x < 1)
                    {
                        TBsquare_size.Text = "1";
                        throw new Exception(square_size_error + "1 px.");
                    }

                    size = x;

                    if (size < lowest_size)
                        size = lowest_size;

                    if (saving_enabled)
                    {
                        regenerate_squares();
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TBsquare_border_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TBsquare_border.Text.Length > 0)
                {
                    int x = int.Parse(TBsquare_border.Text);
                    if (x < 1)
                    {
                        TBsquare_border.Text = "1";
                        throw new Exception(square_border_error + "1 px.");
                    }

                    border_width = x;

                    if (saving_enabled)
                    {
                        regenerate_squares();
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ColorDialog colorDialog1 = new ColorDialog();        

        ColorDialog colorDialog2 = new ColorDialog();

        private void TBsquare_color1_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Forms.DialogResult dr = colorDialog1.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    int argb = Convert.ToInt32(colorDialog1.Color.ToArgb().ToString());

                    byte[] values = BitConverter.GetBytes(argb);

                    byte a = values[3];
                    byte r = values[2];
                    byte g = values[1];
                    byte b = values[0];

                    TBsquare_color1.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
                    color1 = System.Drawing.Color.FromArgb(a, r, g, b);

                    square_color1_str = argb.ToString();

                    if (saving_enabled)
                    {
                        regenerate_squares();
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TBsquare_color2_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Windows.Forms.DialogResult dr = colorDialog2.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    int argb = Convert.ToInt32(colorDialog2.Color.ToArgb().ToString());

                    byte[] values = BitConverter.GetBytes(argb);

                    byte a = values[3];
                    byte r = values[2];
                    byte g = values[1];
                    byte b = values[0];

                    TBsquare_color2.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
                    color2 = System.Drawing.Color.FromArgb(a, r, g, b);

                    square_color2_str = argb.ToString();

                    if (saving_enabled)
                    {
                        regenerate_squares();
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TBmin_square_size_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (TBmin_square_size.Text.Length > 0)
                {
                    int x = int.Parse(TBmin_square_size.Text);
                    if (x < 1)
                    {
                        TBmin_square_size.Text = "1";
                        throw new Exception(min_square_size_too_low_error + "1%.");
                    }
                    else if (x > 100)
                    {
                        TBmin_square_size.Text = "100";
                        throw new Exception(min_square_size_too_high_error + "100%.");
                    }

                    min_square_size_percents = x;

                    if (min_square_size_percents < lowest_min_square_size_percents)
                    {
                        min_square_size_percents = lowest_min_square_size_percents;
                    }

                    if (saving_enabled)
                    {
                        save_settings();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Click_Bset_recommended_square(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TBscreen_size.Text.Length == 0)
                {
                    throw new Exception(screen_size_error2);
                }
                else
                {
                    int d = int.Parse(TBscreen_size.Text);
                    if (d < 1)
                        throw new Exception(screen_size_error1 + ".");

                    int x = Screen.PrimaryScreen.Bounds.Width;
                    int y = Screen.PrimaryScreen.Bounds.Height;

                    Lscreen_resolution.Content = contentLscreen_resolution + x + "x" + y;

                    double b = Math.Sqrt(Math.Pow(d, 2)/(Math.Pow(x, 2) / Math.Pow(y, 2) + 1));
                    double a = b * x / y;

                    double area =  a * b;
                    double pixel_size_mm = area / (x * y) * Math.Pow(25.4, 2);

                    TBsquare_size.Text = Math.Round(50 * 0.0771 / pixel_size_mm).ToString();
                    TBsquare_border.Text = Math.Round(2 * 0.06939 / pixel_size_mm).ToString();

                    regenerate_squares();
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void save_settings()
        {
            FileStream fs = null;
            StreamWriter sw = null;
            string file_path = System.IO.Path.Combine(app_folder_path, settings_filename);

            try
            {
                fs = new FileStream(file_path, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);

                sw.WriteLine(CHBLMB.IsChecked);
                sw.WriteLine(CHBRMB.IsChecked);
                sw.WriteLine(CHBdoubleLMB.IsChecked);
                sw.WriteLine(CHBholdLMB.IsChecked);
                sw.WriteLine(CHBholdRMB.IsChecked);
                sw.WriteLine(CHBscreen_panning.IsChecked);
                sw.WriteLine(TBcursor_idle_before_squares_appear.Text);
                sw.WriteLine(TBtime_to_start_mouse.Text);
                sw.WriteLine(TBcursor_time_in_square.Text);

                sw.WriteLine(CHBrun_at_startup.IsChecked);
                sw.WriteLine(CHBstart_minimized.IsChecked);
                sw.WriteLine(CHBminimize_to_tray.IsChecked);
                
                sw.WriteLine(TBsquare_size.Text);
                sw.WriteLine(TBsquare_border.Text);
                sw.WriteLine(square_color1_str);
                sw.WriteLine(square_color2_str);
                sw.WriteLine(TBmin_square_size.Text);                

                sw.WriteLine(TBscreen_size.Text);

                sw.WriteLine(lang.ToString());

                sw.WriteLine(CHBcheck_for_updates.IsChecked);

                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    if (sw != null)
                        sw.Close();
                    if (fs != null)
                        fs.Close();
                }
                catch (Exception ex2) { }
            }
        }

        private void load_settings()
        {
            FileStream fs = null;
            StreamReader sr = null;
            string file_path = System.IO.Path.Combine(app_folder_path, settings_filename);

            try
            {
                if (File.Exists(file_path))
                {
                    //Checkboxes Checked and Unchecked events work only after form is loaded
                    //so they have to be called manually in order to load save data properly
                    CHBLMB_CheckedChanged(null, null);
                    CHBRMB_CheckedChanged(null, null);
                    CHBdoubleLMB_CheckedChanged(null, null);
                    CHBholdLMB_CheckedChanged(null, null);
                    CHBholdRMB_CheckedChanged(null, null);
                    CHBscreen_panning_CheckedChanged(null, null);
                    CHBcheck_for_updates_CheckedChanged(null, null);

                    fs = new FileStream(file_path, FileMode.Open, FileAccess.Read);
                    sr = new StreamReader(fs);

                    CHBLMB.IsChecked = bool.Parse(sr.ReadLine());
                    CHBRMB.IsChecked = bool.Parse(sr.ReadLine());
                    CHBdoubleLMB.IsChecked = bool.Parse(sr.ReadLine());
                    CHBholdLMB.IsChecked = bool.Parse(sr.ReadLine());
                    CHBholdRMB.IsChecked = bool.Parse(sr.ReadLine());
                    CHBscreen_panning.IsChecked = bool.Parse(sr.ReadLine());
                    TBcursor_idle_before_squares_appear.Text = sr.ReadLine();
                    TBtime_to_start_mouse.Text = sr.ReadLine();
                    TBcursor_time_in_square.Text = sr.ReadLine();
                    
                    CHBrun_at_startup.IsChecked = bool.Parse(sr.ReadLine());
                    CHBstart_minimized.IsChecked = bool.Parse(sr.ReadLine());
                    CHBminimize_to_tray.IsChecked = bool.Parse(sr.ReadLine());

                    TBsquare_size.Text = sr.ReadLine();
                    TBsquare_border.Text = sr.ReadLine();
                    square_color1_str = sr.ReadLine();
                    square_color2_str = sr.ReadLine();

                    int argb = Convert.ToInt32(square_color1_str);

                    byte[] values = BitConverter.GetBytes(argb);

                    byte a = values[3];
                    byte r = values[2];
                    byte g = values[1];
                    byte b = values[0];

                    TBsquare_color1.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
                    color1 = System.Drawing.Color.FromArgb(a, r, g, b);
                    
                    argb = Convert.ToInt32(square_color2_str);

                    values = BitConverter.GetBytes(argb);

                    a = values[3];
                    r = values[2];
                    g = values[1];
                    b = values[0];

                    TBsquare_color2.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
                    color2 = System.Drawing.Color.FromArgb(a, r, g, b);

                    TBmin_square_size.Text = sr.ReadLine();
                    TBscreen_size.Text = sr.ReadLine();

                    Enum.TryParse(sr.ReadLine(), out lang);

                    if(sr.EndOfStream == false) //support old settings file
                    {
                        CHBcheck_for_updates.IsChecked = bool.Parse(sr.ReadLine());
                    }

                    sr.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                loading_error = true;
                System.Windows.Forms.MessageBox.Show(ex.Message + loading_error_msg, error_title, MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    if (sr != null)
                        sr.Close();
                    if (fs != null)
                        fs.Close();
                }
                catch (Exception ex2) { }
            }
        }

        void update_app_if_necessary()
        {
            try
            {
                string content;
                MyWebClient wc = new MyWebClient();
                content = wc.DownloadString(url_latest_version);

                latest_version = content.Replace("\r\n", "").Trim();
            }
            catch (WebException we)
            {
                latest_version = "unknown";
            }

            bool update_available = false;

            if (latest_version != "unknown" &&
                int.Parse(latest_version.Replace(".", "")) > int.Parse(prog_version.Replace(".", "")))
            {
                update_available = true;
            }

            if ((bool)CHBcheck_for_updates.IsChecked && update_available)
            {
                MessageBoxResult dialogResult = System.Windows.MessageBox.Show("A new program version" +
                    " is available. Do you want to download it now?",
                //    " is available. Do you want to perform an automatic update now?",
                    "New Version Available", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (dialogResult == MessageBoxResult.Yes)
                {
                    //Open download page
                    Process.Start("https://" + url_homepage);
                }
            }
        }

        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 3000;
                return w;
            }
        }
    }
}
