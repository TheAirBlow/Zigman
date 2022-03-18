// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Zigman.Engine.Legacy;

/// <summary>
/// Wallpaper changer
/// </summary>
[SuppressMessage("Interoperability", "CA1416", MessageId = "Проверка совместимости платформы")]
public partial class Trojan
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    /// <summary>
    /// Wallpaper style
    /// </summary>
    public enum Style
    {
        Tiled,
        Centered,
        Stretched
    }

    /// <summary>
    /// Set wallpaper
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="style">Style</param>
    /// <exception cref="InvalidOperationException">Unable to change wallpaper style</exception>
    /// <exception cref="Exception">Unable to set wallpaper</exception>
    public void SetWallpaper(string path, Style style)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true) 
                          ?? throw new InvalidOperationException("Unable to change wallpaper style!");
        switch (style) {
            case Style.Stretched:
                key.SetValue(@"WallpaperStyle", "2");
                key.SetValue(@"TileWallpaper", "2");
                break;
            case Style.Centered:
                key.SetValue(@"WallpaperStyle", "1");
                key.SetValue(@"TileWallpaper", "0");
                break;
            case Style.Tiled:
                key.SetValue(@"WallpaperStyle", "1");
                key.SetValue(@"TileWallpaper", "1");
                break;
        }

        if (SystemParametersInfo(20, 0, path, 0x01 | 0x02) != 1)
            throw new Exception("Unable to set wallpaper!");
    }
}