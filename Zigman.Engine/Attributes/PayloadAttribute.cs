// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Zigman.Engine.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PayloadAttribute : Attribute
{
    /// <summary>
    /// Payload's Identifier
    /// </summary>
    public string Identifier;

    /// <summary>
    /// Safety Level
    /// </summary>
    public SafetyLevel SafetyLevel;

    /// <summary>
    /// Should this payload only ran once?
    /// Keep in mind: It would ignore
    /// UseQueue's delay that you've set.
    /// </summary>
    public bool RunOnce;

    /// <summary>
    /// A payload.
    /// </summary>
    /// <param name="id">Payload's Identifier</param>
    /// <param name="level">Safety Level</param>
    /// <param name="runOnce">
    /// Should this payload only ran once?
    /// Keep in mind: It would ignore
    /// UseQueue's delay that you've set.
    /// </param>
    public PayloadAttribute(string id, SafetyLevel level, bool runOnce = false)
    {
        SafetyLevel = level;
        RunOnce = runOnce;
        Identifier = id;
    }
}