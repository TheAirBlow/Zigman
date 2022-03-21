// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Windows.Forms;
using TheAirBlow.Zigman;
using TheAirBlow.Zigman.Attributes;

namespace TheAirBlow.Amogus;

public class Phase2 : Payloads
{
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
}