# Clickless Mouse

## Requirements
**OS:** Windows XP, 2003, Vista, 2008, 7, 8, 8.1, 10<br/>
Works only in programs and games that are run in borderless or windowed mode (fullscreen mode is not supported)<br/>

## Download latest version
[Clickless Mouse 2.02 installer](https://github.com/ProperCode/clickless-mouse/blob/master/Download/Clickless%20Mouse%202.02.exe)<br/>
[Clickless Mouse 2.02 zip](https://github.com/ProperCode/clickless-mouse/blob/master/Download/Clickless%20Mouse%202.02.zip)<br/>

**Languages:** English, Polish

## Description

Clickless Mouse makes using a computer easier for people with repetitive strain injury, carpal tunnel syndrome, 
some motor disabilities and other health problems.<br/>

This application allows to use a mouse without clicking - by moving it only.<br/>

By reacting to user mouse movements this program simulates left/right mouse click, 
double left mouse click and left/right mouse button holding. <br/>

Clickless Mouse can be used with a virtual keyboard to type by moving mouse 
(e.g. [Free Virtual Keyboard](https://freevirtualkeyboard.com/)). <br/>

By using Clickless Mouse with a virtual keyboard the user can fully control a computer by moving mouse 
with a palm, a forearm or a foot.<br/>

When you want to click/hold a mouse button: stop moving the mouse, wait for the squares to appear and move the mouse cursor to the chosen square:
- Top center square = double left mouse button click
- Top left square = left mouse button click
- Top right square = right mouse button click
- Left square = left mouse button holding on/off
- Right square = right mouse button holding on/off<br/>
 
![alt text](https://raw.githubusercontent.com/ProperCode/clickless-mouse/master/Download/1en.jpg) 

When the mouse cursor is located inside a square long enough (depending on cursor time 
in square to register click setting), it automatically moves back to the previous position 
to simulate an action based on the square that was previously entered.<br/>

If the mouse cursor is too close to the top edge of the screen, squares for LMB click,
 RMB click and LMB double click are shown below the mouse cursor.<br/>

If the mouse cursor is too close to the left/right edge of the screen all squares size decrease so at least 25% of a square is visible. 
By default decreased square size cannot be lower than 60% of the normal size.
Squares size don't decrease if the mouse cursor is so close to the edge of 
the screen that smallest size isn't enough to show a square.<br/>

The squares that aren't needed can be disabled.<br/>

Screen panning - when this mode is on, moving the mouse curor to the edges of the screen presses keys:
 up (top edge), down (bottom edge), left (left edge), right (right edge). While screen panning is enabled 
 the squares don't show when the mouse cursor is located at the screen edge.<br/>

**First steps:**
1. Type your diagonal screen size and press 'Set recommended square size button'.
2. Decide which mouse functionality you need. Most users need 'LMB Click', 'RMB click, 'LMB click', 'LMB double click' and 'LMB holding'.
3. If you have a motor disability consider increasing 'Cursor idle time before squares appear', 
'Time to start mouse movement after squares appear' and 'Square size'.<br/>

**Lowest possible values (the program ignores lower values and uses following values instead):**
- Cursor idle time before squares appear [ms]: 100
- Time to start mouse movement after squares appear [ms]: 300
- Cursor time in square to register click [ms]: 10
- Size [px]: 10
- Border width [px]: 1
- Minimum size [%]: 10

![alt text](https://raw.githubusercontent.com/ProperCode/clickless-mouse/master/Download/2en.jpg)
