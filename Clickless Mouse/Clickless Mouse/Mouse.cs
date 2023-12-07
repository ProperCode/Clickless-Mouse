using System.Windows;
using WindowsInput.Native;

namespace Clickless_Mouse
{
    public partial class MainWindow : Window
    {
        void left_down()
        {
            sim.Mouse.LeftButtonDown();
        }

        void left_up()
        {
            sim.Mouse.LeftButtonUp();
        }

        void right_down()
        {
            sim.Mouse.RightButtonDown();
        }

        void right_up()
        {
            sim.Mouse.RightButtonUp();
        }

        public void LMBClick(int X, int Y, int time)
        {
            ///user may forget that right button is pressed or press it by mistake without noticing
            //(holding RMB prevents LMB clicking)
            if (sim.InputDeviceState.IsKeyDown(VirtualKeyCode.RBUTTON))
            {
                right_up();
            }
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
            //user may forget that right button is pressed or press it by mistake without noticing
            //(holding RMB prevents LMB clicking)
            if (sim.InputDeviceState.IsKeyDown(VirtualKeyCode.RBUTTON))
            {
                right_up();
            }
            LMBClick(X, Y, 100);
            LMBClick(X, Y, 100);
        }

        public void LMBHold(int X, int Y, int time)
        {
            freeze_mouse(X, Y, 50);
            if (sim.InputDeviceState.IsKeyDown(VirtualKeyCode.LBUTTON) == false)
            {
                left_down();
            }
            else
            {
                left_up();
            }
            freeze_mouse(X, Y, time);
        }

        public void RMBHold(int X, int Y, int time)
        {
            freeze_mouse(X, Y, 50);
            if (sim.InputDeviceState.IsKeyDown(VirtualKeyCode.RBUTTON) == false)
            {
                right_down();
            }
            else
            {
                right_up();
            }
            freeze_mouse(X, Y, time);
        }
    }
}