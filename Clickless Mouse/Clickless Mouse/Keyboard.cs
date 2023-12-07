using System;
using System.Threading;
using System.Windows;
using WindowsInput.Native;

namespace Clickless_Mouse
{
    public partial class MainWindow : Window
    {
        Thread THRkeymaster;

        void key_press(VirtualKeyCode vkc, bool async, int down_ms = 75)
        {
            if (async)
            {
                THRkeymaster = new Thread(() => key_press(vkc, down_ms));
                THRkeymaster.Start();
            }
            else
                key_press(vkc, down_ms);
        }

        void key_press(VirtualKeyCode vkc, int down_ms = 75)
        {
            sim.Keyboard.KeyDown(vkc);
            Thread.Sleep(down_ms);
            sim.Keyboard.KeyUp(vkc);
        }

        void key_down(VirtualKeyCode vkc)
        {
            if (vkc == VirtualKeyCode.LBUTTON)
                left_down();
            else if (vkc == VirtualKeyCode.RBUTTON)
                right_down();
            else
                sim.Keyboard.KeyDown(vkc);
        }

        void key_up(VirtualKeyCode vkc)
        {
            if (vkc == VirtualKeyCode.LBUTTON)
                left_up();
            else if (vkc == VirtualKeyCode.RBUTTON)
                right_up();
            else
                sim.Keyboard.KeyUp(vkc);
        }

        void release_buttons_and_keys()
        {
            if (sim.InputDeviceState.IsKeyDown(VirtualKeyCode.LBUTTON))
            {
                left_up();
            }

            if (sim.InputDeviceState.IsKeyDown(VirtualKeyCode.RBUTTON))
            {
                right_up();
            }

            foreach (VirtualKeyCode vkc in (VirtualKeyCode[])Enum.GetValues(typeof(VirtualKeyCode)))
            {
                if (sim.InputDeviceState.IsKeyDown(vkc))
                    key_up(vkc);
            }
        }

        void release_buttons()
        {
            if (sim.InputDeviceState.IsKeyDown(VirtualKeyCode.LBUTTON))
            {
                left_up();
            }

            if (sim.InputDeviceState.IsKeyDown(VirtualKeyCode.RBUTTON))
            {
                right_up();
            }
        }
    }
}