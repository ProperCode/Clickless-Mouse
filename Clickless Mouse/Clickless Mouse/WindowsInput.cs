using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using WindowsInput.Native;
using System.Threading;

namespace WindowsInput
{
    /// <summary>
    /// Implements the <see cref="IInputMessageDispatcher"/> by calling <see cref="NativeMethods.SendInput"/>.
    /// </summary>
    internal class WindowsInputMessageDispatcher : IInputMessageDispatcher
    {
        /// <summary>
        /// Dispatches the specified list of <see cref="INPUT"/> messages in their specified order by issuing a single called to <see cref="NativeMethods.SendInput"/>.
        /// </summary>
        /// <param name="inputs">The list of <see cref="INPUT"/> messages to be dispatched.</param>
        /// <exception cref="ArgumentException">If the <paramref name="inputs"/> array is empty.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="inputs"/> array is null.</exception>
        /// <exception cref="Exception">If the any of the commands in the <paramref name="inputs"/> array could not be sent successfully.</exception>
        public void DispatchInput(INPUT[] inputs)
        {
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (inputs.Length == 0) throw new ArgumentException("The input array was empty", "inputs");
            var successful = NativeMethods.SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof (INPUT)));
            if (successful != inputs.Length)
                throw new Exception("Some simulated input commands were not sent successfully. The most common reason for this happening are the security features of Windows including User Interface Privacy Isolation (UIPI). Your application can only send commands to applications of the same or lower elevation. Similarly certain commands are restricted to Accessibility/UIAutomation applications. Refer to the project home page and the code samples for more information.");
        }
    }
}

namespace WindowsInput
{
    /// <summary>
    /// The contract for a service that interprets the state of input devices.
    /// </summary>
    public interface IInputDeviceStateAdaptor
    {
        /// <summary>
        /// Determines whether the specified key is up or down.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is down; otherwise, <c>false</c>.
        /// </returns>
        bool IsKeyDown(VirtualKeyCode keyCode);

        /// <summary>
        /// Determines whether the specified key is up or down.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is up; otherwise, <c>false</c>.
        /// </returns>
        bool IsKeyUp(VirtualKeyCode keyCode);

        /// <summary>
        /// Determines whether the physical key is up or down at the time the function is called regardless of whether the application thread has read the keyboard event from the message pump.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is down; otherwise, <c>false</c>.
        /// </returns>
        bool IsHardwareKeyDown(VirtualKeyCode keyCode);

        /// <summary>
        /// Determines whether the physical key is up or down at the time the function is called regardless of whether the application thread has read the keyboard event from the message pump.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is up; otherwise, <c>false</c>.
        /// </returns>
        bool IsHardwareKeyUp(VirtualKeyCode keyCode);

        /// <summary>
        /// Determines whether the toggling key is toggled on (in-effect) or not.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the toggling key is toggled on (in-effect); otherwise, <c>false</c>.
        /// </returns>
        bool IsTogglingKeyInEffect(VirtualKeyCode keyCode);
    }
}

namespace WindowsInput
{
    /// <summary>
    /// The contract for a service that dispatches <see cref="INPUT"/> messages to the appropriate destination.
    /// </summary>
    internal interface IInputMessageDispatcher
    {
        /// <summary>
        /// Dispatches the specified list of <see cref="INPUT"/> messages in their specified order.
        /// </summary>
        /// <param name="inputs">The list of <see cref="INPUT"/> messages to be dispatched.</param>
        /// <exception cref="ArgumentException">If the <paramref name="inputs"/> array is empty.</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="inputs"/> array is null.</exception>
        /// <exception cref="Exception">If the any of the commands in the <paramref name="inputs"/> array could not be sent successfully.</exception>
        void DispatchInput(INPUT[] inputs);
    }
}
namespace WindowsInput
{
    /// <summary>
    /// The contract for a service that simulates Keyboard and Mouse input and Hardware Input Device state detection for the Windows Platform.
    /// </summary>
    public interface IInputSimulator
    {
        /// <summary>
        /// Gets the <see cref="IKeyboardSimulator"/> instance for simulating Keyboard input.
        /// </summary>
        /// <value>The <see cref="IKeyboardSimulator"/> instance.</value>
        IKeyboardSimulator Keyboard { get; }

        /// <summary>
        /// Gets the <see cref="IMouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="IMouseSimulator"/> instance.</value>
        IMouseSimulator Mouse { get; }

        /// <summary>
        /// Gets the <see cref="IInputDeviceStateAdaptor"/> instance for determining the state of the various input devices.
        /// </summary>
        /// <value>The <see cref="IInputDeviceStateAdaptor"/> instance.</value>
        IInputDeviceStateAdaptor InputDeviceState { get; }
    }
}

namespace WindowsInput
{
    /// <summary>
    /// The service contract for a keyboard simulator for the Windows platform.
    /// </summary>
    public interface IKeyboardSimulator
    {
        /// <summary>
        /// Gets the <see cref="IMouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="IMouseSimulator"/> instance.</value>
        IMouseSimulator Mouse { get; }

        /// <summary>
        /// Simulates the key down gesture for the specified key.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        IKeyboardSimulator KeyDown(VirtualKeyCode keyCode);

        /// <summary>
        /// Simulates the key press gesture for the specified key.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        IKeyboardSimulator KeyPress(VirtualKeyCode keyCode);

        /// <summary>
        /// Simulates a key press for each of the specified key codes in the order they are specified.
        /// </summary>
        /// <param name="keyCodes"></param>
        IKeyboardSimulator KeyPress(params VirtualKeyCode[] keyCodes);

        /// <summary>
        /// Simulates the key up gesture for the specified key.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        IKeyboardSimulator KeyUp(VirtualKeyCode keyCode);

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers and multiple keys like CTRL-ALT-K-C where CTRL and ALT are the modifierKeys and K and C are the keys.
        /// The flow is Modifiers KeyDown in order, Keys Press in order, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of <see cref="VirtualKeyCode"/>s for the modifier keys.</param>
        /// <param name="keyCodes">The list of <see cref="VirtualKeyCode"/>s for the keys to simulate.</param>
        IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, IEnumerable<VirtualKeyCode> keyCodes);

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers and one key like CTRL-ALT-C where CTRL and ALT are the modifierKeys and C is the key.
        /// The flow is Modifiers KeyDown in order, Key Press, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of <see cref="VirtualKeyCode"/>s for the modifier keys.</param>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode);

        /// <summary>
        /// Simulates a modified keystroke where there is one modifier and multiple keys like CTRL-K-C where CTRL is the modifierKey and K and C are the keys.
        /// The flow is Modifier KeyDown, Keys Press in order, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKey">The <see cref="VirtualKeyCode"/> for the modifier key.</param>
        /// <param name="keyCodes">The list of <see cref="VirtualKeyCode"/>s for the keys to simulate.</param>
        IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKey, IEnumerable<VirtualKeyCode> keyCodes);

        /// <summary>
        /// Simulates a simple modified keystroke like CTRL-C where CTRL is the modifierKey and C is the key.
        /// The flow is Modifier KeyDown, Key Press, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKeyCode">The <see cref="VirtualKeyCode"/> for the  modifier key.</param>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKeyCode, VirtualKeyCode keyCode);

        /// <summary>
        /// Simulates uninterrupted text entry via the keyboard.
        /// </summary>
        /// <param name="text">The text to be simulated.</param>
        IKeyboardSimulator TextEntry(string text);

        /// <summary>
        /// Simulates a single character text entry via the keyboard.
        /// </summary>
        /// <param name="character">The unicode character to be simulated.</param>
        IKeyboardSimulator TextEntry(char character);

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="millsecondsTimeout">The number of milliseconds to wait.</param>
        IKeyboardSimulator Sleep(int millsecondsTimeout);

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="timeout">The time to wait.</param>
        IKeyboardSimulator Sleep(TimeSpan timeout);
    }
}
namespace WindowsInput
{
    /// <summary>
    /// The service contract for a mouse simulator for the Windows platform.
    /// </summary>
    public interface IMouseSimulator
    {
        /// <summary>
        /// Gets the <see cref="IKeyboardSimulator"/> instance for simulating Keyboard input.
        /// </summary>
        /// <value>The <see cref="IKeyboardSimulator"/> instance.</value>
        IKeyboardSimulator Keyboard { get; }

        /// <summary>
        /// Simulates mouse movement by the specified distance measured as a delta from the current mouse location in pixels.
        /// </summary>
        /// <param name="pixelDeltaX">The distance in pixels to move the mouse horizontally.</param>
        /// <param name="pixelDeltaY">The distance in pixels to move the mouse vertically.</param>
        IMouseSimulator MoveMouseBy(int pixelDeltaX, int pixelDeltaY);

        /// <summary>
        /// Simulates mouse movement to the specified location on the primary display device.
        /// </summary>
        /// <param name="absoluteX">The destination's absolute X-coordinate on the primary display device where 0 is the extreme left hand side of the display device and 65535 is the extreme right hand side of the display device.</param>
        /// <param name="absoluteY">The destination's absolute Y-coordinate on the primary display device where 0 is the top of the display device and 65535 is the bottom of the display device.</param>
        IMouseSimulator MoveMouseTo(double absoluteX, double absoluteY);

        /// <summary>
        /// Simulates mouse movement to the specified location on the Virtual Desktop which includes all active displays.
        /// </summary>
        /// <param name="absoluteX">The destination's absolute X-coordinate on the virtual desktop where 0 is the left hand side of the virtual desktop and 65535 is the extreme right hand side of the virtual desktop.</param>
        /// <param name="absoluteY">The destination's absolute Y-coordinate on the virtual desktop where 0 is the top of the virtual desktop and 65535 is the bottom of the virtual desktop.</param>
        IMouseSimulator MoveMouseToPositionOnVirtualDesktop(double absoluteX, double absoluteY);

        /// <summary>
        /// Simulates a mouse left button down gesture.
        /// </summary>
        IMouseSimulator LeftButtonDown();

        /// <summary>
        /// Simulates a mouse left button up gesture.
        /// </summary>
        IMouseSimulator LeftButtonUp();

        /// <summary>
        /// Simulates a mouse left button click gesture.
        /// </summary>
        IMouseSimulator LeftButtonClick();

        /// <summary>
        /// Simulates a mouse left button double-click gesture.
        /// </summary>
        IMouseSimulator LeftButtonDoubleClick();

        /// <summary>
        /// Simulates a mouse right button down gesture.
        /// </summary>
        IMouseSimulator RightButtonDown();

        /// <summary>
        /// Simulates a mouse right button up gesture.
        /// </summary>
        IMouseSimulator RightButtonUp();

        /// <summary>
        /// Simulates a mouse right button click gesture.
        /// </summary>
        IMouseSimulator RightButtonClick();

        /// <summary>
        /// Simulates a mouse right button double-click gesture.
        /// </summary>
        IMouseSimulator RightButtonDoubleClick();

        /// <summary>
        /// Simulates a mouse X button down gesture.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        IMouseSimulator XButtonDown(int buttonId);

        /// <summary>
        /// Simulates a mouse X button up gesture.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        IMouseSimulator XButtonUp(int buttonId);

        /// <summary>
        /// Simulates a mouse X button click gesture.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        IMouseSimulator XButtonClick(int buttonId);

        /// <summary>
        /// Simulates a mouse X button double-click gesture.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        IMouseSimulator XButtonDoubleClick(int buttonId);

        /// <summary>
        /// Simulates mouse vertical wheel scroll gesture.
        /// </summary>
        /// <param name="scrollAmountInClicks">The amount to scroll in clicks. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user.</param>
        IMouseSimulator VerticalScroll(int scrollAmountInClicks);

        /// <summary>
        /// Simulates a mouse horizontal wheel scroll gesture. Supported by Windows Vista and later.
        /// </summary>
        /// <param name="scrollAmountInClicks">The amount to scroll in clicks. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left.</param>
        IMouseSimulator HorizontalScroll(int scrollAmountInClicks);

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="millsecondsTimeout">The number of milliseconds to wait.</param>
        IMouseSimulator Sleep(int millsecondsTimeout);

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="timeout">The time to wait.</param>
        IMouseSimulator Sleep(TimeSpan timeout);
    }
}
namespace WindowsInput
{
    /// <summary>
    /// A helper class for building a list of <see cref="INPUT"/> messages ready to be sent to the native Windows API.
    /// </summary>
    internal class InputBuilder : IEnumerable<INPUT>
    {
        /// <summary>
        /// The public list of <see cref="INPUT"/> messages being built by this instance.
        /// </summary>
        private readonly List<INPUT> _inputList;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBuilder"/> class.
        /// </summary>
        public InputBuilder()
        {
            _inputList = new List<INPUT>();
        }

        /// <summary>
        /// Returns the list of <see cref="INPUT"/> messages as a <see cref="System.Array"/> of <see cref="INPUT"/> messages.
        /// </summary>
        /// <returns>The <see cref="System.Array"/> of <see cref="INPUT"/> messages.</returns>
        public INPUT[] ToArray()
        {
            return _inputList.ToArray();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list of <see cref="INPUT"/> messages.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the list of <see cref="INPUT"/> messages.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<INPUT> GetEnumerator()
        {
            return _inputList.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list of <see cref="INPUT"/> messages.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the list of <see cref="INPUT"/> messages.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="INPUT"/> at the specified position.
        /// </summary>
        /// <value>The <see cref="INPUT"/> message at the specified position.</value>
        public INPUT this[int position]
        {
            get
            {
                return _inputList[position];
            }
        }

        /// <summary>
        /// Determines if the <see cref="VirtualKeyCode"/> is an ExtendedKey
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <returns>true if the key code is an extended key; otherwise, false.</returns>
        /// <remarks>
        /// The extended keys consist of the ALT and CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP, PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK key; the BREAK (CTRL+PAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in the numeric keypad.
        /// 
        /// See http://msdn.microsoft.com/en-us/library/ms646267(v=vs.85).aspx Section "Extended-Key Flag"
        /// </remarks>
        public static bool IsExtendedKey(VirtualKeyCode keyCode)
        {
            if (keyCode == VirtualKeyCode.MENU ||
                keyCode == VirtualKeyCode.LMENU ||
                keyCode == VirtualKeyCode.RMENU ||
                keyCode == VirtualKeyCode.CONTROL ||
                keyCode == VirtualKeyCode.RCONTROL ||
                keyCode == VirtualKeyCode.INSERT ||
                keyCode == VirtualKeyCode.DELETE ||
                keyCode == VirtualKeyCode.HOME ||
                keyCode == VirtualKeyCode.END ||
                keyCode == VirtualKeyCode.PRIOR ||
                keyCode == VirtualKeyCode.NEXT ||
                keyCode == VirtualKeyCode.RIGHT ||
                keyCode == VirtualKeyCode.UP ||
                keyCode == VirtualKeyCode.LEFT ||
                keyCode == VirtualKeyCode.DOWN ||
                keyCode == VirtualKeyCode.NUMLOCK ||
                keyCode == VirtualKeyCode.CANCEL ||
                keyCode == VirtualKeyCode.SNAPSHOT ||
                keyCode == VirtualKeyCode.DIVIDE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a key down to the list of <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddKeyDown(VirtualKeyCode keyCode)
        {
            var down =
                new INPUT
                    {
                        Type = (UInt32) InputType.Keyboard,
                        Data =
                            {
                                Keyboard =
                                    new KEYBDINPUT
                                        {
                                            KeyCode = (UInt16) keyCode,
                                            Scan = (UInt16)(NativeMethods.MapVirtualKey((UInt32)keyCode, 0) & 0xFFU),
                                            Flags = IsExtendedKey(keyCode) ? (UInt32) KeyboardFlag.ExtendedKey : 0,
                                            Time = 0,
                                            ExtraInfo = IntPtr.Zero
                                        }
                            }
                    };

            _inputList.Add(down);
            return this;
        }

        /// <summary>
        /// Adds a key up to the list of <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddKeyUp(VirtualKeyCode keyCode)
        {
            var up =
                new INPUT
                    {
                        Type = (UInt32) InputType.Keyboard,
                        Data =
                            {
                                Keyboard =
                                    new KEYBDINPUT
                                        {
                                            KeyCode = (UInt16) keyCode,
                                            Scan = (UInt16)(NativeMethods.MapVirtualKey((UInt32)keyCode, 0) & 0xFFU),
                                            Flags = (UInt32) (IsExtendedKey(keyCode)
                                                                  ? KeyboardFlag.KeyUp | KeyboardFlag.ExtendedKey
                                                                  : KeyboardFlag.KeyUp),
                                            Time = 0,
                                            ExtraInfo = IntPtr.Zero
                                        }
                            }
                    };

            _inputList.Add(up);
            return this;
        }

        /// <summary>
        /// Adds a key press to the list of <see cref="INPUT"/> messages which is equivalent to a key down followed by a key up.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/>.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddKeyPress(VirtualKeyCode keyCode)
        {
            AddKeyDown(keyCode);
            AddKeyUp(keyCode);
            return this;
        }

        /// <summary>
        /// Adds the character to the list of <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="character">The <see cref="System.Char"/> to be added to the list of <see cref="INPUT"/> messages.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddCharacter(char character)
        {
            UInt16 scanCode = character;

            var down = new INPUT
                           {
                               Type = (UInt32)InputType.Keyboard,
                               Data =
                                   {
                                       Keyboard =
                                           new KEYBDINPUT
                                               {
                                                   KeyCode = 0,
                                                   Scan = scanCode,
                                                   Flags = (UInt32)KeyboardFlag.Unicode,
                                                   Time = 0,
                                                   ExtraInfo = IntPtr.Zero
                                               }
                                   }
                           };

            var up = new INPUT
                         {
                             Type = (UInt32)InputType.Keyboard,
                             Data =
                                 {
                                     Keyboard =
                                         new KEYBDINPUT
                                             {
                                                 KeyCode = 0,
                                                 Scan = scanCode,
                                                 Flags =
                                                     (UInt32)(KeyboardFlag.KeyUp | KeyboardFlag.Unicode),
                                                 Time = 0,
                                                 ExtraInfo = IntPtr.Zero
                                             }
                                 }
                         };

            // Handle extended keys:
            // If the scan code is preceded by a prefix byte that has the value 0xE0 (224),
            // we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
            if ((scanCode & 0xFF00) == 0xE000)
            {
                down.Data.Keyboard.Flags |= (UInt32)KeyboardFlag.ExtendedKey;
                up.Data.Keyboard.Flags |= (UInt32)KeyboardFlag.ExtendedKey;
            }

            _inputList.Add(down);
            _inputList.Add(up);
            return this;
        }

        /// <summary>
        /// Adds all of the characters in the specified <see cref="IEnumerable{T}"/> of <see cref="char"/>.
        /// </summary>
        /// <param name="characters">The characters to add.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddCharacters(IEnumerable<char> characters)
        {
            foreach (var character in characters)
            {
                AddCharacter(character);
            }
            return this;
        }

        /// <summary>
        /// Adds the characters in the specified <see cref="string"/>.
        /// </summary>
        /// <param name="characters">The string of <see cref="char"/> to add.</param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddCharacters(string characters)
        {
            return AddCharacters(characters.ToCharArray());
        }

        /// <summary>
        /// Moves the mouse relative to its current position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddRelativeMouseMovement(int x, int y)
        {
            var movement = new INPUT { Type = (UInt32)InputType.Mouse };
            movement.Data.Mouse.Flags = (UInt32)MouseFlag.Move;
            movement.Data.Mouse.X = x;
            movement.Data.Mouse.Y = y;

            _inputList.Add(movement);

            return this;
        }

        /// <summary>
        /// Move the mouse to an absolute position.
        /// </summary>
        /// <param name="absoluteX"></param>
        /// <param name="absoluteY"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddAbsoluteMouseMovement(int absoluteX, int absoluteY)
        {
            var movement = new INPUT { Type = (UInt32)InputType.Mouse };
            movement.Data.Mouse.Flags = (UInt32)(MouseFlag.Move | MouseFlag.Absolute);
            movement.Data.Mouse.X = absoluteX;
            movement.Data.Mouse.Y = absoluteY;

            _inputList.Add(movement);

            return this;
        }

        /// <summary>
        /// Move the mouse to the absolute position on the virtual desktop.
        /// </summary>
        /// <param name="absoluteX"></param>
        /// <param name="absoluteY"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddAbsoluteMouseMovementOnVirtualDesktop(int absoluteX, int absoluteY)
        {
            var movement = new INPUT { Type = (UInt32)InputType.Mouse };
            movement.Data.Mouse.Flags = (UInt32)(MouseFlag.Move | MouseFlag.Absolute | MouseFlag.VirtualDesk);
            movement.Data.Mouse.X = absoluteX;
            movement.Data.Mouse.Y = absoluteY;

            _inputList.Add(movement);

            return this;
        }

        /// <summary>
        /// Adds a mouse button down for the specified button.
        /// </summary>
        /// <param name="button"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseButtonDown(MouseButton button)
        {
            var buttonDown = new INPUT { Type = (UInt32)InputType.Mouse };
            buttonDown.Data.Mouse.Flags = (UInt32)ToMouseButtonDownFlag(button);

            _inputList.Add(buttonDown);

            return this;
        }

        /// <summary>
        /// Adds a mouse button down for the specified button.
        /// </summary>
        /// <param name="xButtonId"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseXButtonDown(int xButtonId)
        {
            var buttonDown = new INPUT { Type = (UInt32)InputType.Mouse };
            buttonDown.Data.Mouse.Flags = (UInt32)MouseFlag.XDown;
            buttonDown.Data.Mouse.MouseData = (UInt32)xButtonId;
            _inputList.Add(buttonDown);

            return this;
        }

        /// <summary>
        /// Adds a mouse button up for the specified button.
        /// </summary>
        /// <param name="button"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseButtonUp(MouseButton button)
        {
            var buttonUp = new INPUT { Type = (UInt32)InputType.Mouse };
            buttonUp.Data.Mouse.Flags = (UInt32)ToMouseButtonUpFlag(button);
            _inputList.Add(buttonUp);

            return this;
        }

        /// <summary>
        /// Adds a mouse button up for the specified button.
        /// </summary>
        /// <param name="xButtonId"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseXButtonUp(int xButtonId)
        {
            var buttonUp = new INPUT { Type = (UInt32)InputType.Mouse };
            buttonUp.Data.Mouse.Flags = (UInt32)MouseFlag.XUp;
            buttonUp.Data.Mouse.MouseData = (UInt32)xButtonId;
            _inputList.Add(buttonUp);

            return this;
        }

        /// <summary>
        /// Adds a single click of the specified button.
        /// </summary>
        /// <param name="button"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseButtonClick(MouseButton button)
        {
            return AddMouseButtonDown(button).AddMouseButtonUp(button);
        }

        /// <summary>
        /// Adds a single click of the specified button.
        /// </summary>
        /// <param name="xButtonId"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseXButtonClick(int xButtonId)
        {
            return AddMouseXButtonDown(xButtonId).AddMouseXButtonUp(xButtonId);
        }

        /// <summary>
        /// Adds a double click of the specified button.
        /// </summary>
        /// <param name="button"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseButtonDoubleClick(MouseButton button)
        {
            return AddMouseButtonClick(button).AddMouseButtonClick(button);
        }

        /// <summary>
        /// Adds a double click of the specified button.
        /// </summary>
        /// <param name="xButtonId"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseXButtonDoubleClick(int xButtonId)
        {
            return AddMouseXButtonClick(xButtonId).AddMouseXButtonClick(xButtonId);
        }

        /// <summary>
        /// Scroll the vertical mouse wheel by the specified amount.
        /// </summary>
        /// <param name="scrollAmount"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseVerticalWheelScroll(int scrollAmount)
        {
            var scroll = new INPUT { Type = (UInt32)InputType.Mouse };
            scroll.Data.Mouse.Flags = (UInt32)MouseFlag.VerticalWheel;
            scroll.Data.Mouse.MouseData = (UInt32)scrollAmount;

            _inputList.Add(scroll);

            return this;
        }

        /// <summary>
        /// Scroll the horizontal mouse wheel by the specified amount.
        /// </summary>
        /// <param name="scrollAmount"></param>
        /// <returns>This <see cref="InputBuilder"/> instance.</returns>
        public InputBuilder AddMouseHorizontalWheelScroll(int scrollAmount)
        {
            var scroll = new INPUT { Type = (UInt32)InputType.Mouse };
            scroll.Data.Mouse.Flags = (UInt32)MouseFlag.HorizontalWheel;
            scroll.Data.Mouse.MouseData = (UInt32)scrollAmount;

            _inputList.Add(scroll);

            return this;
        }

        private static MouseFlag ToMouseButtonDownFlag(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return MouseFlag.LeftDown;

                case MouseButton.MiddleButton:
                    return MouseFlag.MiddleDown;

                case MouseButton.RightButton:
                    return MouseFlag.RightDown;

                default:
                    return MouseFlag.LeftDown;
            }
        }

        private static MouseFlag ToMouseButtonUpFlag(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return MouseFlag.LeftUp;

                case MouseButton.MiddleButton:
                    return MouseFlag.MiddleUp;

                case MouseButton.RightButton:
                    return MouseFlag.RightUp;

                default:
                    return MouseFlag.LeftUp;
            }
        }
    }
}
namespace WindowsInput
{
    /// <summary>
    /// Implements the <see cref="IInputSimulator"/> interface to simulate Keyboard and Mouse input and provide the state of those input devices.
    /// </summary>
    public class InputSimulator : IInputSimulator
    {
        /// <summary>
        /// The <see cref="IKeyboardSimulator"/> instance to use for simulating keyboard input.
        /// </summary>
        private readonly IKeyboardSimulator _keyboardSimulator;

        /// <summary>
        /// The <see cref="IMouseSimulator"/> instance to use for simulating mouse input.
        /// </summary>
        private readonly IMouseSimulator _mouseSimulator;

        /// <summary>
        /// The <see cref="IInputDeviceStateAdaptor"/> instance to use for interpreting the state of the input devices.
        /// </summary>
        private readonly IInputDeviceStateAdaptor _inputDeviceState;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSimulator"/> class using the specified <see cref="IKeyboardSimulator"/>, <see cref="IMouseSimulator"/> and <see cref="IInputDeviceStateAdaptor"/> instances.
        /// </summary>
        /// <param name="keyboardSimulator">The <see cref="IKeyboardSimulator"/> instance to use for simulating keyboard input.</param>
        /// <param name="mouseSimulator">The <see cref="IMouseSimulator"/> instance to use for simulating mouse input.</param>
        /// <param name="inputDeviceStateAdaptor">The <see cref="IInputDeviceStateAdaptor"/> instance to use for interpreting the state of input devices.</param>
        public InputSimulator(IKeyboardSimulator keyboardSimulator, IMouseSimulator mouseSimulator, IInputDeviceStateAdaptor inputDeviceStateAdaptor)
        {
            _keyboardSimulator = keyboardSimulator;
            _mouseSimulator = mouseSimulator;
            _inputDeviceState = inputDeviceStateAdaptor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputSimulator"/> class using the default <see cref="KeyboardSimulator"/>, <see cref="MouseSimulator"/> and <see cref="WindowsInputDeviceStateAdaptor"/> instances.
        /// </summary>
        public InputSimulator()
        {
            _keyboardSimulator = new KeyboardSimulator(this);
            _mouseSimulator = new MouseSimulator(this);
            _inputDeviceState = new WindowsInputDeviceStateAdaptor();
        }

        /// <summary>
        /// Gets the <see cref="IKeyboardSimulator"/> instance for simulating Keyboard input.
        /// </summary>
        /// <value>The <see cref="IKeyboardSimulator"/> instance.</value>
        public IKeyboardSimulator Keyboard
        {
            get { return _keyboardSimulator; }
        }

        /// <summary>
        /// Gets the <see cref="IMouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="IMouseSimulator"/> instance.</value>
        public IMouseSimulator Mouse
        {
            get { return _mouseSimulator; }
        }

        /// <summary>
        /// Gets the <see cref="IInputDeviceStateAdaptor"/> instance for determining the state of the various input devices.
        /// </summary>
        /// <value>The <see cref="IInputDeviceStateAdaptor"/> instance.</value>
        public IInputDeviceStateAdaptor InputDeviceState
        {
            get { return _inputDeviceState; }
        }
    }
}
namespace WindowsInput
{
    /// <summary>
    /// Implements the <see cref="IKeyboardSimulator"/> interface by calling the an <see cref="IInputMessageDispatcher"/> to simulate Keyboard gestures.
    /// </summary>
    public class KeyboardSimulator : IKeyboardSimulator
    {
        private readonly IInputSimulator _inputSimulator;

        /// <summary>
        /// The instance of the <see cref="IInputMessageDispatcher"/> to use for dispatching <see cref="INPUT"/> messages.
        /// </summary>
        private readonly IInputMessageDispatcher _messageDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardSimulator"/> class using an instance of a <see cref="WindowsInputMessageDispatcher"/> for dispatching <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="inputSimulator">The <see cref="IInputSimulator"/> that owns this instance.</param>
        public KeyboardSimulator(IInputSimulator inputSimulator)
        {
            if (inputSimulator == null) throw new ArgumentNullException("inputSimulator");

            _inputSimulator = inputSimulator;
            _messageDispatcher = new WindowsInputMessageDispatcher();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardSimulator"/> class using the specified <see cref="IInputMessageDispatcher"/> for dispatching <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="inputSimulator">The <see cref="IInputSimulator"/> that owns this instance.</param>
        /// <param name="messageDispatcher">The <see cref="IInputMessageDispatcher"/> to use for dispatching <see cref="INPUT"/> messages.</param>
        /// <exception cref="InvalidOperationException">If null is passed as the <paramref name="messageDispatcher"/>.</exception>
        internal KeyboardSimulator(IInputSimulator inputSimulator, IInputMessageDispatcher messageDispatcher)
        {
            if (inputSimulator == null) throw new ArgumentNullException("inputSimulator");

            if (messageDispatcher == null)
                throw new InvalidOperationException(
                    string.Format("The {0} cannot operate with a null {1}. Please provide a valid {1} instance to use for dispatching {2} messages.",
                    typeof(KeyboardSimulator).Name, typeof(IInputMessageDispatcher).Name, typeof(INPUT).Name));

            _inputSimulator = inputSimulator;
            _messageDispatcher = messageDispatcher;
        }

        /// <summary>
        /// Gets the <see cref="IMouseSimulator"/> instance for simulating Mouse input.
        /// </summary>
        /// <value>The <see cref="IMouseSimulator"/> instance.</value>
        public IMouseSimulator Mouse { get { return _inputSimulator.Mouse; } }

        private void ModifiersDown(InputBuilder builder, IEnumerable<VirtualKeyCode> modifierKeyCodes)
        {
            if (modifierKeyCodes == null) return;
            foreach (var key in modifierKeyCodes) builder.AddKeyDown(key);
        }

        private void ModifiersUp(InputBuilder builder, IEnumerable<VirtualKeyCode> modifierKeyCodes)
        {
            if (modifierKeyCodes == null) return;

            // Key up in reverse (I miss LINQ)
            var stack = new Stack<VirtualKeyCode>(modifierKeyCodes);
            while (stack.Count > 0) builder.AddKeyUp(stack.Pop());
        }

        private void KeysPress(InputBuilder builder, IEnumerable<VirtualKeyCode> keyCodes)
        {
            if (keyCodes == null) return;
            foreach (var key in keyCodes) builder.AddKeyPress(key);
        }

        /// <summary>
        /// Sends the list of <see cref="INPUT"/> messages using the <see cref="IInputMessageDispatcher"/> instance.
        /// </summary>
        /// <param name="inputList">The <see cref="System.Array"/> of <see cref="INPUT"/> messages to send.</param>
        private void SendSimulatedInput(INPUT[] inputList)
        {
            _messageDispatcher.DispatchInput(inputList);
        }

        /// <summary>
        /// Calls the Win32 SendInput method to simulate a KeyDown.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to press</param>
        public IKeyboardSimulator KeyDown(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyDown(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method to simulate a KeyUp.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to lift up</param>
        public IKeyboardSimulator KeyUp(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyUp(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method with a KeyDown and KeyUp message in the same input sequence in order to simulate a Key PRESS.
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> to press</param>
        public IKeyboardSimulator KeyPress(VirtualKeyCode keyCode)
        {
            var inputList = new InputBuilder().AddKeyPress(keyCode).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a key press for each of the specified key codes in the order they are specified.
        /// </summary>
        /// <param name="keyCodes"></param>
        public IKeyboardSimulator KeyPress(params VirtualKeyCode[] keyCodes)
        {
            var builder = new InputBuilder();
            KeysPress(builder, keyCodes);
            SendSimulatedInput(builder.ToArray());
            return this;
        }

        /// <summary>
        /// Simulates a simple modified keystroke like CTRL-C where CTRL is the modifierKey and C is the key.
        /// The flow is Modifier KeyDown, Key Press, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKeyCode">The modifier key</param>
        /// <param name="keyCode">The key to simulate</param>
        public IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKeyCode, VirtualKeyCode keyCode)
        {
            ModifiedKeyStroke(new[] { modifierKeyCode }, new[] { keyCode });
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers and one key like CTRL-ALT-C where CTRL and ALT are the modifierKeys and C is the key.
        /// The flow is Modifiers KeyDown in order, Key Press, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of modifier keys</param>
        /// <param name="keyCode">The key to simulate</param>
        public IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode)
        {
            ModifiedKeyStroke(modifierKeyCodes, new[] {keyCode});
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there is one modifier and multiple keys like CTRL-K-C where CTRL is the modifierKey and K and C are the keys.
        /// The flow is Modifier KeyDown, Keys Press in order, Modifier KeyUp.
        /// </summary>
        /// <param name="modifierKey">The modifier key</param>
        /// <param name="keyCodes">The list of keys to simulate</param>
        public IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKey, IEnumerable<VirtualKeyCode> keyCodes)
        {
            ModifiedKeyStroke(new [] {modifierKey}, keyCodes);
            return this;
        }

        /// <summary>
        /// Simulates a modified keystroke where there are multiple modifiers and multiple keys like CTRL-ALT-K-C where CTRL and ALT are the modifierKeys and K and C are the keys.
        /// The flow is Modifiers KeyDown in order, Keys Press in order, Modifiers KeyUp in reverse order.
        /// </summary>
        /// <param name="modifierKeyCodes">The list of modifier keys</param>
        /// <param name="keyCodes">The list of keys to simulate</param>
        public IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, IEnumerable<VirtualKeyCode> keyCodes)
        {
            var builder = new InputBuilder();
            ModifiersDown(builder, modifierKeyCodes);
            KeysPress(builder, keyCodes);
            ModifiersUp(builder, modifierKeyCodes);

            SendSimulatedInput(builder.ToArray());
            return this;
        }

        /// <summary>
        /// Calls the Win32 SendInput method with a stream of KeyDown and KeyUp messages in order to simulate uninterrupted text entry via the keyboard.
        /// </summary>
        /// <param name="text">The text to be simulated.</param>
        public IKeyboardSimulator TextEntry(string text)
        {
            if (text.Length > UInt32.MaxValue / 2) throw new ArgumentException(string.Format("The text parameter is too long. It must be less than {0} characters.", UInt32.MaxValue / 2), "text");
            var inputList = new InputBuilder().AddCharacters(text).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a single character text entry via the keyboard.
        /// </summary>
        /// <param name="character">The unicode character to be simulated.</param>
        public IKeyboardSimulator TextEntry(char character)
        {
            var inputList = new InputBuilder().AddCharacter(character).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="millsecondsTimeout">The number of milliseconds to wait.</param>
        public IKeyboardSimulator Sleep(int millsecondsTimeout)
        {
            Thread.Sleep(millsecondsTimeout);
            return this;
        }

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="timeout">The time to wait.</param>
        public IKeyboardSimulator Sleep(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            return this;
        }
    }
}
namespace WindowsInput
{
    /// <summary>
    /// The mouse button
    /// </summary>
    public enum MouseButton
    {
        /// <summary>
        /// Left mouse button
        /// </summary>
        LeftButton,

        /// <summary>
        /// Middle mouse button
        /// </summary>
        MiddleButton,

        /// <summary>
        /// Right moust button
        /// </summary>
        RightButton,
    }
}
namespace WindowsInput
{
    /// <summary>
    /// Implements the <see cref="IMouseSimulator"/> interface by calling the an <see cref="IInputMessageDispatcher"/> to simulate Mouse gestures.
    /// </summary>
    public class MouseSimulator : IMouseSimulator
    {
        private const int MouseWheelClickSize = 120;

        private readonly IInputSimulator _inputSimulator;

        /// <summary>
        /// The instance of the <see cref="IInputMessageDispatcher"/> to use for dispatching <see cref="INPUT"/> messages.
        /// </summary>
        private readonly IInputMessageDispatcher _messageDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseSimulator"/> class using an instance of a <see cref="WindowsInputMessageDispatcher"/> for dispatching <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="inputSimulator">The <see cref="IInputSimulator"/> that owns this instance.</param>
        public MouseSimulator(IInputSimulator inputSimulator)
        {
            if (inputSimulator == null) throw new ArgumentNullException("inputSimulator");

            _inputSimulator = inputSimulator;
            _messageDispatcher = new WindowsInputMessageDispatcher();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseSimulator"/> class using the specified <see cref="IInputMessageDispatcher"/> for dispatching <see cref="INPUT"/> messages.
        /// </summary>
        /// <param name="inputSimulator">The <see cref="IInputSimulator"/> that owns this instance.</param>
        /// <param name="messageDispatcher">The <see cref="IInputMessageDispatcher"/> to use for dispatching <see cref="INPUT"/> messages.</param>
        /// <exception cref="InvalidOperationException">If null is passed as the <paramref name="messageDispatcher"/>.</exception>
        internal MouseSimulator(IInputSimulator inputSimulator, IInputMessageDispatcher messageDispatcher)
        {
            if (inputSimulator == null)
                throw new ArgumentNullException("inputSimulator");

            if (messageDispatcher == null)
                throw new InvalidOperationException(
                    string.Format("The {0} cannot operate with a null {1}. Please provide a valid {1} instance to use for dispatching {2} messages.",
                    typeof(MouseSimulator).Name, typeof(IInputMessageDispatcher).Name, typeof(INPUT).Name));

            _inputSimulator = inputSimulator;
            _messageDispatcher = messageDispatcher;
        }

        /// <summary>
        /// Gets the <see cref="IKeyboardSimulator"/> instance for simulating Keyboard input.
        /// </summary>
        /// <value>The <see cref="IKeyboardSimulator"/> instance.</value>
        public IKeyboardSimulator Keyboard { get { return _inputSimulator.Keyboard; } }

        /// <summary>
        /// Sends the list of <see cref="INPUT"/> messages using the <see cref="IInputMessageDispatcher"/> instance.
        /// </summary>
        /// <param name="inputList">The <see cref="System.Array"/> of <see cref="INPUT"/> messages to send.</param>
        private void SendSimulatedInput(INPUT[] inputList)
        {
            _messageDispatcher.DispatchInput(inputList);
        }

        /// <summary>
        /// Simulates mouse movement by the specified distance measured as a delta from the current mouse location in pixels.
        /// </summary>
        /// <param name="pixelDeltaX">The distance in pixels to move the mouse horizontally.</param>
        /// <param name="pixelDeltaY">The distance in pixels to move the mouse vertically.</param>
        public IMouseSimulator MoveMouseBy(int pixelDeltaX, int pixelDeltaY)
        {
            var inputList = new InputBuilder().AddRelativeMouseMovement(pixelDeltaX, pixelDeltaY).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates mouse movement to the specified location on the primary display device.
        /// </summary>
        /// <param name="absoluteX">The destination's absolute X-coordinate on the primary display device where 0 is the extreme left hand side of the display device and 65535 is the extreme right hand side of the display device.</param>
        /// <param name="absoluteY">The destination's absolute Y-coordinate on the primary display device where 0 is the top of the display device and 65535 is the bottom of the display device.</param>
        public IMouseSimulator MoveMouseTo(double absoluteX, double absoluteY)
        {
            var inputList = new InputBuilder().AddAbsoluteMouseMovement((int)Math.Truncate(absoluteX), (int)Math.Truncate(absoluteY)).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates mouse movement to the specified location on the Virtual Desktop which includes all active displays.
        /// </summary>
        /// <param name="absoluteX">The destination's absolute X-coordinate on the virtual desktop where 0 is the left hand side of the virtual desktop and 65535 is the extreme right hand side of the virtual desktop.</param>
        /// <param name="absoluteY">The destination's absolute Y-coordinate on the virtual desktop where 0 is the top of the virtual desktop and 65535 is the bottom of the virtual desktop.</param>
        public IMouseSimulator MoveMouseToPositionOnVirtualDesktop(double absoluteX, double absoluteY)
        {
            var inputList = new InputBuilder().AddAbsoluteMouseMovementOnVirtualDesktop((int)Math.Truncate(absoluteX), (int)Math.Truncate(absoluteY)).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse left button down gesture.
        /// </summary>
        public IMouseSimulator LeftButtonDown()
        {
            var inputList = new InputBuilder().AddMouseButtonDown(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse left button up gesture.
        /// </summary>
        public IMouseSimulator LeftButtonUp()
        {
            var inputList = new InputBuilder().AddMouseButtonUp(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse left-click gesture.
        /// </summary>
        public IMouseSimulator LeftButtonClick()
        {
            var inputList = new InputBuilder().AddMouseButtonClick(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse left button double-click gesture.
        /// </summary>
        public IMouseSimulator LeftButtonDoubleClick()
        {
            var inputList = new InputBuilder().AddMouseButtonDoubleClick(MouseButton.LeftButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse right button down gesture.
        /// </summary>
        public IMouseSimulator RightButtonDown()
        {
            var inputList = new InputBuilder().AddMouseButtonDown(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse right button up gesture.
        /// </summary>
        public IMouseSimulator RightButtonUp()
        {
            var inputList = new InputBuilder().AddMouseButtonUp(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse right button click gesture.
        /// </summary>
        public IMouseSimulator RightButtonClick()
        {
            var inputList = new InputBuilder().AddMouseButtonClick(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse right button double-click gesture.
        /// </summary>
        public IMouseSimulator RightButtonDoubleClick()
        {
            var inputList = new InputBuilder().AddMouseButtonDoubleClick(MouseButton.RightButton).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse X button down gesture.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        public IMouseSimulator XButtonDown(int buttonId)
        {
            var inputList = new InputBuilder().AddMouseXButtonDown(buttonId).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse X button up gesture.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        public IMouseSimulator XButtonUp(int buttonId)
        {
            var inputList = new InputBuilder().AddMouseXButtonUp(buttonId).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse X button click gesture.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        public IMouseSimulator XButtonClick(int buttonId)
        {
            var inputList = new InputBuilder().AddMouseXButtonClick(buttonId).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse X button double-click gesture.
        /// </summary>
        /// <param name="buttonId">The button id.</param>
        public IMouseSimulator XButtonDoubleClick(int buttonId)
        {
            var inputList = new InputBuilder().AddMouseXButtonDoubleClick(buttonId).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates mouse vertical wheel scroll gesture.
        /// </summary>
        /// <param name="scrollAmountInClicks">The amount to scroll in clicks. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user.</param>
        public IMouseSimulator VerticalScroll(int scrollAmountInClicks)
        {
            var inputList = new InputBuilder().AddMouseVerticalWheelScroll(scrollAmountInClicks * MouseWheelClickSize).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Simulates a mouse horizontal wheel scroll gesture. Supported by Windows Vista and later.
        /// </summary>
        /// <param name="scrollAmountInClicks">The amount to scroll in clicks. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left.</param>
        public IMouseSimulator HorizontalScroll(int scrollAmountInClicks)
        {
            var inputList = new InputBuilder().AddMouseHorizontalWheelScroll(scrollAmountInClicks * MouseWheelClickSize).ToArray();
            SendSimulatedInput(inputList);
            return this;
        }

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="millsecondsTimeout">The number of milliseconds to wait.</param>
        public IMouseSimulator Sleep(int millsecondsTimeout)
        {
            Thread.Sleep(millsecondsTimeout);
            return this;
        }

        /// <summary>
        /// Sleeps the executing thread to create a pause between simulated inputs.
        /// </summary>
        /// <param name="timeout">The time to wait.</param>
        public IMouseSimulator Sleep(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
            return this;
        }
    }
}
namespace WindowsInput
{
    /// <summary>
    /// An implementation of <see cref="IInputDeviceStateAdaptor"/> for Windows by calling the native <see cref="NativeMethods.GetKeyState"/> and <see cref="NativeMethods.GetAsyncKeyState"/> methods.
    /// </summary>
    public class WindowsInputDeviceStateAdaptor : IInputDeviceStateAdaptor
    {

        /// <summary>
        /// Determines whether the specified key is up or down by calling the GetKeyState function. (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is down; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The key status returned from this function changes as a thread reads key messages from its message queue. The status does not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        /// An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        /// To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        /// An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for Bthe nVirtKey parameter. This gives the status of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left and right instances of those keys. 
        /// VK_LSHIFT
        /// VK_RSHIFT
        /// VK_LCONTROL
        /// VK_RCONTROL
        /// VK_LMENU
        /// VK_RMENU
        /// 
        /// These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public bool IsKeyDown(VirtualKeyCode keyCode)
        {
            Int16 result = NativeMethods.GetKeyState((UInt16)keyCode);
            return (result < 0);
        }

        /// <summary>
        /// Determines whether the specified key is up or downby calling the <see cref="NativeMethods.GetKeyState"/> function. (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is up; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The key status returned from this function changes as a thread reads key messages from its message queue. The status does not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        /// An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        /// To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        /// An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for Bthe nVirtKey parameter. This gives the status of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left and right instances of those keys. 
        /// VK_LSHIFT
        /// VK_RSHIFT
        /// VK_LCONTROL
        /// VK_RCONTROL
        /// VK_LMENU
        /// VK_RMENU
        /// 
        /// These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public bool IsKeyUp(VirtualKeyCode keyCode)
        {
            return !IsKeyDown(keyCode);
        }

        /// <summary>
        /// Determines whether the physical key is up or down at the time the function is called regardless of whether the application thread has read the keyboard event from the message pump by calling the <see cref="NativeMethods.GetAsyncKeyState"/> function. (See: http://msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is down; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The GetAsyncKeyState function works with mouse buttons. However, it checks on the state of the physical mouse buttons, not on the logical mouse buttons that the physical buttons are mapped to. For example, the call GetAsyncKeyState(VK_LBUTTON) always returns the state of the left physical mouse button, regardless of whether it is mapped to the left or right logical mouse button. You can determine the system's current mapping of physical mouse buttons to logical mouse buttons by calling 
        /// Copy CodeGetSystemMetrics(SM_SWAPBUTTON) which returns TRUE if the mouse buttons have been swapped.
        /// 
        /// Although the least significant bit of the return value indicates whether the key has been pressed since the last query, due to the pre-emptive multitasking nature of Windows, another application can call GetAsyncKeyState and receive the "recently pressed" bit instead of your application. The behavior of the least significant bit of the return value is retained strictly for compatibility with 16-bit Windows applications (which are non-preemptive) and should not be relied upon.
        /// 
        /// You can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the vKey parameter. This gives the state of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. 
        /// 
        /// Windows NT/2000/XP: You can use the following virtual-key code constants as values for vKey to distinguish between the left and right instances of those keys. 
        /// 
        /// Code Meaning 
        /// VK_LSHIFT Left-shift key. 
        /// VK_RSHIFT Right-shift key. 
        /// VK_LCONTROL Left-control key. 
        /// VK_RCONTROL Right-control key. 
        /// VK_LMENU Left-menu key. 
        /// VK_RMENU Right-menu key. 
        /// 
        /// These left- and right-distinguishing constants are only available when you call the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public bool IsHardwareKeyDown(VirtualKeyCode keyCode)
        {
            var result = NativeMethods.GetAsyncKeyState((UInt16)keyCode);
            return (result < 0);
        }

        /// <summary>
        /// Determines whether the physical key is up or down at the time the function is called regardless of whether the application thread has read the keyboard event from the message pump by calling the <see cref="NativeMethods.GetAsyncKeyState"/> function. (See: http://msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the key is up; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The GetAsyncKeyState function works with mouse buttons. However, it checks on the state of the physical mouse buttons, not on the logical mouse buttons that the physical buttons are mapped to. For example, the call GetAsyncKeyState(VK_LBUTTON) always returns the state of the left physical mouse button, regardless of whether it is mapped to the left or right logical mouse button. You can determine the system's current mapping of physical mouse buttons to logical mouse buttons by calling 
        /// Copy CodeGetSystemMetrics(SM_SWAPBUTTON) which returns TRUE if the mouse buttons have been swapped.
        /// 
        /// Although the least significant bit of the return value indicates whether the key has been pressed since the last query, due to the pre-emptive multitasking nature of Windows, another application can call GetAsyncKeyState and receive the "recently pressed" bit instead of your application. The behavior of the least significant bit of the return value is retained strictly for compatibility with 16-bit Windows applications (which are non-preemptive) and should not be relied upon.
        /// 
        /// You can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the vKey parameter. This gives the state of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. 
        /// 
        /// Windows NT/2000/XP: You can use the following virtual-key code constants as values for vKey to distinguish between the left and right instances of those keys. 
        /// 
        /// Code Meaning 
        /// VK_LSHIFT Left-shift key. 
        /// VK_RSHIFT Right-shift key. 
        /// VK_LCONTROL Left-control key. 
        /// VK_RCONTROL Right-control key. 
        /// VK_LMENU Left-menu key. 
        /// VK_RMENU Right-menu key. 
        /// 
        /// These left- and right-distinguishing constants are only available when you call the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public bool IsHardwareKeyUp(VirtualKeyCode keyCode)
        {
            return !IsHardwareKeyDown(keyCode);
        }

        /// <summary>
        /// Determines whether the toggling key is toggled on (in-effect) or not by calling the <see cref="NativeMethods.GetKeyState"/> function.  (See: http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        /// </summary>
        /// <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        /// <returns>
        /// 	<c>true</c> if the toggling key is toggled on (in-effect); otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The key status returned from this function changes as a thread reads key messages from its message queue. The status does not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        /// An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        /// To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        /// An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, and VK_MENU as values for the nVirtKey parameter. This gives the status of the SHIFT, CTRL, or ALT keys without distinguishing between left and right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left and right instances of those keys. 
        /// VK_LSHIFT
        /// VK_RSHIFT
        /// VK_LCONTROL
        /// VK_RCONTROL
        /// VK_LMENU
        /// VK_RMENU
        /// 
        /// These left- and right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, and MapVirtualKey functions. 
        /// </remarks>
        public bool IsTogglingKeyInEffect(VirtualKeyCode keyCode)
        {
            Int16 result = NativeMethods.GetKeyState((UInt16)keyCode);
            return (result & 0x01) == 0x01;
        }
    }
}