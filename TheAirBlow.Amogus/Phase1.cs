// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using TheAirBlow.Zigman;
using TheAirBlow.Zigman.Attributes;

namespace TheAirBlow.Amogus;

public class Phase1 : Payloads
{
    [Payload("iconSpam", SafetyLevel.Safe)]
    [UseQueue(3, 10000)]
    public void IconsSpam()
    {
        var icon = LoadIcon(IntPtr.Zero, Random.Next(32512, 32518)); 
        DrawIcon(DisplayHandle, Random.Next(0, Width), Random.Next(0, Height), icon);
        Thread.Sleep(10);
    }
    
    [Payload("patBltSpam", SafetyLevel.Safe)]
    [UseQueue(4, 10000)]
    public void PatBltSpam()
    {
        var brush = CreateSolidBrush((uint)Random.Next(0, int.MaxValue));
        SelectObject(DisplayHandle, brush);
        PatBlt(DisplayHandle, 0, 0, 
            Width, Height, 0x5A0049u);
        DeleteObject(brush);
        Thread.Sleep(100);
    }
    
    [Payload("rngChunks", SafetyLevel.Safe)]
    [UseQueue(5, 10000)]
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
}