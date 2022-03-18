// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Zigman.Engine.Legacy;

public partial class Trojan
{
    public class ConfigurationClass
    {
        /// <summary>
        /// Automatically hide the windows after warning is shown
        /// </summary>
        public bool HideWindowAfterWarning = true;

        /// <summary>
        /// How many threads you want to utilize.
        /// We won't create them until it is actually
        /// required.
        /// </summary>
        public int CountOfThreads;
        
        /// <summary>
        /// Creator of this trojan
        /// </summary>
        public string Creator = "";
        
        /// <summary>
        /// Name of this trojan
        /// </summary>
        public string Name = "";
    }

    /// <summary>
    /// Zigman configuration
    /// </summary>
    public ConfigurationClass Configuration = new();

    /// <summary>
    /// Safety level chosed by the user
    /// </summary>
    private Payload.SafetyLevel _safetyLevel;
    
    public IntPtr DisplayHandle = CreateDC();
    public Graphics DisplayGraphics = null!;
    
    public void Start()
    {
        Console.Title = "Zigman.Engine.Legacy | Warning";
        Console.WriteLine("/------------------------------------\\");
        Console.WriteLine("| Zigman.Engine.Legacy | Made by TheAirBlow |");
        Console.WriteLine("|   A trojan running Zigman Engine   |");
        Console.WriteLine("\\------------------------------------/");
        Console.WriteLine($"\"{Configuration.Name}\" made by {Configuration.Creator}");

        Console.WriteLine();
        if (Environment.OSVersion.Platform != PlatformID.Win32NT) {
            Console.WriteLine("FATAL: This software is windows-only!");
            return;
        }

        if (Environment.OSVersion.Version.Major < 6 &&
            Environment.OSVersion.Version.Minor < 1) {
            Console.WriteLine("FATAL: Windows 7 or newer is required!");
            return;
        }
        
        // Elevation
        if (!IsAdministrator()) {
            Beep(1000, 100);
            Console.WriteLine("Administrator priveleges are required!");
            Console.WriteLine("Press \"Yes\" in the UAC promt.");
            var proc = new Process();
            proc.StartInfo.FileName = "powershell.exe";
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Arguments = $"-Command Start-Process \"{Application.ExecutablePath}\" -Verb runas";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
            return;
        }
        
        Beep(700, 100);
        Console.WriteLine("You are about to run a trojan, made only for the purposes of entertainment and education.");
        Console.WriteLine("There may be flashing lights, which may cause photosensitive epilepsy.");
        Console.WriteLine("You can select to run only safe payloads, or destroy your system.");
        Console.WriteLine("I am not responsible for any damages.");
        Console.WriteLine("Press Enter to continue.");
        Console.WriteLine("Close the window to exit.");
        Console.ReadLine();

        Console.CursorTop -= 7;
        Beep(700, 100);
        Console.WriteLine("We would also like to warn you about the creator.                                        ");
        Console.WriteLine("He could possibly lie about the safety level.                         ");
        Console.WriteLine("For example: MBR overwrite as a safe payload.                    ");
        Console.WriteLine("Press Enter to continue.             ");
        Console.WriteLine("Close the window to exit.");
        Console.WriteLine("                         ");
        Console.ReadLine();
        Console.CursorTop -= 8;

        ConsoleTag:
        Beep(700, 100);
        Console.WriteLine();
        Console.WriteLine("Also type \"SURE\", else it will be rejected!      ");
        Console.WriteLine("0 - Completely safe                                ");
        Console.WriteLine("1 - Destructive, but revertable                    ");
        Console.WriteLine("2 - Destructive, permanent                         ");
        Console.WriteLine("# - Exit                                           ");
        Console.Write("[0/1/2/#]> ");
        var line = Console.ReadLine();
        
        if (string.IsNullOrEmpty(line)) {
            Beep(1000, 100);
            Console.CursorTop--;
            Console.Write("[0/1/2/#]> ");
            Console.CursorTop -= 6;
            goto ConsoleTag;
        }
        
        if (line == "#") {
            Beep(1000, 100);
            return;
        }

        if (!line.Contains("SURE")) {
            Beep(1000, 100);
            Console.CursorTop--;
            Console.Write("[0/1/2/#]> ");
            for (var i = 0; i < line.Length; i++)
                Console.Write(" ");
            Console.CursorTop -= 6;
            goto ConsoleTag;
        }

        line = line.Replace("SURE", "");

        if (!int.TryParse(line, out _)) {
            Beep(1000, 100);
            Console.CursorTop--;
            Console.Write("[0/1/2/#]> ");
            for (var i = 0; i < line.Length; i++)
                Console.Write(" ");
            Console.CursorTop -= 6;
            goto ConsoleTag;
        }

        if (int.Parse(line) >= 3) {
            Beep(1000, 100);
            Console.CursorTop--;
            Console.Write("[0/1/2/#]> ");
            for (var i = 0; i < line.Length; i++)
                Console.Write(" ");
            Console.CursorTop -= 6;
            goto ConsoleTag;
        }
        
        if (Enum.TryParse(line, out Payload.SafetyLevel level)) _safetyLevel = level;
        else {
            Beep(1000, 100);
            Console.CursorTop--;
            Console.Write("[0/1/2/#]> ");
            for (var i = 0; i < line.Length; i++)
                Console.Write(" ");
            Console.CursorTop -= 6;
            goto ConsoleTag;
        }
        
        Beep(700, 100);
        Console.Clear();
        Console.Title = "Zigman.Engine.Legacy | Running";
        Console.WriteLine("/------------------------------------\\");
        Console.WriteLine("| Zigman.Engine.Legacy | Made by TheAirBlow |");
        Console.WriteLine("|   A trojan running Zigman Engine   |");
        Console.WriteLine("\\------------------------------------/");
        Console.WriteLine($"[Info] Creator: {Configuration.Creator}");
        Console.WriteLine($"[Info] Name: {Configuration.Name}");
        if (Configuration.HideWindowAfterWarning)
            ShowWindow(GetConsoleWindow(), 0);
        DisplayGraphics = Graphics.FromHdc(DisplayHandle);
        UserInit();
    }
    
    /// <summary>
    /// Load your payloads and assets here.
    /// </summary>
    public virtual void UserInit() { }
}