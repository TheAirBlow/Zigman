// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Timer = System.Timers.Timer;

namespace Zigman.Engine.Legacy;

/// <summary>
/// Payload queue
/// </summary>
public partial class Trojan
{
    /// <summary>
    /// An enqueued payload
    /// </summary>
    private class EnqueuedPayload
    {
        public int DelayBeforeNext;
        public string ID;
    }
    
    private int _index;
    private bool _enabled;
    private readonly Timer _timer = new();
    private List<EnqueuedPayload> _enqueuedPayloads = new();

    /// <summary>
    /// Enable payload queue
    /// </summary>
    /// <exception cref="Exception">Payload queue is already enabled</exception>
    public void EnablePayloadQueue()
    {
        if (_enabled)
            throw new Exception("Payload queue is already enabled!");
        _enabled = true;
    }

    /// <summary>
    /// Enqueue a payload
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="delayBeforeNext">Delay before next payload (in millis)</param>
    /// <exception cref="Exception">Payload queue is not enabled -or- Payload does not exist</exception>
    public void EnqueuePayload(string id, int delayBeforeNext)
    {
        if (!_enabled)
            throw new Exception("Payload queue is not enabled!");
        lock (_payloads) {
            if (!_payloads.ContainsKey(id)) {
                Console.WriteLine($"[EnqueuePayload] \"{id}\" enqueued, but not found");
                return;
            }
            _enqueuedPayloads.Add(new EnqueuedPayload {
                DelayBeforeNext = delayBeforeNext,
                ID = id
            });
        }
    }

    /// <summary>
    /// Start the queue timer
    /// </summary>
    /// <param name="delayBeforeStart">Delay before starting</param>
    /// <exception cref="Exception">Payload queue is not enabled</exception>
    public void StartQueue(int delayBeforeStart)
    {
        if (!_enabled)
            throw new Exception("Payload queue is not enabled!");

        Console.WriteLine($"[PayloadQueue] Starting queue timer in {delayBeforeStart} ms");
        _timer.Interval = delayBeforeStart;
        _timer.Elapsed += (_, _) => {
            lock (_enqueuedPayloads) lock (_payloads) {
                try {
                    if (!_payloads.ContainsKey(_enqueuedPayloads[_index].ID)) {
                        Console.WriteLine($"[PayloadQueue] Payload \"{_enqueuedPayloads[_index].ID}\" no longer exists!");
                        _enqueuedPayloads.RemoveAt(_index);
                        _index++;
                        if (_index <= _enqueuedPayloads.Count - 1) return;
                        Console.WriteLine("[PayloadQueue] No payloads left, stopping");
                        _timer.Stop();
                        return;
                    }
                    Console.WriteLine($"[PayloadQueue] Enabling payload \"{_enqueuedPayloads[_index].ID}\"");
                    _timer.Interval = _enqueuedPayloads[_index].DelayBeforeNext;
                    _payloads[_enqueuedPayloads[_index].ID].Enabled = true;
                    _index++;
                    if (_index > _enqueuedPayloads.Count - 1) {
                        Console.WriteLine("[PayloadQueue] No payloads left, stopping");
                        _timer.Stop();
                    } else Console.WriteLine($"[PayloadQueue] Next payload \"{_enqueuedPayloads[_index].ID}\" in {_timer.Interval} ms");
                } catch (Exception e) { Console.WriteLine($"[PayloadQueue] Exception occured: {e}"); }
            }
        };
        _timer.Start();
    }
}