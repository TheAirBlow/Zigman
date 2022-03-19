// Copyright © TheAirBlow 2022 <theairblow.help@gmail.com>
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using TheAirBlow.Zigman;
using TheAirBlow.Zigman.Audio;

namespace TheAirBlow.Amogus
{
    public static class Program
    {
        public static AudioManager Manager = new();
        
        public static void Main(string[] args)
        {
            var trojan = new Trojan {
                Title = "Amogus Trojan",
                Author = "TheAirBlow"
            };
            trojan.RegisterClass<AllPayloads>();
            trojan.Run();
        }
    }
}