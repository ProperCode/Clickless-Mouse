using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace ClickLess_Mouse
{
    public partial class Form1 : Form
    {
        //Program interface supports 125% font scaling
        //Separate program version supporting full font scaling is supposed to be added 
        //in the future for Windows 10 only

        //Main config
        bool SL_enabled = true;
        bool SR_enabled = true;
        bool SM_enabled = true;
        bool SLH_enabled = false;
        bool SRH_enabled = false;
        int square_time = 150; //cursor hover time in square needed to perform a click

        //Square config
        int size = 50;
        int border_width = 2;
        Color color1 = Color.FromArgb(-16776961); //square color 1
        Color color2 = Color.FromArgb(-256); //square color 2

        Square SL, SR, SM, SLH, SRH;
        DateTime last_show_time;
        Thread THRmouse_monitor = null, THRsquares_monitor, THRmouse_monitor2;
        int displacement = 0;
        const string prog_name = "Clickless Mouse 2.01"; //in registry and form title

        bool load_completed = false;
        string settings_filename = "settings.txt";
        //full path is necessary if run at startup is used (running at startup uses different current
        //directory
        string app_folder_path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        private const byte VK_LEFT = 0x25;
        private const byte VK_UP = 0x26;
        private const byte VK_RIGHT = 0x27;
        private const byte VK_DOWN = 0x28;
        private const int KEYEVENTF_KEYUP = 0x2;
        private const int KEYEVENTF_KEYDOWN = 0x0;

        public Form1()
        {
            InitializeComponent();

            this.Text = prog_name;

            CHBLMB.Checked = true;
            CHBRMB.Checked = true;
            CHBDoubleLMB.Checked = true;

            TBsquare_size.Text = "50";
            TBsquare_border.Text = "2";
            TBcolor1.Text = "-16776961";
            TBcolor2.Text = "-256";

            load_settings();

            load_completed = true;

            generate_squares();

            this.PointToClient(new Point(0, 0));
            int deskWidth = Screen.PrimaryScreen.Bounds.Width;
            int deskHeight = Screen.PrimaryScreen.Bounds.Height;
            int X = deskWidth / 2 - this.Size.Width / 2;
            int Y = deskHeight / 2 - this.Size.Height / 2;
            this.Location = new Point(X, Y);

            if (CHBstart_minimized.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
            }

            THRmouse_monitor = new Thread(new ThreadStart(monitor_mouse));
            THRmouse_monitor.Start();
        }

        void generate_squares()
        {
            if (SL != null)
                SL.Dispose();
            if (SR != null)
                SR.Dispose();
            if (SM != null)
                SM.Dispose();
            if (SLH != null)
                SLH.Dispose();
            if (SRH != null)
                SRH.Dispose();

            SL = new Square(size, border_width, color1, color2);
            SL.TopMost = true;
            SL.Show();
            SL.Height = size;
            SL.Width = size;

            SR = new Square(size, border_width, color1, color2);
            SR.TopMost = true;
            SR.Show();
            SR.Height = size;
            SR.Width = size;

            SM = new Square(size, border_width, color1, color2);
            SM.TopMost = true;
            SM.Show();
            SM.Height = size;
            SM.Width = size;

            SLH = new Square(size, border_width, color1, color2);
            SLH.TopMost = true;
            SLH.Show();
            SLH.Height = size;
            SLH.Width = size;

            SRH = new Square(size, border_width, color1, color2);
            SRH.TopMost = true;
            SRH.Show();
            SRH.Height = size;
            SRH.Width = size;

            SL.Hide();
            SR.Hide();
            SM.Hide();
            SLH.Hide();
            SRH.Hide();

            displacement = (int)(size / 2);
            show_zone = size + displacement;
        }

        public int x = 0, y = 0;
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

        void left_down()
        {
            int x = Cursor.Position.X,
                y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
        }

        void left_up()
        {
            int x = Cursor.Position.X,
                y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        void right_down()
        {
            int x = Cursor.Position.X,
                y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
        }

        void right_up()
        {
            int x = Cursor.Position.X,
                y = Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
        }

        void reset_keys()
        {
            left_up();
            right_up();
            keybd_event(VK_UP, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_DOWN, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_LEFT, 0, KEYEVENTF_KEYUP, 0);
            keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, 0);
        }

        void monitor_mouse2()
        {
            //Thread.Sleep(5000);

            int x, y;
            int max_x = Screen.PrimaryScreen.Bounds.Width - 1;
            int max_y = Screen.PrimaryScreen.Bounds.Height - 1;
            bool pressed_w, pressed_a, pressed_s, pressed_d;

            while (true)
            {
                x = Cursor.Position.X;
                y = Cursor.Position.Y;

                pressed_w = pressed_a = pressed_s = pressed_d = false;

                if (x == 0)
                {
                    keybd_event(VK_LEFT, 0, KEYEVENTF_KEYDOWN, 0);
                    pressed_a = true;
                }
                else if (x == max_x)
                {
                    keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYDOWN, 0);
                    pressed_d = true;
                }
                if (y == 0)
                {
                    keybd_event(VK_UP, 0, KEYEVENTF_KEYDOWN, 0);
                    pressed_w = true;
                }
                else if (y == max_y)
                {
                    keybd_event(VK_DOWN, 0, KEYEVENTF_KEYDOWN, 0);
                    pressed_s = true;
                }

                Thread.Sleep(200);

                if (pressed_w)
                    keybd_event(VK_UP, 0, KEYEVENTF_KEYUP, 0);
                else if (pressed_s)
                    keybd_event(VK_DOWN, 0, KEYEVENTF_KEYUP, 0);
                if (pressed_a)
                    keybd_event(VK_LEFT, 0, KEYEVENTF_KEYUP, 0);
                else if (pressed_d)
                    keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, 0);

                Thread.Sleep(100);
            }
        }

        int banned_x = -1;
        int banned_y = -1;
        void monitor_mouse()
        {
            int i = 0;
            int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
            while (true)
            {
                x1 = Cursor.Position.X;
                y1 = Cursor.Position.Y;
                Thread.Sleep(10);
                x2 = Cursor.Position.X;
                y2 = Cursor.Position.Y;
                if (x1 == x2 && y1 == y2)
                {
                    if (x1 != banned_x && y1 != banned_y)
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
                if (i >= 20 && squares_visible == false)
                {
                    x = x2;
                    y = y2;

                    SL_start_x = x - show_zone;
                    SL_start_y = y - show_zone;
                    SL_end_x = SL_start_x + size;
                    SL_end_y = SL_start_y + size;

                    SR_start_x = x + displacement;
                    SR_start_y = y - show_zone;
                    SR_end_x = SR_start_x + size;
                    SR_end_y = SR_start_y + size;

                    SM_start_x = x - (int)(size / 2);
                    SM_start_y = y - show_zone - displacement;
                    SM_end_x = SM_start_x + size;
                    SM_end_y = SM_start_y + size;

                    SLH_start_x = x - show_zone - displacement;
                    SLH_start_y = y - displacement;
                    SLH_end_x = SLH_start_x + size;
                    SLH_end_y = SLH_start_y + size;

                    SRH_start_x = x + displacement * 2;
                    SRH_start_y = y - displacement;
                    SRH_end_x = SRH_start_x + size;
                    SRH_end_y = SRH_start_y + size;

                    bool show_above = true;

                    //if top screen edge would cover squares show them below mouse cursor instead
                    if (SM_enabled && SM_start_y < -1 * size * 0.66 || (SM_enabled == false
                        && (SL_enabled || SR_enabled) && SL_start_y < -1 * size * 0.66))
                    {
                        show_above = false;

                        SL_start_y = y + displacement;
                        SL_end_y = SL_start_y + size;

                        SR_start_y = y + displacement;
                        SR_end_y = SR_start_y + size;

                        SM_start_y = y + 2 * displacement;
                        SM_end_y = SM_start_y + size;
                    }

                    TimeSpan ts = new TimeSpan();
                    if (last_show_time != null)
                    {
                        ts = DateTime.Now - last_show_time;
                    }

                    if (last_show_time != null && ts.TotalMilliseconds > 500)
                    {
                        squares_visible = true;
                        if (show_above)
                        {
                            if (SL_enabled)
                                show_SL(true);
                            if (SR_enabled)
                                show_SR(true);
                            if (SM_enabled)
                                show_SM(true);
                        }
                        else
                        {
                            if (SL_enabled)
                                show_SL(true, false);
                            if (SR_enabled)
                                show_SR(true, false);
                            if (SM_enabled)
                                show_SM(true, false);
                        }
                        if (SLH_enabled)
                            show_SLH(true);
                        if (SRH_enabled)
                            show_SRH(true);
                        THRsquares_monitor = new Thread(new ThreadStart(monitor_squares));
                        THRsquares_monitor.Start();
                        i = 0;
                    }
                }
                else if (i >= 100 && squares_visible)
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
            int i_max = square_time / 10;
            while (i_SL < i_max && i_SR < i_max && i_SM < i_max
                && i_SLH < i_max && i_SRH < i_max && squares_visible)
            {
                if (SL_enabled)
                {
                    if (is_cursor_in_SL(Cursor.Position.X, Cursor.Position.Y))
                    {
                        i_SL++;
                    }
                    else i_SL = 0;
                }
                if (SR_enabled)
                {
                    if (is_cursor_in_SR(Cursor.Position.X, Cursor.Position.Y))
                    {
                        i_SR++;
                    }
                    else i_SR = 0;
                }
                if (SM_enabled)
                {
                    if (is_cursor_in_SM(Cursor.Position.X, Cursor.Position.Y))
                    {
                        i_SM++;
                    }
                    else i_SM = 0;
                }
                if (SLH_enabled)
                {
                    if (is_cursor_in_SLH(Cursor.Position.X, Cursor.Position.Y))
                    {
                        i_SLH++;
                    }
                    else i_SLH = 0;
                }
                if (SRH_enabled)
                {
                    if (is_cursor_in_SRH(Cursor.Position.X, Cursor.Position.Y))
                    {
                        i_SRH++;
                    }
                    else i_SRH = 0;
                }
                Thread.Sleep(10);
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
                last_show_time = DateTime.Now;
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
                last_show_time = DateTime.Now;
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
                last_show_time = DateTime.Now;
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
                last_show_time = DateTime.Now;
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
                last_show_time = DateTime.Now;
                squares_visible = false;
            }
        }

        private void mouse_move_detected()
        {
            if (squares_visible)
            {
                int x1 = Cursor.Position.X, y1 = Cursor.Position.Y;
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

        public void LMBClick(int X, int Y, int time)
        {
            freeze_mouse(X, Y, 50);
            left_down();
            freeze_mouse(X, Y, time);
            left_up();
            freeze_mouse(X, Y, 10);
        }

        public void RMBClick(int X, int Y, int time)
        {
            freeze_mouse(X, Y, 50);
            right_down();
            freeze_mouse(X, Y, time);
            right_up();
            freeze_mouse(X, Y, 10);
        }


        public void DLMBClick(int X, int Y, int time)
        {
            LMBClick(X, Y, 100);
            real_sleep(100);
            LMBClick(X, Y, 100);
        }

        bool LMBHold_ON = false;
        public void LMBHold(int X, int Y, int time)
        {
            freeze_mouse(X, Y, 50);
            if (LMBHold_ON == false)
            {
                left_down();
                LMBHold_ON = true;
            }
            else
            {
                left_up();
                LMBHold_ON = false;
            }
            freeze_mouse(X, Y, time);
        }

        bool RMBHold_ON = false;
        //bool RMB_SWITCH = false;
        public void RMBHold(int X, int Y, int time)
        {
            freeze_mouse(X, Y, 50);
            ////if (RMBHold_ON == false || RMB_SWITCH == false)
            if (RMBHold_ON == false)
            {
                right_down();
                RMBHold_ON = true;
            }
            else
            {
                right_up();
                RMBHold_ON = false;
            }
            freeze_mouse(X, Y, time);
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
                Cursor.Position = new Point(X, Y);
                Thread.Sleep(1);
            }
            while (stopwatch.ElapsedMilliseconds < time);
        }

        private delegate void Callback1(bool show, bool above);
        private delegate void Callback2(bool show);

        public void show_SL(bool show, bool above = true)
        {
            if (SL.InvokeRequired)
            {
                try
                {
                    // It's on a different thread, so use Invoke.
                    Callback1 d = new Callback1(show_SL);
                    Invoke(d, new object[] { show, above });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                // It's on the same thread, no need for Invoke
                if (show)
                {
                    if (above == true)
                        SL.Location = new Point(x - size - displacement, y - size - displacement);
                    else
                        SL.Location = new Point(x - size - displacement, y + displacement);
                    SL.Show();
                }
                else SL.Hide();
            }
        }
        public void show_SR(bool show, bool above = true)
        {
            if (SR.InvokeRequired)
            {
                try
                {
                    // It's on a different thread, so use Invoke.
                    Callback1 d = new Callback1(show_SR);
                    Invoke(d, new object[] { show, above });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                // It's on the same thread, no need for Invoke
                if (show)
                {
                    if (above == true)
                        SR.Location = new Point(x + displacement, y - size - displacement);
                    else
                        SR.Location = new Point(x + displacement, y + displacement);
                    SR.Show();
                }
                else SR.Hide();
            }
        }
        public void show_SM(bool show, bool above = true)
        {
            if (SM.InvokeRequired)
            {
                try
                {
                    // It's on a different thread, so use Invoke.
                    Callback1 d = new Callback1(show_SM);
                    Invoke(d, new object[] { show, above });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                // It's on the same thread, no need for Invoke
                if (show)
                {
                    if (above == true)
                        SM.Location = new Point(x - (int)(size / 2), y - size - 2 * displacement);
                    else
                        SM.Location = new Point(x - (int)(size / 2), y + 2 * displacement);
                    SM.Show();
                }
                else SM.Hide();
            }
        }
        public void show_SLH(bool show)
        {
            if (SLH.InvokeRequired)
            {
                try
                {
                    // It's on a different thread, so use Invoke.
                    Callback2 d = new Callback2(show_SLH);
                    Invoke(d, new object[] { show });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                // It's on the same thread, no need for Invoke
                if (show)
                {
                    SLH.Location = new Point(x - size - displacement * 2, y - displacement);
                    SLH.Show();
                }
                else SLH.Hide();
            }
        }
        public void show_SRH(bool show)
        {
            if (SRH.InvokeRequired)
            {
                try
                {
                    // It's on a different thread, so use Invoke.
                    Callback2 d = new Callback2(show_SRH);
                    Invoke(d, new object[] { show });
                }
                catch (ObjectDisposedException ex)
                {
                    //
                }
            }
            else
            {
                // It's on the same thread, no need for Invoke
                if (show)
                {
                    SRH.Location = new Point(x + displacement * 2, y - displacement);
                    SRH.Show();
                }
                else SRH.Hide();
            }
        }

        //----------------------------------------------------------------------------------

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            reset_keys();
            Process.GetCurrentProcess().Kill();
        }

        private void TRBhover_time_Scroll(object sender, EventArgs e)
        {
            int val = TRBhover_time.Value;
            if (val == 1) square_time = 10;
            else square_time = (TRBhover_time.Value - 1) * 50;
            Lhover_time.Text = "Hover time to register click: " + square_time + "ms";
            if (load_completed)
            {
                save_settings();
            }
        }

        private void TRBhover_time_ValueChanged(object sender, EventArgs e)
        {
            TRBhover_time_Scroll(new object(), new EventArgs());
        }

        private void TBsquare_size_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int x = int.Parse(TBsquare_size.Text);
                size = x;

                if (load_completed)
                {
                    generate_squares();
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TBsquare_border_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int y = int.Parse(TBsquare_border.Text);
                border_width = y;

                if (load_completed)
                {
                    generate_squares();
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TBcolor1_Click(object sender, EventArgs e)
        {
            DialogResult dr = colorDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                TBcolor1.Text = colorDialog1.Color.ToArgb().ToString();
            }
        }

        private void TBcolor1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int x = int.Parse(TBcolor1.Text);
                if (x > 0)
                    throw new Exception("Wrong ARGB color value");
                else if (x == -1)
                    color1 = Color.FromArgb(-2); //-1 is bugged
                else
                    color1 = Color.FromArgb(x);

                if (load_completed)
                {
                    generate_squares();
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TBcolor2_Click(object sender, EventArgs e)
        {
            DialogResult dr = colorDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                TBcolor2.Text = colorDialog1.Color.ToArgb().ToString();
            }
        }

        private void TBcolor2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int x = int.Parse(TBcolor2.Text);
                if (x > 0)
                    throw new Exception("Wrong ARGB color value");
                else if (x == -1)
                    color2 = Color.FromArgb(-2); //-1 is bugged
                else
                    color2 = Color.FromArgb(x);

                if (load_completed)
                {
                    generate_squares();
                    save_settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Bpreset_1280_Click(object sender, EventArgs e)
        {
            TBsquare_size.Text = "50";
            TBsquare_border.Text = "2";
        }

        private void Bpreset_1680_Click(object sender, EventArgs e)
        {
            TBsquare_size.Text = "54";
            TBsquare_border.Text = "2";
        }

        private void Bpreset_full_hd_Click(object sender, EventArgs e)
        {
            TBsquare_size.Text = "52";
            TBsquare_border.Text = "2";
        }

        private void Bpreset_1440p_Click(object sender, EventArgs e)
        {
            TBsquare_size.Text = "56";
            TBsquare_border.Text = "2";
        }

        private void Bpreset_4k_Click(object sender, EventArgs e)
        {
            TBsquare_size.Text = "82";
            TBsquare_border.Text = "3";
        }

        private void Bpreset_8k_Click(object sender, EventArgs e)
        {
            TBsquare_size.Text = "143";
            TBsquare_border.Text = "6";
        }

        private void Brestore_default_colors_Click(object sender, EventArgs e)
        {
            TBcolor1.Text = "-16776961";
            TBcolor2.Text = "-256";
        }

        private void CHBrun_at_startup_CheckedChanged(object sender, EventArgs e)
        {
            if (load_completed)
            {
                Microsoft.Win32.RegistryKey rkApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (CHBrun_at_startup.Checked)
                {
                    if (rkApp.GetValue(prog_name) == null)
                    {
                        rkApp.SetValue(prog_name, Application.ExecutablePath.ToString());
                    }
                }
                else if (rkApp.GetValue(prog_name) != null)
                {
                    rkApp.DeleteValue(prog_name, false);
                }

                save_settings();
            }
        }

        private void CHBstart_minimized_CheckedChanged(object sender, EventArgs e)
        {
            if (load_completed)
            {
                save_settings();
            }
        }

        private void save_settings()
        {
            FileStream fs = null;
            StreamWriter sw = null;
            string file_path = Path.Combine(app_folder_path, settings_filename);

            try
            {
                fs = new FileStream(file_path, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);

                sw.WriteLine(CHBLMB.Checked);
                sw.WriteLine(CHBRMB.Checked);
                sw.WriteLine(CHBDoubleLMB.Checked);
                sw.WriteLine(CHBHoldLMB.Checked);
                sw.WriteLine(CHBHoldRMB.Checked);
                sw.WriteLine(CHBarrows.Checked);
                sw.WriteLine(TRBhover_time.Value);

                sw.WriteLine(TBsquare_size.Text);
                sw.WriteLine(TBsquare_border.Text);
                sw.WriteLine(TBcolor1.Text);
                sw.WriteLine(TBcolor2.Text);

                sw.WriteLine(CHBrun_at_startup.Checked);
                sw.WriteLine(CHBstart_minimized.Checked);

                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                try
                {
                    sw.Close();
                    fs.Close();
                }
                catch (Exception ex2) { }
            }
        }

        private void load_settings()
        {
            FileStream fs = null;
            StreamReader sr = null;
            string file_path = Path.Combine(app_folder_path, settings_filename);

            try
            {
                if (File.Exists(file_path))
                {
                    fs = new FileStream(file_path, FileMode.Open, FileAccess.Read);
                    sr = new StreamReader(fs);

                    CHBLMB.Checked = bool.Parse(sr.ReadLine());
                    CHBRMB.Checked = bool.Parse(sr.ReadLine());
                    CHBDoubleLMB.Checked = bool.Parse(sr.ReadLine());
                    CHBHoldLMB.Checked = bool.Parse(sr.ReadLine());
                    CHBHoldRMB.Checked = bool.Parse(sr.ReadLine());
                    CHBarrows.Checked = bool.Parse(sr.ReadLine());
                    TRBhover_time.Value = int.Parse(sr.ReadLine());

                    TBsquare_size.Text = sr.ReadLine();
                    TBsquare_border.Text = sr.ReadLine();
                    TBcolor1.Text = sr.ReadLine();
                    TBcolor2.Text = sr.ReadLine();

                    CHBrun_at_startup.Checked = bool.Parse(sr.ReadLine());
                    CHBstart_minimized.Checked = bool.Parse(sr.ReadLine());

                    sr.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                try
                {
                    sr.Close();
                    fs.Close();
                }
                catch (Exception ex2) { }
            }
        }

        private void CHBLMB_CheckedChanged(object sender, EventArgs e)
        {
            if (load_completed)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBLMB.Checked)
                SL_enabled = true;
            else
                SL_enabled = false;

            if (load_completed)
            {
                save_settings();
            }
        }

        private void CHBRMB_CheckedChanged(object sender, EventArgs e)
        {
            if (load_completed)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBRMB.Checked)
                SR_enabled = true;
            else
                SR_enabled = false;

            if (load_completed)
            {
                save_settings();
            }
        }

        private void CHBDoubleLMB_CheckedChanged(object sender, EventArgs e)
        {
            if (load_completed)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBDoubleLMB.Checked)
                SM_enabled = true;
            else
                SM_enabled = false;

            if (load_completed)
            {
                save_settings();
            }
        }

        private void CHBHoldLMB_CheckedChanged(object sender, EventArgs e)
        {
            if (load_completed)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBHoldLMB.Checked)
                SLH_enabled = true;
            else
                SLH_enabled = false;

            if (load_completed)
            {
                save_settings();
            }
        }

        private void CHBHoldRMB_CheckedChanged(object sender, EventArgs e)
        {
            if (load_completed)
            {
                show_SL(false);
                show_SR(false);
                show_SM(false);
                show_SLH(false);
                show_SRH(false);
            }

            if (CHBHoldRMB.Checked)
                SRH_enabled = true;
            else
                SRH_enabled = false;

            if (load_completed)
            {
                save_settings();
            }
        }

        private void CHBarrows_CheckedChanged(object sender, EventArgs e)
        {
            if (CHBarrows.Checked)
            {
                THRmouse_monitor2 = new Thread(new ThreadStart(monitor_mouse2));
                THRmouse_monitor2.Start();
            }
            else if (THRmouse_monitor2 != null)
            {
                THRmouse_monitor2.Abort();
                THRmouse_monitor2 = null;
            }

            if (load_completed)
            {
                save_settings();
            }
        }
    }
}
