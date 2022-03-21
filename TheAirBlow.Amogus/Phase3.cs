// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Drawing;
using System.Drawing.Drawing2D;
using TheAirBlow.Zigman;
using TheAirBlow.Zigman.Attributes;

namespace TheAirBlow.Amogus;

public class Phase3 : Payloads
{
    [Payload("mirroring", SafetyLevel.Safe)]
    [UseQueue(1, 10000)]
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
    
    private IntPtr? _destOctagon;
    private IntPtr? _bitmapOctagon;
    private PointF? _locationOctago;
    private POINT[][]? _pointsOctagon;
    private bool _firstOctagon = true;
    private int _runTimeOctagon;

    [Payload("octagon", SafetyLevel.Safe)]
    [UseQueue(2, 10000)]
    public void LeSolarisOctagon()
    {
        // Bitmap stuff
        _destOctagon ??= CreateCompatibleDC(DisplayHandle);
        _bitmapOctagon ??= CreateCompatibleBitmap(
                            DisplayHandle, Width, Height);
        if (_firstOctagon) {
            SelectObject(_destOctagon.Value, _bitmapOctagon.Value);
            BitBlt(_destOctagon.Value, 0, 0, Width, Height, 
                DisplayHandle, 0, 0, RasterOperation.SrcCopy);
        }
        
        // Location of the shape
        _locationOctago ??= new PointF(
            Random.Next(0, Width),
            Random.Next(0, Height)); 
        var x = (int)_locationOctago.Value.X;
        var y = (int)_locationOctago.Value.Y;
        var listPoints = new List<POINT[]>();
        
        // Add a octagon with size m to listPoints
        void AddPoints(int m) {
            listPoints.Add(new[] {
                new POINT(x - 0     , y + 20 * m),
                new POINT(x + 15 * m, y + 15 * m),
                new POINT(x + 20 * m, y + 0),
                new POINT(x + 15 * m, y - 15 * m),
                new POINT(x - 0     , y - 20 * m),
                new POINT(x - 15 * m, y - 15 * m),
                new POINT(x - 20 * m, y + 0),
                new POINT(x - 15 * m, y + 15 * m)
            });
        }
        
        // Draw _points on screen
        void Draw(bool rotate) {
            for (var j = 0; j < _pointsOctagon.Length; j++) {
                var i = _pointsOctagon[j];
                var converted = i.ToPoint();
                if (rotate) {
                    var matrix = new Matrix();
                    matrix.RotateAt(5, new PointF(x, y));
                    matrix.TransformPoints(converted);
                }
                var native = converted.ToNativePoint();
                _pointsOctagon[j] = native;
                var polygon = CreatePolygonRgn(native,
                    native.Length, 1);
                SelectClipRgn(_destOctagon.Value, polygon);
                InvertRgn(_destOctagon.Value, polygon);
            }
        }

        // Cache the points
        if (_pointsOctagon == null) {
            for (var i = 16; i > 0; i--)
                AddPoints(i);
            _pointsOctagon = listPoints.ToArray();
        }
        
        Draw(true);
        BitBlt(DisplayHandle, x-680/2, y-680/2, 680, 660,
            _destOctagon.Value, x-680/2, y-680/2, RasterOperation.SrcCopy);
        Draw(false); Thread.Sleep(200);
        _firstOctagon = false; _runTimeOctagon++;
        if (_runTimeOctagon == 25) {
            _locationOctago = null;
            _pointsOctagon = null;
            _bitmapOctagon = null;
            _firstOctagon = true;
            _destOctagon = null;
            _runTimeOctagon = 0;
        }
    }
    
    [Payload("invert", SafetyLevel.Safe)]
    [UseQueue(3, 10000)]
    public void InvertScreen()
    {
        StretchBlt(DisplayHandle, 0, 0, Width, Height, 
            DisplayHandle, 0, 0, Width, Height, RasterOperation.DstInvert);
        Thread.Sleep(250);
    }
    
    [Payload("wtf", SafetyLevel.Safe)]
    [UseQueue(4, 10000)]
    public void WhatTheFuck()
    {
        StretchBlt(DisplayHandle, Random.Next(-5, 5), Random.Next(-5, 5), Width, Height,
            DisplayHandle, 0, 0, Width, Height, 6684742);
        Thread.Sleep(100);
    }
    
    [Payload("tunneling", SafetyLevel.Safe)]
    [UseQueue(5, 10000)]
    public void TunnelingEffect()
    {
        var w = Width - 100;
        var h = Height - 100;
        StretchBlt(DisplayHandle, 50, 50, w, h, DisplayHandle, 
            0, 0, Width, Height, RasterOperation.SrcCopy);
        Thread.Sleep(100);
    }
}