// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Reflection;
using Spectre.Console;

namespace TheAirBlow.Zigman;

/// <summary>
/// Manage the ThreadPool
/// </summary>
internal class ThreadsManager
{
    /// <summary>
    /// Information about a thread
    /// </summary>
    internal class ThreadInfo
    {
        /// <summary>
        /// The payloads to run
        /// </summary>
        public volatile Dictionary<string, Payloads.Payload> Payloads = new();

        /// <summary>
        /// Did payloads get modified?
        /// </summary>
        public volatile bool Dirty = true;

        /// <summary>
        /// Thread's ID
        /// </summary>
        public volatile int Identifier = -1;
    }
    
    /// <summary>
    /// In which thread a payload is located
    /// </summary>
    private Dictionary<string, int> _payloadLocation = new();
    
    /// <summary>
    /// All threads' information
    /// </summary>
    private volatile Dictionary<int, ThreadInfo> _threads = new();
    
    /// <summary>
    /// Amount of threads
    /// </summary>
    private int _threadsCount = 0;
    
    /// <summary>
    /// Flag all threads for shutdown
    /// </summary>
    public volatile bool Shutdown = false;

    /// <summary>
    /// Create a new instance of the ThreadsManager
    /// </summary>
    /// <param name="threadsCount">Amount of threads</param>
    internal ThreadsManager(int threadsCount)
    {
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Initializing ThreadsManager...");
        _threadsCount = threadsCount;
        ThreadPool.SetMaxThreads(_threadsCount,
            _threadsCount);
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Initialization done!");
    }

    /// <summary>
    /// Add a payload
    /// </summary>
    /// <param name="id">Payload's Identifier</param>
    /// <param name="payload">Payload to add</param>
    public void AddPayload(string id, Payloads.Payload payload)
    {
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Adding payload {id} to a thread...");
        CreateThread(); // Would do nothing if there are already enough threads
        
        lock (_threads) {
            var ordered = _threads.OrderBy(
                x => x.Value.Payloads.Count)
                .ToDictionary(x => x.Key, 
                x => x.Value);
            var thread = ordered.Values.ToList()[0];
            while (thread.Identifier == -1) {}
            if (payload.IsSubPayloads && thread.Payloads.Count != 0) {
                // We need to move all payloads from
                // this thread, because it would lock
                // up until the queue would end.
                ordered = ordered.Where(x 
                    => x.Key != thread.Identifier).ToDictionary(
                    x => x.Key, 
                    x => x.Value);
                var otherThread = ordered.Values.ToList()[0];
                foreach (var i in thread.Payloads) {
                    thread.Payloads.Remove(i.Key); // Move
                    otherThread.Payloads.Add(i.Key, i.Value);
                }

                // Mark the other dirty
                otherThread.Dirty = true;
            }
            
            // Mark the selected thread dirty
            // and add the payload to it
            thread.Payloads.Add(id, payload);
            thread.Dirty = true;
            
            _payloadLocation.Add(id, thread.Identifier);
        }
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Payload {id} successfully added!");
    }

    /// <summary>
    /// Remove a payload
    /// </summary>
    /// <param name="id">Payload's Identifier</param>
    public void RemovePayload(string id)
    {
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Removing payload {id}...");
        lock (_threads) {
            var threadId = _payloadLocation[id];
            
            /* NOTICE:
             * We do not need to lock the Value,
             * nor the Payloads field because
             * the thread clones the field
             * for caching, but also preventing
             * an Exception caused by the
             * collection being updated.
             *
             * For it to be updated on the
             * thread's side, we just set
             * the Dirty Bit to true.
             */
            _threads[threadId].Payloads.Remove(id);
            _threads[threadId].Dirty = true;
        }
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Payload {id} successfully removed!");
    }

    /// <summary>
    /// Stop all threads
    /// </summary>
    public void StopAllThreads()
    {
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Shutting down all threads...");

        var watch = new Stopwatch();
        watch.Start();
        Shutdown = true;
        
        /* NOTICE:
         * We should give a possibility
         * for threads to remove themselves
         * from the _threads field.
         * 
         * If the while loop would've been
         * inside of the lock, then we
         * would cause a deadlock.
         */
        var done = false;
        while (!done)
            lock (_threads)
                done = _threads.Count == 0;
        watch.Stop();
        AnsiConsole.MarkupLine($"[grey]INFO:[/] All threads stopped in {watch.Elapsed}!");
    }

    /// <summary>
    /// Create a thread
    /// </summary>
    private void CreateThread()
    {
        lock (_threads) {
            if (_threads.Count + 1 > _threadsCount)
                return;
            
            AnsiConsole.MarkupLine($"[grey]INFO:[/] Creating a new thread...");
            var id = _threads.Count;
            ThreadPool.QueueUserWorkItem(ThreadRoutine, id);
            
            AnsiConsole.MarkupLine($"[grey]INFO:[/] Finished successfully!");
        }
    }

    /// <summary>
    /// Thread's routine
    /// </summary>
    /// <param name="stateInfo">Thread ID</param>
    private async void ThreadRoutine(object? stateInfo)
    {
        var id = (int)stateInfo!;

        void LogInfo(string msg)
            => AnsiConsole.MarkupLine($"[aqua]Thread {id}[/] [grey]INFO:[/] {msg}");

        void LogError(string msg)
            => AnsiConsole.MarkupLine($"[aqua]Thread {id}[/] [red]ERROR:[/] {msg}");
        
        LogInfo($"Assigned Thread ID {id}");
        lock (_threads) _threads[id].Identifier = id;
        Dictionary<string, Payloads.Payload> payloads = new();
        try {
            while (!Shutdown) {
                lock (_threads) {
                    if (_threads[id].Dirty) {
                        LogInfo("Reloading information...");
                        
                        // Make a clone, else if would not be caching at all
                        payloads = _threads[id].Payloads.ToDictionary(
                            entry => entry.Key,
                            entry => entry.Value);
                        _threads[id].Dirty = false;
                    }
                }
                
                foreach (var i in payloads) {
                    if (i.Value.IsSubPayloads) {
                        while (i.Value.Payloads.GetQueueLength() != 0) { }
                        await Task.Delay(i.Value.UseQueueAttribute.DelayBeforeNext);
                        
                        lock (_threads) _threads[id].Payloads.Remove(i.Key);
                        continue;
                    }
                    
                    try { i.Value.Method.Invoke(null, null); }
                    catch (Exception e) {
                        LogError($"An exception occured while running {i.Key}!");
                        AnsiConsole.WriteException(e);
                    }
                    
                    if (i.Value.PayloadAttribute.RunOnce)
                        lock (_threads) _threads[id].Payloads.Remove(i.Key);
                }
            }
        } catch (Exception e) {
            LogError($"Thread's routine crashed!");
            AnsiConsole.WriteException(e);
        }
        
        LogInfo("Shutting down...");
        lock(_threads) _threads.Remove(id);
    }
}