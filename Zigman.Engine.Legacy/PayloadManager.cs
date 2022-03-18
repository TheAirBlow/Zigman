// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Zigman.Engine.Legacy;

/// <summary>
/// Payload manager
/// </summary>
public partial class Trojan
{
    /// <summary>
    /// Thread/queue pair
    /// </summary>
    public class ThreadQueuePair
    {
        /// <summary>
        /// Thread
        /// </summary>
        public volatile Thread Thread = null!;
        
        /// <summary>
        /// Actions to be executed
        /// </summary>
        public volatile List<string> PayloadIDs = new();

        /// <summary>
        /// Index in the list
        /// </summary>
        public volatile int Index;

        /// <summary>
        /// Is stacking enabled for this thread
        /// </summary>
        public volatile bool StackingEnabled = true;

        /// <summary>
        /// Stop the thread if it got emptied
        /// (all payloads removed)
        /// </summary>
        public volatile bool GotEmpty = false;
    }

    /// <summary>
    /// All pairs
    /// </summary>
    private volatile Dictionary<int, ThreadQueuePair> _pairs = new();

    /// <summary>
    /// All payloads
    /// </summary>
    private volatile Dictionary<string, Payload> _payloads = new();

    /// <summary>
    /// Is initialized
    /// </summary>
    private bool _stopThreads;

    /// <summary>
    /// Threads stopped
    /// </summary>
    private volatile int _threadsStopped;

    /// <summary>
    /// Threads already created
    /// </summary>
    private int _threadsCreated;

    /// <summary>
    /// Create a thread/queue pair
    /// </summary>
    /// <param name="stacking">Is stacking enabled</param>
    private void CreateThreadQueuePair(bool stacking)
    {
        lock (_pairs) {
            var id = _pairs.Count;
            var pair = new ThreadQueuePair {
                Index = id,
                StackingEnabled = stacking,
                Thread = new Thread(() => {
                    try {
                        bool gotEmpty;
                        ThreadQueuePair pair;
                        lock (_pairs) {
                            pair = _pairs[id];
                            lock (pair) gotEmpty = pair.GotEmpty;
                        }
                        
                        while (!_stopThreads && !gotEmpty) {
                            lock (pair.PayloadIDs) {
                                foreach (var pid in pair.PayloadIDs) {
                                    lock (_payloads) {
                                        if (!_payloads.ContainsKey(pid)) {
                                            Console.WriteLine($"[Thread {id}] Payload \"{pid}\" got deleted!");
                                            pair.PayloadIDs.Remove(pid);
                                            if (pair.PayloadIDs.Count == 0) 
                                                pair.GotEmpty = true;
                                            break;
                                        }
                                        if (!_payloads[pid].Enabled) continue;
                                        try { _payloads[pid].Action(); }
                                        catch (Exception e) { Console.WriteLine($"[Thread {id}] Exception while running \"{pid}\": {e}"); }
                                    }
                                }
                            }
                        }
                        
                        if (gotEmpty) Console.WriteLine($"[Thread {id}] Stopping because thread got emptied");
                        else {
                            Console.WriteLine($"[Thread {id}] Stopping because of StopThreads");
                            Interlocked.Increment(ref _threadsStopped);
                        }
                        
                        lock (_pairs) _pairs.Remove(id);
                    } catch (Exception e) { Console.WriteLine($"[Thread {id}] Exception occured: {e}"); }
                })
            };
            _pairs.Add(id, pair);
            _threadsCreated++;
            _pairs[id].Thread.Start();
            Console.WriteLine($"[CreateThreadQueuePair] Created a new thread at index {id}");
        }
    }

    /// <summary>
    /// Throws a Payload into a thread with
    /// the smallest count of Payloads in it
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="action">Action</param>
    /// <exception cref="Exception">
    /// Payload already exists
    /// -or- Can't create more threads, but payload has stacking disabled
    /// -or- Found no threads with same stacking switch value as payload requires
    /// </exception>
    public void AddPayload(string id, Payload action)
    {
        if ((int)action.LevelOfSafety > (int)_safetyLevel) {
            Console.WriteLine($"[AddPayload] Ignoring payload \"{id}\" because of safety level");
            return;
        }

        lock (_payloads) {
            if (_payloads.ContainsKey(id))
                throw new Exception($"Payload \"{id}\" already exists!");
            _payloads.Add(id, action);
            Console.WriteLine($"[AddPayload] Payload \"{id}\" added to the database");
        }

        if (!action.CanBeStacked && _threadsCreated == Configuration.CountOfThreads)
            throw new Exception($"Can't create more threads, but \"{id}\" has stacking disabled!");
        if (_threadsCreated < Configuration.CountOfThreads)
            CreateThreadQueuePair(action.CanBeStacked);
        lock (_pairs) {
            var ordered = _pairs.OrderBy(x => {
                var (_, value) = x;
                lock (value) lock (value.PayloadIDs)
                    return value.PayloadIDs.Count;
            }).ToList();
            lock (ordered) {
                var selected = ordered.FirstOrDefault(x => x.Value.StackingEnabled == action.CanBeStacked);
                lock (selected.Value) {
                    if (selected.Value == null)
                        throw new Exception("Found no threads with same stacking switch value as payload requires!");
                    lock (selected.Value.PayloadIDs)
                        selected.Value.PayloadIDs.Add(id);
                    Console.WriteLine($"[AddPayload] Payload \"{id}\" added to thread {selected.Value.Index}");
                }
            }
        }
    }

    /// <summary>
    /// Enable a payload
    /// </summary>
    /// <param name="id">ID</param>
    /// <exception cref="Exception">Payload does not exist</exception>
    public void EnablePayload(string id)
    {
        lock (_payloads) {
            if (!_payloads.ContainsKey(id))
                throw new Exception($"Payload \"{id}\" does not exist!");
            _payloads[id].Enabled = true;
        }
    }

    /// <summary>
    /// Disable a payload
    /// </summary>
    /// <param name="id">ID</param>
    /// <exception cref="Exception">Payload does not exist</exception>
    public void DisablePayload(string id)
    {
        lock (_payloads) {
            if (!_payloads.ContainsKey(id))
                throw new Exception($"Payload \"{id}\" does not exist!");
            _payloads[id].Enabled = false;
        }
    }
    
    /// <summary>
    /// Completely delete a payload
    /// </summary>
    /// <param name="id">ID</param>
    /// <exception cref="Exception">Payload does not exist</exception>
    public void DeletePayload(string id)
    {
        lock (_payloads) {
            if (!_payloads.ContainsKey(id))
                throw new Exception($"Payload \"{id}\" does not exist!");
            _payloads.Remove(id);
        }
    }

    /// <summary>
    /// Stop all threads
    /// </summary>
    private void StopThreads()
    {
        Console.WriteLine($"[StopThreads] Stopping all threads...");
        _stopThreads = true;
        lock (_pairs) {
            while (_threadsStopped != _pairs.Count) { }
            _pairs.Clear();
        }
        Console.WriteLine($"[StopThreads] Done!");
    }
}