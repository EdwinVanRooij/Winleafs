﻿using Nanoleaf_Models.Models.Scheduling.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanoleaf_Models.Models.Scheduling
{
    public class Program
    {
        public List<TimeTrigger> Triggers { get; set; }

        public Program()
        {
            Triggers = new List<TimeTrigger>();
        }

        public void AddTrigger(TimeTrigger trigger)
        {
            Triggers.Add(trigger);
            ReorderTriggers();
        }

        /// <summary>
        /// Returns the TimeTrigger that should be active at this moment
        /// </summary>
        public void ReorderTriggers()
        {
            var now = DateTime.Now;
            Triggers = Triggers.OrderBy(t => t.GetActualDateTime(now)).ToList();
        }
    }
}
