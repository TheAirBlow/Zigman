// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Zigman.Engine.Legacy;

/// <summary>
/// A payload
/// </summary>
public class Payload
{
    /// <summary>
    /// How safe is this payload
    /// </summary>
    public enum SafetyLevel
    {
        /// <summary>
        /// This payload is completely safe
        /// and can be easily reverted. 
        /// Example: shaking any window everywhere to clear out everything
        /// </summary>
        Safe = 0,
        
        /// <summary>
        /// This payload may destroy something,
        /// but can be reverted by restarting the OS
        /// or the application that was affected.
        /// </summary>
        SemiDestructive = 1,
        
        /// <summary>
        /// This payload pernametly destroys something,
        /// and it cannot be reverted easily and requires
        /// to restore essential parts of the OS,
        /// or even the bootsector or the partition table.
        /// </summary>
        Destructive = 2
    }
    
    /// <summary>
    /// Can be this payload get stacked
    /// with other ones on a one thread
    /// </summary>
    public bool CanBeStacked = false;

    /// <summary>
    /// Payload itself.
    /// </summary>
    public Action Action = null!;

    /// <summary>
    /// How safe is this payload
    /// </summary>
    public SafetyLevel LevelOfSafety;

    /// <summary>
    /// Is the payload enabled
    /// </summary>
    public bool Enabled = false;
}