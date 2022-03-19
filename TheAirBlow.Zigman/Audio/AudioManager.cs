// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Media;
using System.Text;

namespace TheAirBlow.Zigman.Audio;

/// <summary>
/// Helps out with audio management and streaming,
/// also has support for ByteBeat builtin.
/// </summary>
#pragma warning disable CS8618
public class AudioManager : IDisposable
{
    /// <summary>
    /// Audio Stream
    /// </summary>
    private Stream? _audioStream;
    
    /// <summary>
    /// SoundPlayer Instance
    /// </summary>
    private SoundPlayer _player;
    
    /// <summary>
    /// Initializes AudioManager
    /// </summary>
    public AudioManager()
        => Initialize();

    /// <summary>
    /// Initialization
    /// </summary>
    private void Initialize()
    {
        if (_audioStream != null)
            _audioStream.Dispose();
        _audioStream = new MemoryStream();
        _player = new(_audioStream);
    }

    /// <summary>
    /// Stops the playback and reinitializes
    /// the AudioManager for next operations
    /// </summary>
    public void StopPlayback()
    {
        _player.Stop();
        Initialize();
    }

    /// <summary>
    /// Play audio from stream
    /// </summary>
    /// <param name="stream">Stream</param>
    public void PlayStream(Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        _audioStream = stream;
        _player.PlaySync();
    }

    /// <summary>
    /// Play audio from file
    /// </summary>
    /// <param name="path">Path to the file</param>
    /// <param name="loop">Loop the track</param>
    public void PlayFile(string path, bool loop = false)
    {
        StopPlayback(); _player = new(path);
        if (loop) _player.PlayLooping();
        else _player.Play();
    }
    
    /// <summary>
    /// Play ByteBeat
    /// </summary>
    /// <param name="func">Function</param>
    /// <param name="sampleRate">Sample Rate</param>
    /// <param name="bitsPerSample">Bits per Sample</param>
    /// <param name="seconds">For how long to play ByteBeat.</param>
    /// <param name="loop">Loop the track</param>
    public void ByteBeat(Func<int, byte> func, int sampleRate = 8000, 
        int bitsPerSample = 8, int seconds = 5, int channels = 1, 
        bool loop = false)
    {
        StopPlayback();
        using var writer = new BinaryWriter(_audioStream!);
        
        // The WAVE header
        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(0);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));
        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);

        // Information about the data supplied
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * bitsPerSample / 8);
        writer.Write((short)(channels * bitsPerSample / 8));
        writer.Write((short)bitsPerSample);

        // The ByteBeat itself
        writer.Write(Encoding.ASCII.GetBytes("data"));
        var data = new byte[sampleRate * seconds];
        for (var t = 0; t < data.Length; t++)
            data[t] = func(t);
        
        writer.Write(data.Length * channels * bitsPerSample / 8);
        writer.Write(data);
        
        // Write the stream's length
        writer.Seek(4, SeekOrigin.Begin);
        writer.Write((int)(writer.BaseStream.Length - 8));

        // Start playback
        _audioStream!.Seek(0, SeekOrigin.Begin);
        var memoryStream = (MemoryStream)_audioStream;
        if (loop) _player.PlayLooping();
        else _player.PlaySync();
    }

    /// <summary>
    /// Dispose stuff
    /// </summary>
    public void Dispose()
    {
        _audioStream?.Dispose();
        _player.Stop();
    }
}