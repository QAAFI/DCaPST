using DCAPST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Startup.Models
{
    public class Clock : IClock
    {
        public DateTime Today { get; set; }

        /// <summary>Returns the current fraction of the overall simulation which has been completed</summary>
        public double FractionComplete { get; set; }

    }
}
