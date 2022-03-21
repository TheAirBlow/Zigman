// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Drawing;
using System.Drawing.Drawing2D;
using TheAirBlow.Zigman;
using TheAirBlow.Zigman.Attributes;

namespace TheAirBlow.Amogus;

public class Phase4 : Payloads
{
    [Payload("screenRotate", SafetyLevel.Safe)]
    [UseQueue(1, 10000)]
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

    private IntPtr? _destTriangle;
    private IntPtr? _bitmapTriangle;
    private PointF? _locationTriangle;
    private POINT[][]? _pointsTriangle;
    private bool _firstTriangle = true;
    private int _runTimeTriangle;

    [Payload("triangle", SafetyLevel.Safe)]
    [UseQueue(2, 5000)]
    public void LeSolarisTriangle()
    {
        // Bitmap stuff
        _destTriangle ??= CreateCompatibleDC(DisplayHandle);
        _bitmapTriangle ??= CreateCompatibleBitmap(
                            DisplayHandle, Width, Height);
        if (_firstTriangle) {
            SelectObject(_destTriangle.Value, _bitmapTriangle.Value);
            BitBlt(_destTriangle.Value, 0, 0, Width, Height, 
                DisplayHandle, 0, 0, RasterOperation.SrcCopy);
        }
        
        // Location of the shape
        _locationTriangle ??= new PointF(
            Random.Next(0, Width),
            Random.Next(0, Height)); 
        var x = (int)_locationTriangle.Value.X;
        var y = (int)_locationTriangle.Value.Y;
        var listPoints = new List<POINT[]>();
        
        // Add a octagon with size m to listPoints
        void AddPoints(int m) {
            listPoints.Add(new[] {
                new POINT(x - 0     , y + 10 * m),
                new POINT(x - 10 * m, y - 8 * m),
                new POINT(x + 10 * m, y - 8 * m),
                new POINT(x - 0     , y + 10 * m)
            });
        }
        
        // Draw _points on screen
        void Draw(bool rotate) {
            for (var j = 0; j < _pointsTriangle.Length; j++) {
                var i = _pointsTriangle[j];
                var converted = i.ToPoint();
                if (rotate) {
                    var matrix = new Matrix();
                    matrix.RotateAt(5, new PointF(x, y));
                    matrix.TransformPoints(converted);
                }
                var native = converted.ToNativePoint();
                _pointsTriangle[j] = native;
                var polygon = CreatePolygonRgn(native,
                    native.Length, 1);
                SelectClipRgn(_destTriangle.Value, polygon);
                InvertRgn(_destTriangle.Value, polygon);
            }
        }

        // Cache the points
        if (_pointsTriangle == null) {
            for (var i = 16; i > 0; i--)
                AddPoints(i);
            _pointsTriangle = listPoints.ToArray();
        }
        
        Draw(true);
        BitBlt(DisplayHandle, x-400/2, y-400/2, 400, 400,
            _destTriangle.Value, x-400/2, y-400/2, RasterOperation.SrcCopy);
        Draw(false); Thread.Sleep(200);
        _firstTriangle = false; _runTimeTriangle++;
        if (_runTimeTriangle == 25) {
            _locationTriangle = null;
            _pointsTriangle = null;
            _bitmapTriangle = null;
            _firstTriangle = true;
            _destTriangle = null;
            _runTimeTriangle = 0;
        }
    }

    [Payload("screenSmelter", SafetyLevel.Safe)]
    [UseQueue(5, 10000)]
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
}