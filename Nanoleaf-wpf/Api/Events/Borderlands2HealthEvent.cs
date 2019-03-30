﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winleafs.Wpf.Helpers;

/*
{
  "Health_maximum": {
    "baseAddress": "32506856",
    "pointers": [ "0", "1760", "664", "940", "88" ]
  },

  "Health_current": {
    "baseAddress": "32506856",
    "pointers": [ "0", "1760", "664", "940", "108" ]
  },

  "Shield_maximum": {
    "baseAddress": "32506856",
    "pointers": [ "0", "1760", "664", "952", "88" ]
  },

  "Shield_current": {
    "baseAddress": "32506856",
    "pointers": [ "0", "1760", "664", "952", "108" ]
  }
}
*/


namespace Winleafs.Wpf.Api.Events
{
    public class Borderlands2HealthEvent : BaseProcessPercentageEvent
    {
        private static readonly string _processName = "Borderlands2";
        private static readonly int _maxHealthBaseAddress = 32506856;
        private static readonly int[] _maxHealthPointers = { 0, 1760, 664, 940, 88 };
        private static readonly int _currentHealthBaseAddress = 32506856;
        private static readonly int[] _currentHealthPointers = { 0, 1760, 664, 940, 108 };

        public Borderlands2HealthEvent(Orchestrator orchestrator) : base(orchestrator, _processName)
        {

        }

        protected override async Task ApplyEffectLocalAsync(MemoryReader memoryReader)
        {
            var maxHealth = memoryReader.ReadFloat(_maxHealthBaseAddress, _maxHealthPointers);
            var currentHealth = memoryReader.ReadFloat(_currentHealthBaseAddress, _currentHealthPointers);

            try
            {
                var percentage = (100 / maxHealth) * currentHealth;
            }
            catch
            {
                //Do nothing
            }
        }
    }
}
