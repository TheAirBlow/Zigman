// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Zigman.Engine.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class UseQueueAttribute : Attribute
{
    /// <summary>
    /// Position in queue
    /// </summary>
    public int QueuePosition;
    
    /// <summary>
    /// Delay before next payload, in milliseconds.
    /// In case of a sub-payloads, it would wait
    /// until the sub-payloads' queue would end,
    /// and then wait for DelayBeforeNext.
    /// </summary>
    public int DelayBeforeNext;

    /// <summary>
    /// Automatically stop after DelayBeforeNext runs out
    /// </summary>
    public bool AutoStop;

    /// <summary>
    /// Add this payload to the Queue
    /// </summary>
    /// <param name="pos">Position in queue</param>
    /// <param name="delay">Delay before next payload, in seconds.</param>
    /// <param name="autoStop">Automatically stop after DelayBeforeNext runs out</param>
    public UseQueueAttribute(int pos, int delay, bool autoStop = false)
    {
        AutoStop = autoStop;
        QueuePosition = pos;
        DelayBeforeNext = delay;
    }
}