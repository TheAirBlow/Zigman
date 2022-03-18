// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using System.Diagnostics;
using System.Reflection;
using System.Timers;
using Spectre.Console;
using Zigman.Engine.Attributes;
using Zigman.Engine.Exceptions;
using Timer = System.Timers.Timer;

namespace Zigman.Engine;

/// <summary>
/// Payloads manager.
/// </summary>
public partial class Payloads
{
    /// <summary>
    /// A payload.
    /// </summary>
    public class Payload
    {
        public UseQueueAttribute UseQueueAttribute;
        public PayloadAttribute PayloadAttribute;
        public MethodInfo Method;
        
        // Make sub-payloads possible
        public bool IsSubPayloads;
        public Payloads Payloads;
    }

    /// <summary>
    /// All payloads
    /// </summary>
    private Dictionary<string, Payload> _payloads = new();

    /// <summary>
    /// Queue timer
    /// </summary>
    private Timer _timer = new();

    /// <summary>
    /// Payloads queue
    /// </summary>
    private Queue<Payload> _queue = new();

    /// <summary>
    /// Threads Manager
    /// </summary>
    private ThreadsManager _manager;

    /// <summary>
    /// Warn about non-static members
    /// </summary>
    private bool _warn;
    
    /// <summary>
    /// Safety Level
    /// </summary>
    private SafetyLevel _level;

    /// <summary>
    /// Get the queue's length
    /// </summary>
    /// <returns>Queue's Length</returns>
    internal int GetQueueLength()
        => _queue.Count;
    
    /// <summary>
    /// Count of sub-payloads.
    /// </summary>
    private static int SubPayloadsCount;

    /// <summary>
    /// Modify IDs for this Payloads instance
    /// to be formatted properly.
    /// </summary>
    internal void ModifyIds()
    {
        // Why is this even required? It is because
        // the ThreadsManager is shared among all
        // of the sub-threads, and there may be
        // payloads with same IDs inside different
        // Payloads classes.
        var toReplace = new Dictionary<string, string>();
        foreach (var i in _payloads) {
            var newId = $"SubPayload{SubPayloadsCount}." +
                        $"{i.Value.PayloadAttribute.Identifier}";
            i.Value.PayloadAttribute.Identifier = newId;
            toReplace.Add(i.Key, newId);
        }
        
        foreach (var i in toReplace) {
            var value = _payloads[i.Key];
            _payloads.Remove(i.Key);
            _payloads.Add(i.Value, value);
        }
        
        SubPayloadsCount++;
    }

    /// <summary>
    /// Set the settings to use
    /// </summary>
    /// <param name="manager">Threads Manager</param>
    /// <param name="warn">Warn about non-static members</param>
    /// <param name="level">Safety Level</param>
    internal void SetSettings(ThreadsManager manager, bool warn, SafetyLevel level)
    {
        _manager = manager;
        _level = level;
        _warn = warn;
    }

    /// <summary>
    /// Load all payloads
    /// </summary>
    internal void Initialize()
    {
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Initializing Payloads...");
        var watch = new Stopwatch();
        watch.Start();
        if (_warn) AnsiConsole.MarkupLine($"[grey]INFO:[/] Set WarnAboutNonStatic" +
                                         $" to false if you want to disable " +
                                         $"non-static warnings!");
        
        // We would make sure the queue positions are unique
        var queue = new SortedDictionary<int, Payload>();
        var names = new List<string>();
        foreach (var i in GetType().GetMethods()) {
            // Ignore non-static members
            if (i.IsStatic) {
                if (_warn) AnsiConsole.MarkupLine($"[yellow]WARN:[/] Found non-static method {i.Name}");
                continue;
            }
            
            // Check for arguments
            if (i.GetGenericArguments().Length != 0)
                throw new InvalidPayloadException($"Arguments present, method {i.Name}!");

            // Get the payload attribute
            var payloadAttr = i.GetCustomAttributes(
                typeof(PayloadAttribute), false);
            if (payloadAttr.Length == 0)
                continue;
            var payloadAttrContent = (PayloadAttribute)payloadAttr[0];
            
            // Skip if the SafetyLevel if higher
            if (payloadAttrContent.SafetyLevel > _level) {
                AnsiConsole.MarkupLine($"[yellow]WARN:[/] {i.Name}'s safety level is too high, skipping...");
                continue;
            }
            
            var payload = new Payload {
                PayloadAttribute = payloadAttrContent,
                Method = i
            };

            // Check if the ID is unique
            if (names.Contains(payloadAttrContent.Identifier))
                throw new IdentifierException($"Multiple payloads with ID " +
                                              $"{payloadAttrContent.Identifier}!");
            names.Add(payloadAttrContent.Identifier);
            
            // Add to the queue if needed
            var queueAttr = i.GetCustomAttributes(
                typeof(UseQueueAttribute), false);
            if (queueAttr.Length != 0) {
                var queueAttrContent = 
                    (UseQueueAttribute)queueAttr[0];
                payload.UseQueueAttribute = queueAttrContent;
                try { queue.Add(queueAttrContent.QueuePosition,
                        payload); } 
                catch (ArgumentException) {
                    throw new QueuePositionException($"Non-unique position for {i.Name}!");
                } catch (Exception e) {
                    throw new UnknownErrorException("An unknown error occured!", e);
                }
            }
            
            // Process sub-payloads
            if (i.ReturnType.IsSubclassOf(typeof(Payloads))) {
                // Get the return value
                var obj = i.Invoke(null, null);
                if (obj == null) 
                    throw new InvalidPayloadException($"Return value is null, method {i.Name}!");
                var value = (Payloads)obj; 
                
                // Setup the Payloads instance
                value.SetSettings(_manager, _warn, _level);
                value.Initialize(); value.ModifyIds();
                
                // Make the thread wait until the Payload's queue gets emptied
                payload.IsSubPayloads = true;
                payload.Payloads = value;

                // Replace some things to prevent unexpected behaviour
                payload.PayloadAttribute.RunOnce = true;
            } 

            if (i.ReturnType != typeof(void))
                throw new InvalidPayloadException($"Invalid return type, method {i.Name}!");
            
            // Save the payload's MethodInfo
            _payloads.Add(payloadAttrContent.Identifier, payload);
        }
        
        // Here we actually enqueue the payloads
        foreach (var i in queue.Values)
            _queue.Enqueue(i);

        watch.Stop();
        AnsiConsole.MarkupLine($"[grey]INFO:[/] Initialization done in {watch.Elapsed}!");
    }

    /// <summary>
    /// Stop all threads.
    /// Notice: Would stop ALL threads,
    /// including sub-payloads ones!
    /// </summary>
    public void StopAllThreads()
        => _manager.StopAllThreads();

    /// <summary>
    /// Stops all payloads from this
    /// exact Payloads instance.
    /// </summary>
    public void StopAllPayloads()
    {
        foreach (var i in _payloads)
            _manager.RemovePayload(i.Key);
    }

    /// <summary>
    /// Starts up the queue
    /// </summary>
    internal void StartQueue()
    {
        AnsiConsole.MarkupLine("[grey]INFO:[/] Starting queue...");
        
        _timer.Interval = 1;
        // Not the best way to do this,
        // but I do not want extra arguments.
        _timer.Elapsed += (_, _) => TimerRoutine();
        _timer.Start();
        
        AnsiConsole.MarkupLine("[grey]INFO:[/] Queue successfully started!");
    }

    /// <summary>
    /// The payload to be stopped.
    /// </summary>
    private string? _toStop;
    
    /// <summary>
    /// Timer's routine
    /// </summary>
    private void TimerRoutine()
    {
        if (_toStop != null) {
            _manager.RemovePayload(_toStop);
            _toStop = null;
        }
        if (_queue.Count == 0) {
            AnsiConsole.MarkupLine("[yellow]WARN:[/] The queue got emptied!");
            _timer.Stop();
            return;
        }
        var item = _queue.Dequeue();
        _manager.AddPayload(item.PayloadAttribute.Identifier, item);
        _timer.Interval = item.UseQueueAttribute.DelayBeforeNext;
        if (item.UseQueueAttribute.AutoStop)
            _toStop = item.PayloadAttribute.Identifier;
    }
}