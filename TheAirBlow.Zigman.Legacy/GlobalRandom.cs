// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace TheAirBlow.Zigman.Legacy;

/// <summary>
/// Global random
/// </summary>
public partial class Trojan
{
    /// <summary>
    /// Random instance
    /// </summary>
    public Random Random = new Random();
    
    /// <summary>
    /// Generate a random string
    /// </summary>
    /// <param name="chars">Charset</param>
    /// <param name="length">Length</param>
    /// <returns>Random string</returns>
    public string RandomString(string chars, int length)
        => new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
}